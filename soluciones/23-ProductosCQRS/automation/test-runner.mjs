import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "23-ProductosCQRS",
  csproj: "ProductosTest/ProductosTest.csproj",
  baseUrl: "http://localhost:5110",
  healthPaths: ["/swagger", "/", "/api/Productos"],
  env: { ASPNETCORE_ENVIRONMENT: "Development", ASPNETCORE_URLS: "http://localhost:5110" },
  // Puertos host de postgres/mongo/redis publicados en docker-compose.yml
  dbServices: ["postgres", "mongo", "redis"],
  fallbackComposeFile: "docker-compose.api.yml",
  prepareDb: [],
};

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
let exitCode = 0;
const log = (m) => console.log(m);
const logOk = (m) => console.log(`  ✅ ${m}`);
const logErr = (m) => console.error(`  ❌ ${m}`);

function run(cmd) {
  const res = spawnSync(cmd, { cwd: ROOT, encoding: "utf8", shell: true });
  if (res.status !== 0) throw new Error(`Fallo: ${cmd}\n${(res.stdout || "") + (res.stderr || "")}`.slice(0, 2000));
  return res;
}
function tryRun(cmd) {
  try { run(cmd); } catch (e) { log(`  ⚠️ (tolerado) ${e.message.split("\n")[0]}`); }
}
function dockerAvailable() {
  try { return spawnSync("docker", ["compose", "version"], { encoding: "utf8", shell: true }).status === 0; }
  catch { return false; }
}
function killTree(pid) {
  if (!pid) return;
  try { spawnSync("taskkill", ["/pid", String(pid), "/T", "/F"], { shell: true }); }
  catch { try { process.kill(pid); } catch { } }
}
function spawnApi(csproj, envExtra = {}) {
  const child = spawn("dotnet", ["run", "--project", csproj], {
    cwd: ROOT, env: { ...process.env, ...CONFIG.env, ...envExtra },
    stdio: ["ignore", "pipe", "pipe"], shell: true,
  });
  child.stdout.on("data", d => process.stdout.write(`   [api] ${d}`));
  child.stderr.on("data", d => process.stderr.write(`   [api] ${d}`));
  return child;
}
async function fetchAny(url) {
  try {
    const ctrl = new AbortController();
    const t = setTimeout(() => ctrl.abort(), 3000);
    const res = await fetch(url, { signal: ctrl.signal });
    clearTimeout(t);
    return res;
  } catch { return null; }
}
async function waitForApi(timeoutMs = 20000) {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    for (const p of CONFIG.healthPaths) {
      if (await fetchAny(CONFIG.baseUrl + p)) return true;
    }
    await sleep(500);
  }
  return false;
}
async function req(method, urlPath, { body, headers = {}, token, formData } = {}) {
  const h = { ...headers };
  if (formData) {
    // FormData nativo: fetch pone el boundary; NO setear Content-Type
  } else if (body !== undefined) {
    h["Content-Type"] = h["Content-Type"] || "application/json";
  }
  if (token) h.Authorization = `Bearer ${token}`;
  const res = await fetch(CONFIG.baseUrl + urlPath, {
    method, headers: h,
    body: formData || (body !== undefined ? (typeof body === "string" ? body : JSON.stringify(body)) : undefined),
  });
  let text = ""; try { text = await res.text(); } catch { }
  let json = null; try { json = text ? JSON.parse(text) : null; } catch { }
  return { status: res.status, text, json, headers: res.headers };
}
const resultados = [];
async function test(nombre, fn) {
  try { await fn(); resultados.push({ nombre, ok: true }); logOk(nombre); }
  catch (e) { resultados.push({ nombre, ok: false, error: e.message }); logErr(`${nombre} → ${e.message}`); exitCode = 1; }
}
function assert(c, m) { if (!c) throw new Error(m); }
function assertEq(a, e, m) { if (a !== e) throw new Error(`${m}: obtenido ${JSON.stringify(a)}, esperado ${JSON.stringify(e)}`); }

async function runSuite() {
  // CQRS: Mongo arranca vacío; el POST dispara el evento de sincronización de forma síncrona
  let token = null;
  let productoId = null;
  const emailUnico = `cqrs_${Date.now()}@test.com`;

  await test("POST /api/Auth/login admin → 200 con token", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: "admin@admin.com", password: "Admin123!" },
    });
    assertEq(r.status, 200, "status");
    assert(r.json?.token, "la respuesta no contiene token");
    token = r.json.token;
  });

  await test("POST /api/Auth/login incorrecto → 401", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: "admin@admin.com", password: "Wrong123!" },
    });
    assertEq(r.status, 401, "status");
  });

  await test("POST /api/Auth/signup email único → 200", async () => {
    const r = await req("POST", "/api/Auth/signup", {
      body: { email: emailUnico, password: "Pass123!", nombre: "Test", apellidos: "Runner" },
    });
    assertEq(r.status, 200, "status");
    assert(r.json?.token, "signup no devuelve token");
  });

  await test("GET /api/Auth/me sin token → 401", async () => {
    const r = await req("GET", "/api/Auth/me");
    assertEq(r.status, 401, "status");
  });

  await test("GET /api/Auth/me con token → 200", async () => {
    const r = await req("GET", "/api/Auth/me", { token });
    assertEq(r.status, 200, "status");
  });

  await test("GET /api/Productos → 200 (array, puede venir [])", async () => {
    const r = await req("GET", "/api/Productos");
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
  });

  await test("POST /api/Productos → 200 (NO 201) y guarda id", async () => {
    const r = await req("POST", "/api/Productos", {
      token,
      body: { nombre: `Producto CQRS ${Date.now()}`, precio: 19.99, categoriaId: 1 },
    });
    assertEq(r.status, 200, "status");
    productoId = r.json?.id ?? r.json?.Id;
    assert(productoId !== undefined && productoId !== null, "la respuesta no contiene id");
  });

  await test("GET /api/Productos/{id} tras POST → 200 (sync por evento)", async () => {
    const r = await req("GET", `/api/Productos/${productoId}`);
    assertEq(r.status, 200, "status");
    assert(r.json, "el producto no aparece tras el POST");
  });

  await test("PUT /api/Productos/{id} → 200", async () => {
    const r = await req("PUT", `/api/Productos/${productoId}`, {
      token,
      body: { nombre: "Producto CQRS Editado", precio: 29.99, categoriaId: 1 },
    });
    assertEq(r.status, 200, "status");
  });

  await test("GET /api/Productos/{id} tras PUT → 200 con datos actualizados (sync por evento)", async () => {
    const r = await req("GET", `/api/Productos/${productoId}`);
    assertEq(r.status, 200, "status");
    const nombre = r.json?.nombre ?? r.json?.Nombre;
    const precio = r.json?.precio ?? r.json?.Precio;
    assertEq(nombre, "Producto CQRS Editado", "nombre sincronizado en read model");
    assertEq(precio, 29.99, "precio sincronizado en read model");
  });

  await test("DELETE /api/Productos/{id} → 204", async () => {
    const r = await req("DELETE", `/api/Productos/${productoId}`, { token });
    assertEq(r.status, 204, "status");
  });

  await test("GET /api/Productos/{id} tras DELETE → 404", async () => {
    const r = await req("GET", `/api/Productos/${productoId}`);
    assertEq(r.status, 404, "status");
  });

  await test("GET /api/Productos/search → 404 (endpoint no existe en CQRS)", async () => {
    const r = await req("GET", "/api/Productos/search?q=teclado");
    assertEq(r.status, 404, "status");
  });
}

async function main() {
  log(`\n=== ${CONFIG.nombre} · test-runner (Node nativo) ===\n`);
  let procs = [];
  const composeFiles = [];
  if (existsSync(path.join(ROOT, "docker-compose.yml"))) composeFiles.push("docker-compose.yml");
  if (CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))) composeFiles.push(CONFIG.fallbackComposeFile);
  try {
    if (composeFiles.length && CONFIG.dbServices.length && dockerAvailable()) {
      log(`▶ Levantando infraestructura: ${CONFIG.dbServices.join(", ")}`);
      run(`docker compose up -d ${CONFIG.dbServices.join(" ")}`);
      for (const c of CONFIG.prepareDb) { log(`▶ Prepare: ${c}`); tryRun(c); }
      await sleep(2000);
    }
    log("▶ dotnet restore"); run(`dotnet restore "${CONFIG.csproj}"`);
    log("▶ dotnet build"); run(`dotnet build "${CONFIG.csproj}" -c Debug --no-restore`);
    log("▶ dotnet run (segundo plano)");
    procs.push(spawnApi(CONFIG.csproj));
    let listo = await waitForApi(20000);
    if (!listo && composeFiles.length && dockerAvailable()) {
      console.warn("  ⚠️ CLI falló → fallback docker compose up -d --build");
      procs.forEach(p => killTree(p.pid)); procs = [];
      const file = CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))
        ? CONFIG.fallbackComposeFile : "docker-compose.yml";
      log(`▶ Fallback: docker compose -f ${file} up -d --build`);
      run(`docker compose -f ${file} up -d --build`);
      listo = await waitForApi(20000);
    }
    if (!listo) throw new Error("La API no respondió en 20s");
    log("\n▶ Ejecutando suite de tests HTTP...\n");
    await runSuite();
  } catch (e) {
    logErr(`Error fatal: ${e.message}`); exitCode = 1;
  } finally {
    log("\n▶ Limpieza...");
    procs.forEach(p => killTree(p.pid));
    if (composeFiles.length) {
      for (const f of new Set([...composeFiles].reverse())) {
        try { run(`docker compose -f ${f} down -v`); } catch (e) { logErr(`down ${f}: ${e.message}`); }
      }
    }
  }
  const pass = resultados.filter(r => r.ok).length, fail = resultados.length - pass;
  log("\n========== RESUMEN ==========");
  log(`Total: ${resultados.length} · OK: ${pass} · KO: ${fail}`);
  for (const r of resultados.filter(r => !r.ok)) log(`  ✗ ${r.nombre}: ${r.error}`);
  log("==============================\n");
  process.exit(exitCode);
}
main();

import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "15-ProductosIdentity",
  csproj: "ProductosTest/ProductosTest.csproj",
  baseUrl: "http://localhost:5000",
  healthPaths: ["/swagger", "/", "/api/Productos"],
  env: { ASPNETCORE_ENVIRONMENT: "Development", ASPNETCORE_URLS: "http://localhost:5000" },
  dbServices: ["db", "redis"],
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
  if (res.status !== 0) throw new Error(`Fallo: ${cmd}\n${(res.stdout||"")+(res.stderr||"")}`.slice(0,2000));
  return res;
}
function dockerAvailable() {
  try { return spawnSync("docker", ["compose","version"], { encoding:"utf8", shell:true }).status === 0; }
  catch { return false; }
}
function killTree(pid) {
  if (!pid) return;
  try { spawnSync("taskkill", ["/pid", String(pid), "/T", "/F"], { shell: true }); }
  catch { try { process.kill(pid); } catch {} }
}
function spawnApi() {
  const child = spawn("dotnet", ["run", "--project", CONFIG.csproj], {
    cwd: ROOT, env: { ...process.env, ...CONFIG.env },
    stdio: ["ignore","pipe","pipe"], shell: true,
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
async function req(method, urlPath, { body, headers = {}, token, raw } = {}) {
  const h = { ...headers };
  if (!raw && body !== undefined && !(body instanceof FormData)) h["Content-Type"] = h["Content-Type"] || "application/json";
  if (token) h.Authorization = `Bearer ${token}`;
  const res = await fetch(CONFIG.baseUrl + urlPath, {
    method, headers: h,
    body: body instanceof FormData ? body : (body !== undefined ? (typeof body === "string" ? body : JSON.stringify(body)) : undefined),
  });
  let text = ""; try { text = await res.text(); } catch {}
  let json = null; try { json = text ? JSON.parse(text) : null; } catch {}
  return { status: res.status, text, json, headers: res.headers };
}
const resultados = [];
async function test(nombre, fn) {
  try { await fn(); resultados.push({nombre, ok:true}); logOk(nombre); }
  catch (e) { resultados.push({nombre, ok:false, error:e.message}); logErr(`${nombre} → ${e.message}`); exitCode = 1; }
}
function assert(c, m) { if (!c) throw new Error(m); }
function assertEq(a, e, m) { if (a !== e) throw new Error(`${m}: obtenido ${JSON.stringify(a)}, esperado ${JSON.stringify(e)}`); }

async function runSuite() {
  const baseA = "/api/Auth";
  const baseP = "/api/Productos";
  let adminToken = null;
  let idCreado = null;

  // ── Auth ──────────────────────────────────────────────────────────────────
  await test("GET /api/Auth/me sin token → 401", async () => {
    const r = await req("GET", `${baseA}/me`);
    assertEq(r.status, 401, "status");
  });

  await test("POST /api/Auth/login admin@admin.com → 200 con token", async () => {
    const r = await req("POST", `${baseA}/login`, {
      body: { email: "admin@admin.com", password: "Admin123!" },
    });
    assertEq(r.status, 200, "status");
    adminToken = r.json?.token ?? r.json?.Token;
    assert(typeof adminToken === "string" && adminToken.length > 0, "no se ha devuelto token");
    assertEq(r.json?.role ?? r.json?.Role, "ADMIN", "role");
  });

  await test("GET /api/Auth/me con token admin → 200", async () => {
    const r = await req("GET", `${baseA}/me`, { token: adminToken });
    assertEq(r.status, 200, "status");
    assertEq(r.json?.role ?? r.json?.Role, "ADMIN", "role");
  });

  await test("POST login con credenciales malas → 401", async () => {
    const r = await req("POST", `${baseA}/login`, {
      body: { email: "admin@admin.com", password: "WrongPass1!" },
    });
    assertEq(r.status, 401, "status");
  });

  await test("POST /api/Auth/signup email nuevo → 200", async () => {
    const email = `test.identity.${Date.now()}@example.com`;
    const r = await req("POST", `${baseA}/signup`, {
      body: { email, password: "Abc12345!", nombre: "Test", apellidos: "Identity" },
    });
    assertEq(r.status, 200, "status");
    assert(r.json && (r.json.token ?? r.json.Token), "signup debe devolver token");
  });

  // ── Productos con autorización ────────────────────────────────────────────
  await test("GET productos sin auth → 200 (seed ≥10)", async () => {
    const r = await req("GET", baseP);
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
    assert(r.json.length >= 10, `se esperaban ≥10 productos, hay ${r.json.length}`);
  });

  await test("POST producto sin token → 401", async () => {
    const r = await req("POST", baseP, {
      body: { nombre: "No Auth", precio: 1.0, categoriaId: 1 },
    });
    assertEq(r.status, 401, "status");
  });

  await test("POST producto con token ADMIN → 201", async () => {
    const r = await req("POST", baseP, {
      body: { nombre: "Producto Identity Test", precio: 33.3, categoriaId: 1 },
      token: adminToken,
    });
    assertEq(r.status, 201, "status");
    idCreado = r.json?.id ?? r.json?.Id;
    assert(idCreado !== undefined && idCreado !== null, "no se ha devuelto id");
  });

  await test(`PUT /${idCreado ?? "?"} con ADMIN → 200`, async () => {
    const r = await req("PUT", `${baseP}/${idCreado}`, {
      body: { nombre: "Producto Identity Test v2", precio: 34.3, categoriaId: 1 },
      token: adminToken,
    });
    assertEq(r.status, 200, "status");
    assertEq(r.json?.nombre ?? r.json?.Nombre, "Producto Identity Test v2", "nombre actualizado");
  });

  await test(`DELETE /${idCreado ?? "?"} con ADMIN → 204`, async () => {
    const r = await req("DELETE", `${baseP}/${idCreado}`, { token: adminToken });
    assertEq(r.status, 204, "status");
  });

  await test(`GET /${idCreado ?? "?"} tras delete → 404`, async () => {
    const r = await req("GET", `${baseP}/${idCreado}`);
    assertEq(r.status, 404, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });
}

async function main() {
  log(`\n=== ${CONFIG.nombre} · test-runner (Node nativo) ===\n`);
  let apiProc = null;
  const composeFiles = [];
  if (existsSync(path.join(ROOT, "docker-compose.yml"))) composeFiles.push("docker-compose.yml");
  if (CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))) composeFiles.push(CONFIG.fallbackComposeFile);
  try {
    if (composeFiles.length && CONFIG.dbServices.length && dockerAvailable()) {
      log(`▶ Levantando infraestructura: ${CONFIG.dbServices.join(", ")}`);
      run(`docker compose up -d ${CONFIG.dbServices.join(" ")}`);
      for (const c of CONFIG.prepareDb) { log(`▶ Prepare: ${c}`); run(c); }
      await sleep(2000);
    }
    log("▶ dotnet restore"); run(`dotnet restore "${CONFIG.csproj}"`);
    log("▶ dotnet build");  run(`dotnet build "${CONFIG.csproj}" -c Debug --no-restore`);
    log("▶ dotnet run (segundo plano)");
    apiProc = spawnApi();
    let listo = await waitForApi(20000);
    if (!listo && composeFiles.length && dockerAvailable()) {
      console.warn("  ⚠️ CLI falló → fallback docker compose up -d --build");
      killTree(apiProc?.pid); apiProc = null;
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
    if (apiProc) killTree(apiProc.pid);
    if (composeFiles.length) {
      for (const f of new Set([...composeFiles].reverse())) {
        try { run(`docker compose -f ${f} down -v`); } catch (e) { logErr(`down ${f}: ${e.message}`); }
      }
    }
  }
  const pass = resultados.filter(r=>r.ok).length, fail = resultados.length - pass;
  log("\n========== RESUMEN ==========");
  log(`Total: ${resultados.length} · OK: ${pass} · KO: ${fail}`);
  for (const r of resultados.filter(r=>!r.ok)) log(`  ✗ ${r.nombre}: ${r.error}`);
  log("==============================\n");
  process.exit(exitCode);
}
main();

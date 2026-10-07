import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "07-ProductosResult",
  csproj: "ProductosResult/ProductosResult.csproj",
  baseUrl: "http://localhost:5000",
  healthPaths: ["/swagger", "/api/productos", "/"],
  env: { ASPNETCORE_ENVIRONMENT: "Development", ASPNETCORE_URLS: "http://localhost:5000" },
  dbServices: [],
  fallbackComposeFile: null,
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
async function req(method, urlPath, { body, headers = {}, token } = {}) {
  const h = { "Content-Type": "application/json", ...headers };
  if (token) h.Authorization = `Bearer ${token}`;
  const res = await fetch(CONFIG.baseUrl + urlPath, {
    method, headers: h,
    body: body !== undefined ? (typeof body === "string" ? body : JSON.stringify(body)) : undefined,
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
  const base = "/api/productos";
  let id = null;

  // La API arranca sin seed: la lista debe venir vacía
  await test("GET lista → 200 (vacío)", async () => {
    const r = await req("GET", base);
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
    assertEq(r.json.length, 0, "la lista inicial debe estar vacía");
  });

  await test("POST producto válido → 201 con id", async () => {
    const r = await req("POST", base, {
      body: { nombre: "Portátil Ultra", precio: 899.99, categoria: "Electrónica", descripcion: "Test 07" },
    });
    assertEq(r.status, 201, "status");
    id = r.json?.id ?? r.json?.Id;
    assert(id !== undefined && id !== null, "no se ha devuelto id");
  });

  await test(`GET /${"{}"} por id → 200`.replace("{}", String(id ?? "")), async () => {
    const r = await req("GET", `${base}/${id}`);
    assertEq(r.status, 200, "status");
    assertEq(Number(r.json?.id ?? r.json?.Id), Number(id), "id del recurso");
  });

  await test("PUT por id → 200", async () => {
    const r = await req("PUT", `${base}/${id}`, {
      body: { id, nombre: "Portátil Ultra v2", precio: 799.99, categoria: "Electrónica", descripcion: "Actualizado" },
    });
    assertEq(r.status, 200, "status");
  });

  await test("PATCH precio 42.5 (body número crudo) → 200", async () => {
    const r = await req("PATCH", `${base}/${id}/precio`, { body: "42.5" });
    assertEq(r.status, 200, "status");
  });

  await test("PATCH precio -1 → 400", async () => {
    const r = await req("PATCH", `${base}/${id}/precio`, { body: "-1" });
    assertEq(r.status, 400, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });

  await test("DELETE por id → 204", async () => {
    const r = await req("DELETE", `${base}/${id}`);
    assertEq(r.status, 204, "status");
  });

  await test("GET recurso borrado → 404", async () => {
    const r = await req("GET", `${base}/${id}`);
    assertEq(r.status, 404, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });

  await test("GET id inexistente 999 → 404", async () => {
    const r = await req("GET", `${base}/999`);
    assertEq(r.status, 404, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });

  await test("GET /search?termino=… → 200", async () => {
    const r = await req("GET", `${base}/search?termino=portatil`);
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
  });

  await test("GET /search sin termino → 400", async () => {
    const r = await req("GET", `${base}/search`);
    assertEq(r.status, 400, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });

  await test("POST sin nombre → 400 {message}", async () => {
    const r = await req("POST", base, { body: { precio: 10, categoria: "X", descripcion: "sin nombre" } });
    assertEq(r.status, 400, "status");
    assert(r.json && typeof r.json.message === "string", "debe devolver {message}");
  });
}

async function main() {
  log(`\n=== ${CONFIG.nombre} · test-runner (Node nativo) ===\n`);
  let apiProc = null;
  let composeTouched = false;
  const composeFiles = [];
  if (existsSync(path.join(ROOT, "docker-compose.yml"))) composeFiles.push("docker-compose.yml");
  if (CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))) composeFiles.push(CONFIG.fallbackComposeFile);
  try {
    if (composeFiles.length && CONFIG.dbServices.length && dockerAvailable()) {
      log(`▶ Levantando infraestructura: ${CONFIG.dbServices.join(", ")}`);
      run(`docker compose up -d ${CONFIG.dbServices.join(" ")}`);
      composeTouched = true;
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
      composeTouched = true;
      listo = await waitForApi(20000);
    }
    if (!listo) throw new Error("La API no respondió en 20s" + (composeFiles.length ? "" : " (sin fallback docker disponible)"));
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

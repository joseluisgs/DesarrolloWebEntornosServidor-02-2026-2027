import { spawn, spawnSync } from "node:child_process";
import { existsSync, mkdirSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "04-ProductosControllersApiDI",
  // Rutas relativas a ROOT
  csproj: "ProductosControllersApiDI/ProductosControllersApiDI.csproj",
  baseUrl: "http://localhost:5000",
  // Orden de polling: cualquier respuesta HTTP (200,404...) = listo
  healthPaths: ["/swagger", "/", "/api/productos"],
  env: {
    ASPNETCORE_ENVIRONMENT: "Development",
    ASPNETCORE_URLS: "http://localhost:5000",
  },
  // Sin compose en 04 → no se toca docker en fase de infra
  dbServices: [],
  // Fallback: fichero compose completo si existe (null = docker-compose.yml)
  fallbackComposeFile: null,
  // Seed en memoria (5 productos ids 1-5) → no hay prepareDb
  prepareDb: [],
};

// ==== helpers ====
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
let exitCode = 0;

function log(msg) { console.log(msg); }
function logOk(msg) { console.log(`  ✅ ${msg}`); }
function logErr(msg) { console.error(`  ❌ ${msg}`); }

function run(cmd, args = [], opts = {}) {
  const res = spawnSync(cmd, args, { cwd: ROOT, encoding: "utf8", shell: true, ...opts });
  if (res.status !== 0) {
    const out = `${res.stdout || ""}\n${res.stderr || ""}`.trim();
    throw new Error(`Fallo al ejecutar: ${cmd} ${args.join(" ")}\n${out.slice(0, 2000)}`);
  }
  return res;
}

function dockerAvailable() {
  try {
    const r = spawnSync("docker", ["compose", "version"], { encoding: "utf8", shell: true });
    return r.status === 0;
  } catch { return false; }
}

// Matar árbol de procesos en Windows (dotnet run puede dejar hijos)
function killTree(pid) {
  if (!pid) return;
  try { spawnSync("taskkill", ["/pid", String(pid), "/T", "/F"], { shell: true, encoding: "utf8" }); }
  catch { try { process.kill(pid); } catch {} }
}

function spawnApi() {
  const child = spawn(
    "dotnet",
    ["run", "--project", CONFIG.csproj],
    {
      cwd: ROOT,
      env: { ...process.env, ...CONFIG.env },
      stdio: ["ignore", "pipe", "pipe"],
      shell: true,
    }
  );
  child.stdout.on("data", (d) => process.stdout.write(`   [api] ${d}`));
  child.stderr.on("data", (d) => process.stderr.write(`   [api] ${d}`));
  child.on("error", (e) => logErr(`Error spawn dotnet: ${e.message}`));
  return child;
}

async function fetchAny(url, opts = {}) {
  // Devuelve la respuesta aunque sea 404; null si no hay conexión
  try {
    const ctrl = new AbortController();
    const t = setTimeout(() => ctrl.abort(), 3000);
    const res = await fetch(url, { ...opts, signal: ctrl.signal });
    clearTimeout(t);
    return res;
  } catch { return null; }
}

async function waitForApi(timeoutMs = 20000) {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    for (const p of CONFIG.healthPaths) {
      const res = await fetchAny(CONFIG.baseUrl + p);
      if (res) return true; // cualquier código HTTP = listo
    }
    await sleep(500);
  }
  return false;
}

// Wrapper de test: valida status y opcionalmente body
function expectStatus(res, expected, label) {
  const ok = Array.isArray(expected) ? expected.includes(res.status) : res.status === expected;
  if (!ok) throw new Error(`${label}: status ${res.status}, esperado ${expected}`);
}

// ==== suite de tests ====
async function req(method, urlPath, { body, headers = {}, token } = {}) {
  const h = { "Content-Type": "application/json", ...headers };
  if (token) h["Authorization"] = `Bearer ${token}`;
  const res = await fetch(CONFIG.baseUrl + urlPath, {
    method,
    headers: h,
    body: body !== undefined ? (typeof body === "string" ? body : JSON.stringify(body)) : undefined,
  });
  let text = "";
  try { text = await res.text(); } catch {}
  let json = null;
  try { json = text ? JSON.parse(text) : null; } catch {}
  return { res, status: res.status, text, json, headers: res.headers };
}

const resultados = [];
async function test(nombre, fn) {
  try {
    await fn();
    resultados.push({ nombre, ok: true });
    logOk(nombre);
  } catch (e) {
    resultados.push({ nombre, ok: false, error: e.message });
    logErr(`${nombre} → ${e.message}`);
    exitCode = 1;
  }
}
function assert(cond, msg) { if (!cond) throw new Error(msg); }
function assertEq(actual, esperado, msg) {
  if (actual !== esperado) throw new Error(`${msg}: obtenido ${JSON.stringify(actual)}, esperado ${JSON.stringify(esperado)}`);
}

// Suite de 04: seed 1-5, rutas propias (search/filter/order/group/stats, PATCH /{id}/precio)
async function runSuite() {
  let idCreado = 0;

  await test("GET /api/productos → 200 con al menos 5 items (seed)", async () => {
    const r = await req("GET", "/api/productos");
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "el body debe ser un array");
    assert(r.json.length >= 5, `longitud ${r.json.length}, esperado >= 5`);
  });

  await test("GET /api/productos/1 → 200 nombre Portátil", async () => {
    const r = await req("GET", "/api/productos/1");
    assertEq(r.status, 200, "status");
    assertEq(r.json?.nombre, "Portátil", "nombre del producto seed");
    assertEq(r.json?.id, 1, "id");
  });

  await test("GET /api/productos/999 → 404", async () => {
    const r = await req("GET", "/api/productos/999");
    assertEq(r.status, 404, "status");
  });

  await test("POST /api/productos → 201 con id >= 6", async () => {
    const r = await req("POST", "/api/productos", {
      body: { nombre: "Producto Nuevo 04", precio: 49.99, categoria: "Test" },
    });
    assertEq(r.status, 201, "status");
    assert(r.json && typeof r.json.id === "number", "body con id numérico");
    idCreado = r.json.id;
    assert(idCreado >= 6, `id ${idCreado}, esperado >= 6 (próximo tras el seed)`);
  });

  await test("PATCH /{id}/precio con número crudo 123.45 → 200", async () => {
    // El body va como número JSON crudo, no como objeto { precio: ... }
    const r = await req("PATCH", `/api/productos/${idCreado}/precio`, { body: 123.45 });
    assertEq(r.status, 200, "status");
    assertEq(r.json?.precio, 123.45, "precio tras PATCH");
  });

  await test("GET /search sin termino → 400", async () => {
    const r = await req("GET", "/api/productos/search");
    assertEq(r.status, 400, "status");
  });

  await test("GET /filter/categoria sin parámetro → 400", async () => {
    const r = await req("GET", "/api/productos/filter/categoria");
    assertEq(r.status, 400, "status");
  });

  await test("GET /stats → 200 con payload", async () => {
    const r = await req("GET", "/api/productos/stats");
    assertEq(r.status, 200, "status");
    assert(typeof r.json?.totalProductos === "number", "campo totalProductos");
    assert(typeof r.json?.precioMedio === "number", "campo precioMedio");
    assert(typeof r.json?.precioMinimo === "number", "campo precioMinimo");
    assert(typeof r.json?.precioMaximo === "number", "campo precioMaximo");
    assert(r.json?.categorias !== undefined, "campo categorias");
  });

  await test("GET /group/categoria → 200 array de grupos", async () => {
    const r = await req("GET", "/api/productos/group/categoria");
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "el body debe ser un array");
    if (r.json.length > 0) {
      assert(typeof r.json[0].categoria === "string", "grupo con campo categoria");
      assert(Array.isArray(r.json[0].productos), "grupo con campo productos (array)");
    }
  });
}

// ==== main ====
async function main() {
  log(`\n=== ${CONFIG.nombre} · test-runner (Node nativo) ===\n`);

  let apiProc = null;
  let composeUp = false;
  const composeFiles = [];
  if (existsSync(path.join(ROOT, "docker-compose.yml"))) composeFiles.push("docker-compose.yml");
  if (CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))) composeFiles.push(CONFIG.fallbackComposeFile);

  try {
    // a) Infraestructura: SOLO servicios BD si hay compose
    if (composeFiles.length && CONFIG.dbServices.length && dockerAvailable()) {
      log(`▶ Levantando infraestructura: ${CONFIG.dbServices.join(", ")}`);
      run(`docker compose up -d ${CONFIG.dbServices.join(" ")}`);
      composeUp = true;
      for (const c of CONFIG.prepareDb) { log(`▶ Prepare: ${c}`); run(c); }
      await sleep(2000);
    }

    // b) CLI: restore + build + run
    log("▶ dotnet restore");
    run(`dotnet restore "${CONFIG.csproj}"`);
    log("▶ dotnet build");
    run(`dotnet build "${CONFIG.csproj}" -c Debug --no-restore`);
    log("▶ dotnet run (segundo plano)");
    apiProc = spawnApi();

    let listo = await waitForApi(20000);

    // c) Fallback Docker si CLI falla
    if (!listo && composeFiles.length && dockerAvailable()) {
      logWarnFallback();
      killTree(apiProc?.pid); apiProc = null;
      const file = CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))
        ? CONFIG.fallbackComposeFile : "docker-compose.yml";
      log(`▶ Fallback: docker compose -f ${file} up -d --build`);
      run(`docker compose -f ${file} up -d --build`);
      composeUp = true;
      listo = await waitForApi(20000);
    }

    if (!listo) {
      if (!composeFiles.length || !dockerAvailable()) {
        logErr("Sin fallback docker disponible");
        throw new Error("La API no respondió en 20s · sin fallback docker disponible");
      }
      throw new Error("La API no respondió en 20s");
    }

    // e) Suite de tests
    log("\n▶ Ejecutando suite de tests HTTP...\n");
    await runSuite();

  } catch (e) {
    logErr(`Error fatal: ${e.message}`);
    exitCode = 1;
  } finally {
    // f) Limpieza garantizada
    log("\n▶ Limpieza...");
    if (apiProc) killTree(apiProc.pid);
    if (composeUp || composeFiles.length) {
      for (const f of new Set([...composeFiles].reverse())) {
        try { run(`docker compose -f ${f} down -v`); } catch (e) { logErr(`down ${f}: ${e.message}`); }
      }
    }
  }

  // g) Resumen y salida
  const pass = resultados.filter(r => r.ok).length;
  const fail = resultados.filter(r => !r.ok).length;
  log("\n========== RESUMEN ==========");
  log(`Total: ${resultados.length} · OK: ${pass} · KO: ${fail}`);
  for (const r of resultados.filter(r => !r.ok)) {
    log(`  ✗ ${r.nombre}: ${r.error}`);
  }
  log("==============================\n");
  process.exit(exitCode);
}

function logWarnFallback() {
  console.warn("  ⚠️ Arranque CLI fallido/no disponible → fallback docker compose up -d --build");
}

main();

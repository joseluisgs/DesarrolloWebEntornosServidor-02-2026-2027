import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "12-ProductosMongo",
  csproj: "ProductosMongo/ProductosMongo.csproj",
  baseUrl: "http://localhost:5000",
  healthPaths: ["/swagger", "/api/productos", "/"],
  env: { ASPNETCORE_ENVIRONMENT: "Development", ASPNETCORE_URLS: "http://localhost:5000" },
  dbServices: ["db"], // mongo
  fallbackComposeFile: "docker-compose.api.yml", // stack completo (api + db)
  // El seed genera ObjectIds nuevos en cada arranque: los ids se toman de GET
  prepareDb: [],
};

// ID fijo del proveedor sembrado por MongoDbSeeder (colección 'proveedores').
// Producto lo referencia por este ID; sin él el service devuelve 400.
const PROVEEDOR_ID = "64b000000000000000000001";

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
  let primerId = null;
  let primerNombre = null;
  let creadoId = null;

  await test("GET lista → 200 con length >= 10 (seed Mongo)", async () => {
    const r = await req("GET", base);
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
    assert(r.json.length >= 10, `esperaba >= 10, obtenido ${r.json.length}`);
    primerId = r.json[0]?.id ?? r.json[0]?._id;
    primerNombre = r.json[0]?.nombre;
    assert(primerId, "el primer producto no tiene id");
    assert(primerNombre, "el primer producto no tiene nombre");
    // Los ObjectIds se regeneran en cada arranque: siempre usar ids reales de GET
    assert(String(primerId).length === 24, `id no parece ObjectId: ${primerId}`);
  });

  await test(`GET /{id} del primer producto → 200`, async () => {
    const r = await req("GET", `${base}/${primerId}`);
    assertEq(r.status, 200, "status");
  });

  await test("POST producto nuevo → 201 y guarda id", async () => {
    const r = await req("POST", base, {
      body: {
        nombre: "Producto Nuevo Mongo",
        precio: 42.5,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
        proveedorId: PROVEEDOR_ID,
      },
    });
    assertEq(r.status, 201, "status");
    creadoId = r.json?.id ?? r.json?._id;
    assert(creadoId, "no se ha devuelto id");
  });

  await test("PUT con id correcto (body.id = URL) → 200", async () => {
    const r = await req("PUT", `${base}/${creadoId}`, {
      body: {
        id: creadoId,
        nombre: "Producto Nuevo Mongo (act.)",
        precio: 55.5,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
        proveedorId: PROVEEDOR_ID,
      },
    });
    assertEq(r.status, 200, "status");
  });

  await test("PUT con id del body distinto del URL → 400", async () => {
    const r = await req("PUT", `${base}/${creadoId}`, {
      body: {
        id: "000000000000000000000000",
        nombre: "No coincide",
        precio: 10,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
        proveedorId: PROVEEDOR_ID,
      },
    });
    assertEq(r.status, 400, "status");
  });

  await test("POST con precio 0 → 400 (service)", async () => {
    const r = await req("POST", base, {
      body: {
        nombre: "Gratis",
        precio: 0,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
        proveedorId: PROVEEDOR_ID,
      },
    });
    assertEq(r.status, 400, "status");
  });

  await test("POST sin proveedorId → 400 (referencia obligatoria)", async () => {
    const r = await req("POST", base, {
      body: {
        nombre: "Sin Proveedor",
        precio: 10,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
      },
    });
    assertEq(r.status, 400, "status");
  });

  await test(`POST nombre duplicado del primero → 409`, async () => {
    const r = await req("POST", base, {
      body: {
        nombre: primerNombre,
        precio: 11.11,
        imagen: null,
        categoria: { nombre: "Test", descripcion: "Categoría de prueba" },
        proveedorId: PROVEEDOR_ID,
      },
    });
    assertEq(r.status, 409, "status");
  });

  await test("DELETE id creado → 204", async () => {
    const r = await req("DELETE", `${base}/${creadoId}`);
    assertEq(r.status, 204, "status");
  });

  await test("GET lista: el creado ya no está o aparece en /deleted", async () => {
    const lista = await req("GET", base);
    assertEq(lista.status, 200, "status lista");
    const enLista = Array.isArray(lista.json)
      && lista.json.some((p) => String(p.id ?? p._id) === String(creadoId));
    if (enLista) {
      const del = await req("GET", `${base}/deleted`);
      assertEq(del.status, 200, "status /deleted");
      const enDeleted = Array.isArray(del.json)
        && del.json.some((p) => String(p.id ?? p._id) === String(creadoId));
      assert(enDeleted, "el id está en la lista pero no en /deleted");
    } else {
      assert(!enLista, "el producto borrado sigue en la lista");
    }
  });

  await test("GET /filter → 200", async () => {
    const r = await req("GET", `${base}/filter?nombre=&precioMin=&precioMax=`);
    assertEq(r.status, 200, "status");
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

import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "17-ProductosGraphQL",
  csproj: "ProductosGraphQL/ProductosGraphQL.csproj",
  baseUrl: "http://localhost:5110",
  healthPaths: ["/graphql", "/"],
  env: { ASPNETCORE_ENVIRONMENT: "Development", ASPNETCORE_URLS: "http://localhost:5110" },
  dbServices: ["db", "redis"],
  fallbackComposeFile: "docker-compose.api.yml",
  // Respaldo para volúmenes creados antes de que el compose creara "productos_graphql"
  prepareDb: [
    `docker compose exec -T db psql -U admin -d postgres -c "CREATE DATABASE productos_graphql;"`,
  ],
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
// Como CREATE DATABASE falla si la BD ya existe, toleramos ese error
function tryRun(cmd) {
  try { return run(cmd); }
  catch (e) { log(`  ⚠️ Prepare tolerado (¿ya existe?): ${e.message.split("\n")[0]}`); return null; }
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

async function gql(query, variables) {
  return req("POST", "/graphql", { body: { query, ...(variables ? { variables } : {}) } });
}

async function runSuite() {
  // ── IDE / health ──────────────────────────────────────────────────────────
  await test("GET /graphql (IDE) → 200", async () => {
    const r = await req("GET", "/graphql");
    assertEq(r.status, 200, "status");
  });

  // ── Queries anónimas ──────────────────────────────────────────────────────
  await test("query categorias → 200 con data.categorias.nodes ≥ 5", async () => {
    const r = await gql(`{ categorias { nodes { id nombre } } }`);
    assertEq(r.status, 200, "status HTTP");
    assert(!r.json?.errors || r.json.errors.length === 0, `errores GraphQL: ${JSON.stringify(r.json?.errors)}`);
    const nodes = r.json?.data?.categorias?.nodes;
    assert(Array.isArray(nodes), "data.categorias.nodes debe ser un array");
    assert(nodes.length >= 5, `se esperaban ≥5 categorías, hay ${nodes.length}`);
  });

  await test("query productos → 200 con nodes ≥ 10 (seed)", async () => {
    const r = await gql(`{ productos { nodes { id nombre precio } } }`);
    assertEq(r.status, 200, "status HTTP");
    assert(!r.json?.errors || r.json.errors.length === 0, `errores GraphQL: ${JSON.stringify(r.json?.errors)}`);
    const nodes = r.json?.data?.productos?.nodes;
    assert(Array.isArray(nodes), "data.productos.nodes debe ser un array");
    assert(nodes.length >= 10, `se esperaban ≥10 productos, hay ${nodes.length}`);
  });

  await test("query producto(id: 1) → 200 con nombre", async () => {
    const r = await gql(`{ producto(id: 1) { id nombre } }`);
    assertEq(r.status, 200, "status HTTP");
    assert(!r.json?.errors || r.json.errors.length === 0, `errores GraphQL: ${JSON.stringify(r.json?.errors)}`);
    assert(r.json?.data?.producto, "data.producto no debe ser null");
    assertEq(Number(r.json.data.producto.id), 1, "id");
    assert(typeof r.json.data.producto.nombre === "string", "nombre");
  });

  await test("query productos con filtro (precio gt 50) → 200", async () => {
    const r = await gql(`{ productos(where: { precio: { gt: 50 } }) { nodes { id precio } } }`);
    assertEq(r.status, 200, "status HTTP");
    assert(!r.json?.errors || r.json.errors.length === 0, `errores GraphQL: ${JSON.stringify(r.json?.errors)}`);
    const nodes = r.json?.data?.productos?.nodes;
    assert(Array.isArray(nodes), "data.productos.nodes debe ser un array");
    for (const n of nodes) assert(Number(n.precio) > 50, `precio ${n.precio} no cumple el filtro`);
  });

  // ── Mutación sin token → 200 HTTP pero errores GraphQL de auth ────────────
  await test("mutation crearProducto sin token → 200 con body.errors (auth)", async () => {
    const r = await gql(
      `mutation { crearProducto(input: { nombre: "Mutation Sin Token", precio: 10.5, categoriaId: 1 }) { id } }`
    );
    assertEq(r.status, 200, "status HTTP (GraphQL siempre 200 salvo fallo de red)");
    const tieneErrores = Array.isArray(r.json?.errors) && r.json.errors.length > 0;
    const noData = !r.json?.data?.crearProducto;
    assert(tieneErrores, `se esperaban errors de autorización, body: ${r.text.slice(0, 300)}`);
    assert(noData, "data.crearProducto no debe existir sin token");
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
      for (const c of CONFIG.prepareDb) { log(`▶ Prepare: ${c}`); tryRun(c); }
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

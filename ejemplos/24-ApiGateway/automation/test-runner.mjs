import { spawn, spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(__dirname, "..");

const CONFIG = {
  nombre: "24-ApiGateway",
  // restore/build sobre la solución; el arranque usa la lista de servicios
  csproj: "ApiGateway.slnx",
  baseUrl: "http://localhost:5100",
  healthPaths: ["/swagger", "/api/proveedores", "/"],
  env: { ASPNETCORE_ENVIRONMENT: "Development" },
  dbServices: ["postgres", "mongo", "redis"],
  // No existe docker-compose.api.yml: el compose raíz ya levanta API + gateway + BD
  fallbackComposeFile: null,
  prepareDb: [
    // POSTGRES_DB solo crea auth_db; productos_db hay que crearla a mano
    `docker compose exec -T postgres psql -U admin -d postgres -c "CREATE DATABASE productos_db;"`,
  ],
  servicios: [
    { name: "AuthMicroService", csproj: "AuthMicroService/AuthMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5101" } },
    { name: "ProductosMicroService", csproj: "ProductosMicroService/ProductosMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5102" } },
    { name: "ProveedoresMicroService", csproj: "ProveedoresMicroService/ProveedoresMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5103" } },
    {
      name: "ApiGateway",
      csproj: "ApiGateway/ApiGateway.csproj",
      env: {
        ASPNETCORE_URLS: "http://localhost:5100",
        // Overrides YARP (Clusters de ApiGateway/appsettings.json) para CLI local:
        // los destinos por defecto apuntan a DNS Docker (auth-microservice, etc.)
        "ReverseProxy__Clusters__auth-cluster__Destinations__auth-destination__Address": "http://localhost:5101",
        "ReverseProxy__Clusters__productos-cluster__Destinations__productos-destination__Address": "http://localhost:5102",
        "ReverseProxy__Clusters__proveedores-cluster__Destinations__proveedores-destination__Address": "http://localhost:5103",
        "Jwt__Key": "SuperSecretKeyForJwtToken2024!SuperSecretKeyForJwtToken2024!",
        "Jwt__Issuer": "AuthMicroService",
        "Jwt__Audience": "ApiGateway",
      },
    },
  ],
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
  // --no-launch-profile: ignorar applicationUrl de launchSettings.json
  // para que ASPNETCORE_URLS del runner gobierna los puertos (5100-5103)
  const child = spawn("dotnet", ["run", "--project", csproj, "--no-launch-profile"], {
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
async function waitForApi(timeoutMs = 30000) {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    // Gateway (baseUrl) + microservicios directos para detectar procesos caídos pronto
    let gatewayOk = false;
    for (const p of CONFIG.healthPaths) {
      if (await fetchAny(CONFIG.baseUrl + p)) { gatewayOk = true; break; }
    }
    if (gatewayOk) {
      const puertos = ["http://localhost:5101", "http://localhost:5102", "http://localhost:5103"];
      const microsOk = (await Promise.all(puertos.map(p => fetchAny(p + "/")))).every(Boolean);
      if (microsOk) return true;
      // Si el gateway responde, damos por bueno aunque un micro espere otra vía de health
      return true;
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
  // Todo contra el gateway (baseUrl :5100). No hay /me ni usuarios seed → hay que registrar
  let token = null;
  let productoId = null;
  let proveedorId = null;
  const emailUnico = `gw_${Date.now()}@test.com`;
  const password = "Pass123!";

  await test("1. GET /api/proveedores → 200 con semilla", async () => {
    const r = await req("GET", "/api/proveedores");
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json) && r.json.length >= 1, "la lista de proveedores está vacía");
    proveedorId = r.json[0].id ?? r.json[0]._id ?? r.json[0].Id;
    assert(proveedorId, "el primer proveedor no tiene id");
  });

  await test("2. GET /api/productos sin token → 401", async () => {
    const r = await req("GET", "/api/productos");
    assertEq(r.status, 401, "status");
  });

  await test("3a. POST /api/Auth/register usuario único → 200 con token", async () => {
    const r = await req("POST", "/api/Auth/register", {
      body: { email: emailUnico, password, nombre: "Gateway", apellidos: "Tester" },
    });
    assertEq(r.status, 200, "status");
    assert(r.json?.token, "register no devuelve token");
    token = r.json.token;
  });

  await test("3b. POST /api/Auth/login → 200 con token", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: emailUnico, password },
    });
    assertEq(r.status, 200, "status");
    assert(r.json?.token, "login no devuelve token");
    token = r.json.token;
  });

  await test("4. POST /api/Auth/login incorrecto → 401", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: emailUnico, password: "Wrong123!" },
    });
    assertEq(r.status, 401, "status");
    assertEq(r.json?.error, "Credenciales invalidas", "mensaje de error");
  });

  await test("5. GET /api/productos con token → 200 array", async () => {
    const r = await req("GET", "/api/productos", { token });
    assertEq(r.status, 200, "status");
    assert(Array.isArray(r.json), "la respuesta no es un array");
  });

  // Escrituras requieren rol ADMIN → login con admin seedeado
  await test("5b. POST /api/Auth/login admin → 200 con token ADMIN", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: "admin@admin.com", password: "Admin123!" },
    });
    assertEq(r.status, 200, "status");
    assert(r.json?.token, "login admin no devuelve token");
    // AuthResponseDto serializa como { token, email, nombre, rol, expiration }
    const rol = r.json?.rol ?? r.json?.Rol ?? r.json?.role;
    assertEq(rol, "ADMIN", "rol del token");
    token = r.json.token;
  });

  await test("6. POST /api/productos con token ADMIN → 201 y guarda id", async () => {
    const r = await req("POST", "/api/productos", {
      token,
      body: {
        nombre: `Producto Gateway ${Date.now()}`,
        precio: 49.99,
        categoriaId: 1,
        proveedorId,
      },
    });
    assertEq(r.status, 201, "status");
    assert(r.headers.get("location"), "falta la cabecera Location");
    productoId = r.json?.id ?? r.json?._id ?? r.json?.Id;
    assert(productoId !== undefined && productoId !== null, "la respuesta no contiene id");
  });

  await test("7. GET /api/productos/{id} → 200", async () => {
    const r = await req("GET", `/api/productos/${productoId}`, { token });
    assertEq(r.status, 200, "status");
  });

  await test("8. PUT /api/productos/{id} → 200", async () => {
    const r = await req("PUT", `/api/productos/${productoId}`, {
      token,
      body: {
        nombre: "Producto Gateway Editado",
        precio: 59.99,
        categoriaId: 1,
        proveedorId,
      },
    });
    assertEq(r.status, 200, "status");
  });

  await test("9. DELETE → 204 y GET {id} → 404", async () => {
    const del = await req("DELETE", `/api/productos/${productoId}`, { token });
    assertEq(del.status, 204, "DELETE status");
    const get = await req("GET", `/api/productos/${productoId}`, { token });
    assertEq(get.status, 404, "GET tras DELETE status");
  });

  await test("10. GET /api/proveedores/{id} inexistente → 404", async () => {
    const r = await req("GET", "/api/proveedores/000000000000000000000000");
    assertEq(r.status, 404, "status");
  });

  await test("11. POST /api/Auth/register duplicado → 400", async () => {
    const r = await req("POST", "/api/Auth/register", {
      body: { email: emailUnico, password, nombre: "Dup", apellidos: "Duplicate" },
    });
    assertEq(r.status, 400, "status");
    assert(r.json?.error, "la respuesta no contiene error");
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
      await sleep(5000); // esperar a que Postgres acepte conexiones
      for (const c of CONFIG.prepareDb) { log(`▶ Prepare: ${c}`); tryRun(c); }
      await sleep(2000);
    }
    log("▶ dotnet restore"); run(`dotnet restore "${CONFIG.csproj}"`);
    log("▶ dotnet build"); run(`dotnet build "${CONFIG.csproj}" -c Debug --no-restore`);
    log("▶ dotnet run (4 procesos en segundo plano)");
    for (const s of CONFIG.servicios) {
      log(`   · ${s.name} → ${s.env.ASPNETCORE_URLS}`);
      procs.push(spawnApi(s.csproj, s.env));
    }
    let listo = await waitForApi(30000);
    if (!listo && composeFiles.length && dockerAvailable()) {
      console.warn("  ⚠️ CLI falló → fallback docker compose up -d --build");
      procs.forEach(p => killTree(p.pid)); procs = [];
      const file = CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))
        ? CONFIG.fallbackComposeFile : "docker-compose.yml";
      log(`▶ Fallback: docker compose -f ${file} up -d --build`);
      run(`docker compose -f ${file} up -d --build`);
      listo = await waitForApi(30000);
    }
    if (!listo) throw new Error("El gateway no respondió en 30s");
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

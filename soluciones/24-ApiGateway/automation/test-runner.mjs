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
  // No existe docker-compose.api.yml: el compose raíz ya levanta API + gateway + BD.
  // productos_db la crea postgres-init.sql en volúmenes nuevos; la comprobación de
  // abajo cubre volúmenes antiguos, y ambas rutas son idempotentes.
  fallbackComposeFile: null,
  // Orden importa: Proveedores siempra antes que Productos, porque Productos le
  // pide un proveedor real para sembrar sus productos de ejemplo.
  servicios: [
    { name: "ProveedoresMicroService", csproj: "ProveedoresMicroService/ProveedoresMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5103" } },
    { name: "AuthMicroService", csproj: "AuthMicroService/AuthMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5101" } },
    { name: "ProductosMicroService", csproj: "ProductosMicroService/ProductosMicroService.csproj", env: { ASPNETCORE_URLS: "http://localhost:5102" } },
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
  // String + shell:true (no array): el array con shell:true dispara DEP0190 en Node
  try { return spawnSync("docker compose version", { encoding: "utf8", shell: true }).status === 0; }
  catch { return false; }
}
function dockerOut(cmd) {
  const res = spawnSync(cmd, { cwd: ROOT, encoding: "utf8", shell: true });
  return { code: res.status ?? 1, out: `${res.stdout || ""}${res.stderr || ""}` };
}
/** Espera a que Postgres del compose acepte conexiones (no con sleeps a ciegas). */
async function esperarPostgres(timeoutMs = 60000) {
  const inicio = Date.now();
  while (Date.now() - inicio < timeoutMs) {
    const { code } = dockerOut(`docker compose exec -T postgres pg_isready -U admin -d auth_db`);
    if (code === 0) return true;
    await sleep(1000);
  }
  return false;
}
/**
 * Crea productos_db solo si no existe (idempotente).
 * postgres-init.sql ya la crea en volúmenes nuevos; esto cubre volúmenes antiguos.
 */
function crearBaseProductos() {
  const existe = dockerOut(
    `docker compose exec -T postgres psql -U admin -d postgres -tA -c "SELECT 1 FROM pg_database WHERE datname='productos_db';"`
  );
  if (existe.out.trim() === "1") return "productos_db ya existía";
  const crea = dockerOut(`docker compose exec -T postgres psql -U admin -d postgres -c "CREATE DATABASE productos_db;"`);
  if (crea.code !== 0) throw new Error(`No se pudo crear productos_db:\n${crea.out}`);
  return "productos_db creada";
}
function killTree(pid) {
  if (!pid) return;
  try { spawnSync(`taskkill /pid ${pid} /T /F`, { shell: true }); }
  catch { try { process.kill(pid); } catch { } }
}
function spawnApi(csproj, envExtra = {}) {
  // --no-launch-profile: ignorar applicationUrl de launchSettings.json
  // para que ASPNETCORE_URLS del runner gobierna los puertos (5100-5103).
  // String (no array) + shell:true: el array con shell:true dispara DEP0190 en Node.
  const cmd = `dotnet run --project "${csproj}" --no-launch-profile`;
  const child = spawn(cmd, {
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
async function waitForApi(timeoutMs = 45000) {
  const start = Date.now();
  const puertos = {
    "http://localhost:5101": "Auth",
    "http://localhost:5102": "Productos",
    "http://localhost:5103": "Proveedores",
  };
  while (Date.now() - start < timeoutMs) {
    // Exigimos los CUATRO servicios: si un microservicio ha muerto (BD caída,
    // puerto mal...) la suite no se lanza, porque saldrían tests falsos.
    let gatewayOk = false;
    for (const p of CONFIG.healthPaths) {
      if (await fetchAny(CONFIG.baseUrl + p)) { gatewayOk = true; break; }
    }
    if (gatewayOk) {
      const micros = await Promise.all(Object.keys(puertos).map(async p => {
        const res = await fetchAny(p + "/swagger/index.html");
        return !!res && res.ok;
      }));
      if (micros.every(Boolean)) return true;
      const caidos = Object.entries(puertos).filter((_, i) => !micros[i]).map(([, n]) => n);
      process.stdout.write(`   [espera] gateway OK · aún sin responder: ${caidos.join(", ")}\n`);
    }
    await sleep(1000);
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
  try {
    const detalle = await fn();
    resultados.push({ nombre, ok: true });
    logOk(nombre);
    // Cada test devuelve un resumen de lo que la API ha respondido de verdad
    if (detalle) log(`         ${detalle}`);
  }
  catch (e) { resultados.push({ nombre, ok: false, error: e.message }); logErr(`${nombre} → ${e.message}`); exitCode = 1; }
}
function assert(c, m) { if (!c) throw new Error(m); }
function assertEq(a, e, m) { if (a !== e) throw new Error(`${m}: obtenido ${JSON.stringify(a)}, esperado ${JSON.stringify(e)}`); }
/** Como assertEq, pero imprime el cuerpo de la respuesta: sin él un 400 es un misterio. */
function assertStatus(r, esperado) {
  if (r.status !== esperado) {
    const cuerpo = (r.text || "(sin cuerpo)").replace(/\s+/g, " ").slice(0, 500);
    throw new Error(`status: obtenido ${r.status}, esperado ${esperado} · cuerpo: ${cuerpo}`);
  }
}
/** Cuerpo de la respuesta en una sola línea, para el resumen de cada test. */
function cuerpo(r) {
  const t = (r.text || "").replace(/\s+/g, " ").trim();
  return t ? (t.length > 200 ? `${t.slice(0, 200)}…` : t) : "(cuerpo vacío)";
}

async function runSuite() {
  // Todo contra el gateway (baseUrl :5100). No hay /me ni usuarios seed → hay que registrar
  let token = null;
  let productoId = null;
  let proveedorId = null;
  const emailUnico = `gw_${Date.now()}@test.com`;
  const password = "Pass123!";

  await test("1. GET /api/proveedores → 200 con semilla", async () => {
    const r = await req("GET", "/api/proveedores");
    assertStatus(r, 200);
    assert(Array.isArray(r.json) && r.json.length >= 1, "la lista de proveedores está vacía");
    proveedorId = r.json[0].id ?? r.json[0]._id ?? r.json[0].Id;
    assert(proveedorId, "el primer proveedor no tiene id");
    return `HTTP 200 · ${r.json.length} proveedores sembrados · primero: ${r.json[0].nombre} (${proveedorId})`;
  });

  await test("2. GET /api/productos sin token → 401", async () => {
    const r = await req("GET", "/api/productos");
    assertStatus(r, 401);
    return `HTTP 401 · el gateway exige JWT en /api/productos · ${cuerpo(r)}`;
  });

  await test("3a. POST /api/Auth/register usuario único → 200 con token", async () => {
    const r = await req("POST", "/api/Auth/register", {
      body: { email: emailUnico, password, nombre: "Gateway", apellidos: "Tester" },
    });
    assertStatus(r, 200);
    assert(r.json?.token, "register no devuelve token");
    token = r.json.token;
    return `HTTP 200 · ${cuerpo(r)}`;
  });

  await test("3b. POST /api/Auth/login → 200 con token", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: emailUnico, password },
    });
    assertStatus(r, 200);
    assert(r.json?.token, "login no devuelve token");
    token = r.json.token;
    return `HTTP 200 · ${r.json.email} · token de ${r.json.token.length} caracteres`;
  });

  await test("4. POST /api/Auth/login incorrecto → 401", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: emailUnico, password: "Wrong123!" },
    });
    assertStatus(r, 401);
    assertEq(r.json?.error, "Credenciales invalidas", "mensaje de error");
    return `HTTP 401 · ${cuerpo(r)}`;
  });

  await test("5. GET /api/productos con token → 200 array con semilla", async () => {
    const r = await req("GET", "/api/productos", { token });
    assertStatus(r, 200);
    assert(Array.isArray(r.json), "la respuesta no es un array");
    // La semilla de productos de ejemplo debe haberse creado al arrancar
    assert(r.json.length >= 1, "no hay ningún producto de ejemplo: la semilla no se creó");
    const p = r.json[0];
    return `HTTP 200 · ${r.json.length} productos · [${p.id}] ${p.nombre} ${p.precio} € · ${p.categoriaNombre} · proveedor ${p.proveedorNombre}`;
  });

  // Escrituras requieren rol ADMIN → login con admin seedeado
  await test("5b. POST /api/Auth/login admin → 200 con token ADMIN", async () => {
    const r = await req("POST", "/api/Auth/login", {
      body: { email: "admin@admin.com", password: "Admin123!" },
    });
    assertStatus(r, 200);
    assert(r.json?.token, "login admin no devuelve token");
    // AuthResponseDto serializa como { token, email, nombre, rol, expiration }
    const rol = r.json?.rol ?? r.json?.Rol ?? r.json?.role;
    assertEq(rol, "ADMIN", "rol del token");
    token = r.json.token;
    return `HTTP 200 · ${r.json.email} · rol ${rol}`;
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
    assertStatus(r, 201);
    assert(r.headers.get("location"), "falta la cabecera Location");
    productoId = r.json?.id ?? r.json?._id ?? r.json?.Id;
    assert(productoId !== undefined && productoId !== null, "la respuesta no contiene id");
    // La validación remota Productos → Proveedores ha tenido que funcionar para llegar aquí
    return `HTTP 201 · Location: ${r.headers.get("location")} · ${cuerpo(r)}`;
  });

  await test("7. GET /api/productos/{id} → 200", async () => {
    const r = await req("GET", `/api/productos/${productoId}`, { token });
    assertStatus(r, 200);
    return `HTTP 200 · ${cuerpo(r)}`;
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
    assertStatus(r, 200);
    return `HTTP 200 · ${cuerpo(r)}`;
  });

  await test("9. DELETE → 204 y GET {id} → 404", async () => {
    const del = await req("DELETE", `/api/productos/${productoId}`, { token });
    assertStatus(del, 204);
    const get = await req("GET", `/api/productos/${productoId}`, { token });
    assertStatus(get, 404);
    return `DELETE HTTP 204 (sin cuerpo) · GET posterior HTTP 404 · ${cuerpo(get)}`;
  });

  await test("10. GET /api/proveedores/{id} inexistente → 404", async () => {
    const r = await req("GET", "/api/proveedores/000000000000000000000000");
    assertStatus(r, 404);
    return `HTTP 404 · ${cuerpo(r)}`;
  });

  await test("11. POST /api/Auth/register duplicado → 400", async () => {
    const r = await req("POST", "/api/Auth/register", {
      body: { email: emailUnico, password, nombre: "Dup", apellidos: "Duplicate" },
    });
    assertStatus(r, 400);
    assert(r.json?.error, "la respuesta no contiene error");
    return `HTTP 400 · ${cuerpo(r)}`;
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
      log("▶ Esperando a que Postgres acepte conexiones...");
      if (!(await esperarPostgres())) throw new Error("Postgres no respondió en 60s");
      log(`▶ Prepare: ${crearBaseProductos()}`);
    }
    log("▶ dotnet restore"); run(`dotnet restore "${CONFIG.csproj}"`);
    log("▶ dotnet build"); run(`dotnet build "${CONFIG.csproj}" -c Debug --no-restore`);
    log("▶ dotnet run (4 procesos en segundo plano)");
    for (const s of CONFIG.servicios) {
      log(`   · ${s.name} → ${s.env.ASPNETCORE_URLS}`);
      procs.push(spawnApi(s.csproj, s.env));
    }
    let listo = await waitForApi(45000);
    if (!listo && composeFiles.length && dockerAvailable()) {
      console.warn("  ⚠️ CLI falló → fallback docker compose up -d --build");
      procs.forEach(p => killTree(p.pid)); procs = [];
      const file = CONFIG.fallbackComposeFile && existsSync(path.join(ROOT, CONFIG.fallbackComposeFile))
        ? CONFIG.fallbackComposeFile : "docker-compose.yml";
      log(`▶ Fallback: docker compose -f ${file} up -d --build`);
      run(`docker compose -f ${file} up -d --build`);
      listo = await waitForApi(45000);
    }
    if (!listo) {
      throw new Error("No respondieron los 4 servicios en 45s (gateway + Auth + Productos + Proveedores). Revisa la consola [api]: si un microservicio dice 'FTL', esa es la causa real.");
    }
    log("\n▶ Ejecutando suite de tests HTTP...\n");
    await runSuite();
  } catch (e) {
    logErr(`Error fatal: ${e.message}`); exitCode = 1;
  } finally {
    log("\n▶ Limpieza: parando los 4 procesos dotnet...");
    procs.forEach(p => killTree(p.pid));
    // La infraestructura se DEJA LEVANTADA: así, terminada la suite, puedes seguir
    // trabajando en modo desarrollo desde Rider o la terminal con la BD ya llena.
    // Para reiniciar de cero: docker compose down -v
    if (composeFiles.length) {
      log("▶ Infraestructura (postgres, mongo, redis) se queda corriendo.");
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

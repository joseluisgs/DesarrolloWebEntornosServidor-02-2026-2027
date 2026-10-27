# 25 - Productos OAuth (Identity + JWT + login externo)

API REST de productos con **ASP.NET Core Identity**, **JWT** y **cuatro puertas de entrada** elegibles por configuración: la propia (email y contraseña) y las externas (Google, GitHub, X/Twitter o Microsoft), en ASP.NET Core (.NET 10).

Es la evolución del ejemplo 15-ProductosIdentity: la puerta propia se queda igual y se añaden las puertas externas con OAuth 2.0. **Todas las puertas externas vienen cerradas (`Enabled: false`)**: el ejemplo compila, arranca y se prueba sin ninguna cuenta ni credencial.

---

## Tutorial paso a paso

### Paso 0. Arrancar y probar la puerta propia

```bash
# 1. Base de datos (PostgreSQL + Redis)
docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000` (Swagger en `/swagger` si el entorno es Development). El arranque crea el esquema y siembra productos y usuarios.

Comprueba que la puerta propia funciona:

```bash
# Login local con el usuario de prueba
curl -X POST http://localhost:5000/api/Auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"admin@admin.com","password":"Admin123!"}'
# → 200 { "token": "eyJ...", "role": "ADMIN", ... }

# Usa el token en un endpoint protegido
curl http://localhost:5000/api/Auth/me -H "Authorization: Bearer <token>"
# → 200 perfil de admin@admin.com

# Y comprueba que una puerta externa está cerrada
curl -i http://localhost:5000/api/Auth/external/Google
# → 404 "La puerta 'Google' no está activa: revisa Auth:Google:Enabled"
```

### Paso 1. Crear la app en el proveedor

Los cuatro proveedores piden lo mismo: una app de desarrollador con la **misma URL de retorno**, que en este ejemplo es siempre:

```
http://localhost:5000/api/Auth/external-callback
```

#### Opción A: GitHub (el más rápido, 5 minutos)

1. Con tu cuenta de GitHub: **Settings → Developer settings → OAuth Apps → New OAuth App**.
2. Rellena: nombre (por ejemplo `ProductosOAuth`), Homepage `http://localhost:5000` y en **Authorization callback URL** la URL de retorno de arriba.
3. Pulsa **Register**. Copia el **Client ID** y pulsa **Generate a new client secret** para el **Client Secret** (solo se muestra una vez).
4. No pide verificación ni teléfono.

#### Opción B: Google (modo Testing)

1. En `console.cloud.google.com` crea un **proyecto nuevo** (uno de pruebas, no el de producción).
2. **APIs & Services → OAuth consent screen**: tipo **External**, modo **Testing**, y añade tus **usuarios de prueba** (el correo con el que vas a entrar).
3. **APIs & Services → Credentials → Create Credentials → OAuth client ID → Web application**: en **Authorized redirect URIs** pones la URL de retorno.
4. Copia **Client ID** y **Client Secret** (`GOCSPX-...`).
5. En modo Testing, el navegador avisa de que la app no está verificada: es normal en clase.

#### Opción C: X / Twitter

1. En `developer.x.com`, **Projects & Apps**: crea proyecto y app (el nivel Free basta).
2. **App settings → User authentication settings**: activa **OAuth 2.0**, tipo **Web App**, y registra la Callback URI (la misma URL) y como Website URL `http://localhost:5000`.
3. **OAuth 2.0 → Client ID and Client Secret**.
4. Es el más tedioso: pide verificación de teléfono y su consola cambia a menudo. El nombre del esquema de la app es `Twitter`.

#### Opción D: Microsoft

1. En `entra.microsoft.com`, **App registrations → New registration** (cuenta personal o escolar).
2. En **Redirect URI**, tipo **Web**, la misma URL de retorno.
3. **Overview → Application (client) ID** y **Certificates & secrets → New client secret** (copia el **valor**, no el identificador; se muestra una vez).
4. Aprovecha las cuentas escolares o Outlook que ya tienen los alumnos.

### Paso 2. Guardar los secretos y abrir la puerta

Los secretos **no van nunca en el `appsettings.json`** ni en el repo: van en el user-secrets de tu máquina.

```bash
cd ProductosTest

# Opción A (GitHub)
dotnet user-secrets set "GitHub:ClientId" "Iv1.xxxxxxxxxxxx"
dotnet user-secrets set "GitHub:ClientSecret" "xxxxxxxxxxxxxxxx"

# Opción B (Google)      -> "Google:ClientId" / "Google:ClientSecret"
# Opción C (X/Twitter)   -> "Twitter:ClientId" / "Twitter:ClientSecret"
# Opción D (Microsoft)   -> "Microsoft:ClientId" / "Microsoft:ClientSecret"
```

Y en `appsettings.json`, abre la puerta elegida:

```json
"Auth": {
  "GitHub": { "Enabled": true, "ClientId": "", "ClientSecret": "" }
}
```

Si activas una puerta sin guardar sus credenciales, la API no arranca: avisa con `InvalidOperationException` (fail-fast), que es lo que toca.

### Paso 3. Probar el flujo en el navegador

1. Abre `http://localhost:5000/api/Auth/external/GitHub` en el navegador (cambia `GitHub` por tu proveedor).
2. Tu servidor responde con una redirección (302) y el navegador se va al proveedor.
3. Autorizas en el proveedor (con tu cuenta).
4. El proveedor redirige a `external-callback` con un código; tu servidor lo canjea, crea o vincula la cuenta local en Identity y responde **con el JWT de esta API**.
5. Usa ese token igual que el del login local: `Authorization: Bearer <token>` contra `GET /api/Auth/me`.

Si no quieres navegador, recuerda que este flujo **es de navegador por diseño**: las idas y vueltas son redirecciones HTTP; lo único que no toca al navegador es el canje del código, que ocurre entre servidores.

### Errores típicos

| Síntoma | Causa |
|---------|-------|
| `404` "La puerta 'X' no está activa" | `Auth:X:Enabled` sigue en `false`, o el esquema es `Twitter` y no `X` |
| El proveedor dice `redirect_uri mismatch` | La URL registrada no coincide carácter a carácter (puerto, `http`/`https`, barra final) |
| `401` al volver del callback | Has llegado al callback tecleando la URL, sin pasar por el proveedor |
| Pantalla de app no verificada | Modo Testing de Google o aviso de X; en clase no hace falta verificar |
| `AddGoogle` no compila | En .NET 10 el proveedor es paquete NuGet: falta `dotnet add package Microsoft.AspNetCore.Authentication.Google` (los cuatro paquetes ya están en el `.csproj`) |

### Seguridad (regla de la casa)

- Arranca con todas las puertas cerradas: quien quiera probar una la abre con **credenciales propias**.
- Nunca repartas tu Client Secret en clase ni en el material: un secreto filtrado recae en tu cuota y en tus recursos.
- Los secretos viven en user-secrets o variables de entorno, jamás en el repo.

---

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra → restore/build/run → polling ≤20 s → **11 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

### Auth - `http://localhost:5000/api/Auth`

| Método | Ruta | Auth | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------|--------------------|-------------------|---------|
| POST | `/api/Auth/login` | no | `{ email, password }` | `200` `{ token, role, ... }` | `401` credenciales |
| POST | `/api/Auth/signup` | no | `{ email, password, nombre, apellidos }` | `200` `{ token, ... }` | `400`/`409` |
| GET | `/api/Auth/external/{provider}` | no | - | `302` al proveedor | `404` puerta cerrada |
| GET | `/api/Auth/external-callback` | no | - | `200` `{ token, ... }` | `401` sin info del proveedor |
| GET | `/api/Auth/me` | **Bearer** | - | `200` perfil + `role` | `401` sin token |

Proveedores válidos en `{provider}`: `Google`, `GitHub`, `Twitter`, `Microsoft`.

**Login admin:** `{ "email": "admin@admin.com", "password": "Admin123!" }` → `{ "token": ".", "role": "ADMIN", ... }`. Envío del token: cabecera `Authorization: Bearer <token>`.

### Productos - `/api/Productos`

| Método | Ruta | Auth / Roles | Cuerpo | Éxito | Errores |
|--------|------|--------------|--------|-------|---------|
| GET | `/api/Productos` | público | - | `200` (=10) | - |
| GET | `/api/Productos/{id}` | público | - | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId, imagen? }` | `201` | `401`, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` | `401`, `403`, `400`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | - | `204` | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | - | `200` `{message}` | `401`, `404` |
| GET | `/api/Productos/deleted` | público | - | `200` | - |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | - | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales | - | `200` | - |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | - | `200` `{data, pagination}` | - |
| GET | `/api/Productos/categorias` | público | - | `200` | - |
| GET | `/api/Productos/categorias/{id}` | público | - | `200` | `404` |

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

> 📝 **Nota:** Los dos compose son **proyectos distintos** (cada fichero lleva su `name:`), así que no se pisan: levantar el stack completo no destruye los contenedores locales, y al revés.
>
> - **Desarrollo** (`docker compose up -d`): PostgreSQL en `localhost:5432` y Redis en `localhost:6379`. Es lo que necesitas con el play de Rider.
> - **Stack completo** (`-f docker-compose.api.yml`): api en el `5000`, PostgreSQL en el `5433` y Redis en el `6380`, porque dentro de la red de Docker la app se conecta a los servicios `db` y `redis`, no a `localhost`.
>
> ⚠️ Los nombres de contenedor son propios de esta solución, pero los **puertos 5432 y 6379 son comunes**: no levantes dos soluciones a la vez o la segunda fallará con `port is already allocated`.

## Estructura

```
25-ProductosOAuth/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/  (Auth, Productos)
│   ├── Infrastructures/  (IdentityConfig, OAuthConfig, ...)
│   ├── Seed/IdentitySeeder.cs
│   └── ...
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- ASP.NET Core Identity + EF Core/PostgreSQL
- JWT bearer auth, `CSharpFunctionalExtensions`, Serilog, Swashbuckle
- `AspNet.Security.OAuth.GitHub`, `AspNet.Security.OAuth.Twitter`, `Microsoft.AspNetCore.Authentication.Google` y `Microsoft.AspNetCore.Authentication.MicrosoftAccount` (en .NET 10 los proveedores son paquetes NuGet, no vienen en el shared framework)

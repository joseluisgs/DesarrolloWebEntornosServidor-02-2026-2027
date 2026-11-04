# 17 - Productos GraphQL (HotChocolate)

API de productos con **GraphQL** (HotChocolate): Queries, Mutations, Subscriptions, Filtering, Sorting y Paging en ASP.NET Core (.NET 10).

## Descripción

Endpoint único `/graphql` (IDE **Banana Cake Pop** en Development). DataLoaders para el problema N+1; seed: ≥10 productos y ≥5 categorías en PostgreSQL.

- El compose crea la BD `productos_graphql` en el primer arranque (el runner solo la prepara si el volumen es de una versión anterior)
- Infra: PostgreSQL + Redis vía Docker

## Ejecución manual

```bash
# 1. Infra (crea la BD productos_graphql en el primer arranque)
docker compose up -d

# 2. API
dotnet run --project ProductosGraphQL
```

> 💡 **Consejo:** desde Rider el perfil `Properties/launchSettings.json` arranca en `Development` y en el puerto 5000, así que el play del IDE ejecuta exactamente estos mismos pasos.

- API: `http://localhost:5110`
- IDE GraphQL: `http://localhost:5110/graphql`

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra + `prepareDb` → restore/build/run → polling `/graphql` ≤20 s → **6 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoint GraphQL

| Método | Ruta | Cuerpo | Respuesta |
|--------|------|--------|-----------|
| GET | `/graphql` | — | IDE Banana Cake Pop `200` |
| POST | `/graphql` | `{ query, variables? }` | `{ data, errors? }` |

### Ejemplos de query

```graphql
# IDE / clientes
{ categorias { nodes { id nombre } } }

{ productos { nodes { id nombre precio } } }

{ producto(id: 1) { id nombre } }

{ productos(where: { precio: { gt: 50 } }) { nodes { id precio } } }
```

| Operación | Tipo | Notas |
|-----------|------|-------|
| `categorias` | Query | ≥5 nodos (seed) |
| `productos` | Query | ≥10 nodos; filtros `where`, orden, paging |
| `producto(id)` | Query | `null` si no existe |
| create/update/delete | Mutation | patrón Result en dominio |
| updates | Subscription | tiempo real |

> 📝 **Nota:** Los tests envían `POST /graphql` con `Content-Type: application/json`.

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

> 📝 **Nota:** Los dos compose son **proyectos distintos** (cada fichero lleva su `name:`), así que no se pisan: levantar el stack completo no destruye los contenedores locales, y al revés.
>
> - **Desarrollo** (`docker compose up -d`): PostgreSQL en `localhost:5440` con la BD `productos_graphql`, y Redis en `localhost:6390`. Es lo que necesitas con el play de Rider.
> - **Stack completo** (`-f docker-compose.api.yml`): api en el `5110`, PostgreSQL en el `5441` y Redis en el `6391`, porque dentro de la red de Docker la app se conecta a los servicios `db` y `redis`, no a `localhost`.
>
> ⚠️ **Advertencia:** Los nombres de contenedor son propios de esta solución, pero los **puertos 5440 y 6390 son comunes**: no levantes dos soluciones a la vez o la segunda fallará con `port is already allocated`.

## Estructura

```
17-ProductosGraphQL/
├── ProductosGraphQL.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosGraphQL/
│   ├── DataLoaders/
│   ├── GraphQL/
│   │   ├── Inputs/
│   │   ├── Resolvers/   ← Query/Mutation/Subscription
│   │   └── Types/
│   ├── Seed/ Controllers/ …
│   └── Program.cs
├── ProductosGraphQL.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- HotChocolate (GraphQL server)
- EF Core + PostgreSQL, Serilog, CSharpFunctionalExtensions

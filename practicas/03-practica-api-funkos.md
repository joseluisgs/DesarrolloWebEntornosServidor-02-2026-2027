# Práctica 3: Construcción de la API de Funkos

- [Práctica 3: Construcción de la API de Funkos](#práctica-3-construcción-de-la-api-de-funkos)
  - [Objetivo](#objetivo)
  - [Descripción](#descripción)
  - [Tareas a Realizar](#tareas-a-realizar)
  - [Tecnologías](#tecnologías)
  - [Estructura de Proyecto](#estructura-de-proyecto)
  - [Formato de Entrega](#formato-de-entrega)

---

## Objetivo

Construir, de principio a fin, una API REST completa de gestión de una colección de **Funkos** (figuras de vinilo coleccionables), aplicando de forma progresiva todos los contenidos de la unidad: diseño REST, ASP.NET Core, inyección de dependencias, persistencia, seguridad, documentación, testing y despliegue con Docker.

---

## Descripción

A lo largo de los temas de la unidad has ido proponiendo —en los **Retos** finales de cada tema— las piezas de un mismo proyecto: la API de Funkos. En esta práctica esas piezas se ensamblan en un único entregable evolutivo.

**Contexto:** una tienda online necesita gestionar su catálogo de Funkos: consultarlos, crearlos, modificarlos y eliminarlos, con autenticación, persistencia y despliegue.

Un Funko tiene, como mínimo, estas propiedades: `Id`, `Nombre`, `Precio`, `Stock`, `Categoria`, `IsDeleted`, `CreatedAt` y `UpdatedAt`.

> 💡 **Consejo:** Sigue las fases en orden. Cada fase parte de la anterior: en lugar de reescribir, refactorizas y evolucionas tu propio código.

---

## Tareas a Realizar

### 1. Define tu recurso (Tema 1)

- Enumera las propiedades de un Funko y justifica cada una.
- Identifica quién es tu cliente y tu servidor en este sistema.

### 2. Diseña la API en papel (Tema 2)

> ⚠️ **Advertencia:** No escribas código todavía. Primero el papel.

- Completa la tabla de endpoints: verbo HTTP, ruta, descripción y código de estado de respuesta.

| Operación | Verbo | Ruta | Éxito |
|-----------|-------|------|-------|
| Listar todos los Funkos | | | |
| Consultar un Funko por ID | | | |
| Crear un Funko | | | |
| Actualizar un Funko completo | | | |
| Eliminar un Funko | | | |
| Buscar Funkos por nombre | | | |

### 3. Tu primera API (Tema 3)

- Implementa el CRUD completo como **Minimal API** con los datos en memoria (`List<Funko>`).
- Verifica con las colecciones `.http` o con Swagger los seis endpoints del paso anterior.

### 4. La misma API con controladores (Tema 4)

- Reimplementa el CRUD con **controladores MVC** (`FunkosController`).
- Compara ambos enfoques y anota en el README cuál elegirías para un proyecto real y por qué.

### 5. Inyección de dependencias completa (Tema 6)

- Separa `IFunkoService`/`FunkoService` e `IFunkoRepository`/`FunkoRepository` (en memoria con `Dictionary<long, Funko>`).
- Registra todo en DI con ciclos de vida adecuados y constructor primario.

### 6. Errores con patrón Result (Tema 7)

- Gestiona los errores de dominio (Funko no encontrado, nombre duplicado, stock insuficiente) con `Result<T>`.
- Añade un manejador global de excepciones que devuelva Problem Details.

### 7. DTOs, validaciones y mapeadores (Tema 8)

- Crea los DTOs de entrada y salida (`CreateFunkoDto`, `UpdateFunkoDto`, `FunkoDto`).
- Valida con Data Annotations y un validador de dominio; mapea con métodos de extensión.

### 8. Configuración y logs (Tema 9)

- Mueve la configuración a `appsettings.json` + perfiles por entorno.
- Configura Serilog con salida a consola y fichero, con log estructurado de las operaciones CRUD.

### 9. Persistencia real (Tema 12)

- Sustituye la lista en memoria por **Entity Framework Core** con PostgreSQL (o SQLite en desarrollo).
- Relación Uno a Muchos con `Categoria`, borrado lógico con `IsDeleted` y migraciones.

### 10. Seguridad (Temas 16 y 17)

- Añade autenticación **JWT** y **BCrypt** para las contraseñas.
- Protege las operaciones de escritura: solo usuarios autenticados; el borrado, solo rol `Admin`.

### 11. Documentación y calidad (Temas 24 y 28)

- Documenta la API con **Swagger/OpenAPI** (comentarios XML incluidos).
- Escribe tests unitarios del servicio y tests de integración del endpoint con `WebApplicationFactory`.

### 12. Despliegue (Tema 29)

- Dockeriza la API con **Dockerfile multi-etapa** y levanta API + base de datos con `docker-compose`.

---

## Tecnologías

| Tecnología | Para qué | Paquete NuGet |
|------------|----------|---------------|
| **ASP.NET Core** | API REST (Minimal APIs o controladores) | — |
| **EF Core** | Persistencia con base de datos | `Microsoft.EntityFrameworkCore` |
| **Serilog** | Log estructurado | `Serilog.Sinks.File` |
| **JWT + BCrypt** | Autenticación y hash de contraseñas | `Microsoft.AspNetCore.Authentication.JwtBearer`, `BCrypt.Net-Next` |
| **NUnit + FluentAssertions** | Tests unitarios e de integración | `NUnit`, `FluentAssertions` |
| **Docker** | Contenedores de despliegue | — |
| **C# 14** | Primary constructors, top-level statements | — |

---

## Estructura de Proyecto

```
FunkoApp/
├── FunkoApp.slnx
├── FunkoApp/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Controllers/          (o endpoints Minimal API)
│   ├── Models/
│   ├── Entity/
│   ├── Dtos/
│   ├── Mappers/
│   ├── Repositories/
│   ├── Services/
│   ├── Validators/
│   ├── Middleware/
│   └── Infrastructures/
├── FunkoApp.Test/
│   ├── Services/
│   └── Integration/
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## Formato de Entrega

- Repositorio en **GitHub** con el nombre `FunkoApp`, con commits incrementales (uno por fase).
- **README** con: instrucciones de uso, cómo ejecutar los tests, cómo levantar la API con Docker y una sección de **justificación de decisiones** (enfoque Minimal API o controladores, elección de base de datos, estrategia de seguridad).
- La API debe arrancar con `docker compose up` y servir la documentación en `/swagger`.
- Los tests deben pasar en verde (`dotnet test`).

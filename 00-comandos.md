- [00. Guía de Supervivencia: Comandos .NET CLI](#00-guía-de-supervivencia-comandos-net-cli)
  - [1. Crear Proyectos y Soluciones](#1-crear-proyectos-y-soluciones)
  - [2. Scaffolding: Generar Código Automáticamente](#2-scaffolding-generar-código-automáticamente)
  - [3. Hot Reload: Desarrollo en Vivo](#3-hot-reload-desarrollo-en-vivo)
  - [4. Compilar y Ejecutar](#4-compilar-y-ejecutar)
  - [5. Paquetes NuGet](#5-paquetes-nuget)
  - [6. .NET Tools: Herramientas Globales](#6-net-tools-herramientas-globales)
  - [7. Entity Framework Core](#7-entity-framework-core)
  - [8. Tests](#8-tests)
  - [9. Formateo de Código](#9-formateo-de-código)
  - [10. User Secrets (Secretos de Desarrollo)](#10-user-secrets-secretos-de-desarrollo)
  - [11. Certificados de Desarrollo](#11-certificados-de-desarrollo)
  - [12. Docker](#12-docker)
  - [13. Git](#13-git)



# 00. Guía de Supervivencia: Comandos .NET CLI

> 💡 **Punto de partida:** Esta guía recopila todos los comandos de `dotnet` que necesitas para crear, compilar, ejecutar, scaffoldear, testear y desplegar APIs en .NET 10. Guárdala como referencia rápida.

## 1. Crear Proyectos y Soluciones

### Templates disponibles

```bash
# Ver todos los templates instalados
dotnet new list

# Buscar un template concreto
dotnet new list webapi
dotnet new list api
dotnet new list nunit

# Buscar en NuGet.org
dotnet new search webapi
```

### Crear proyecto

```bash
# Web API (el más usado en DAW)
dotnet new webapi -n MiApi -f net10.0

# Web API con Minimal APIs
dotnet new webapiaot -n MiApiAot -f net10.0

# API Controller vacío (sin scaffolding)
dotnet new apicontroller -n MiController

# MVC (Model-View-Controller)
dotnet new mvc -n MiAppMvc -f net10.0

# Razor Pages
dotnet new webapp -n MiAppRazor -f net10.0

# Class Library (biblioteca de clases)
dotnet new classlib -n MiLibreria -f net10.0

# Console App
dotnet new console -n MiConsola -f net10.0

# Worker Service (servicio en segundo plano)
dotnet new worker -n MiWorker -f net10.0

# Blazor Web App
dotnet new blazor -n MiBlazor -f net10.0

# Proyecto de test
dotnet new nunit -n MiProyecto.Test -f net10.0
dotnet new xunit -n MiProyecto.Test -f net10.0
dotnet new mstest -n MiProyecto.Test -f net10.0

# Proyecto vacío (sin nada)
dotnet new web -n MiApiVacia -f net10.0
```

### Crear solución

```bash
# Crear solución (.slnx en .NET 10+)
dotnet new sln -n MiSolucion

# Añadir proyectos a la solución
dotnet sln MiSolucion.slnx add MiApi/MiApi.csproj
dotnet sln MiSolucion.slnx add MiApi.Test/MiApi.Test.csproj

# Listar proyectos
dotnet sln MiSolucion.slnx list

# Eliminar proyecto
dotnet sln MiSolucion.slnx remove MiApi.Test/MiApi.Test.csproj

# Migrar de .sln a .slnx
dotnet sln migrate
```



## 2. Scaffolding: Generar Código Automáticamente

El scaffolding genera código boilerplate (controladores, vistas, etc.) a partir de tus modelos y DbContext.

### Instalar el generador

```bash
# Instalar globalmente (solo una vez)
dotnet tool install -g dotnet-aspnet-codegenerator

# Si ya está instalado, actualizar
dotnet tool update -g dotnet-aspnet-codegenerator

# Añadir paquete NuGet al proyecto
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

### Generar un Controller API completo (CRUD)

```bash
# API Controller con CRUD automático para un modelo
dotnet aspnet-codegenerator controller \
    -name ProductosController \
    -m MiApi.Models.Producto \
    -dc MiApi.Data.AppDbContext \
    -api \
    -async \
    -namespace MiApi.Controllers \
    -outDir Controllers

# MVC Controller con vistas (Create, Edit, Delete, Details, Index)
dotnet aspnet-codegenerator controller \
    -name ProductosController \
    -m MiApi.Models.Producto \
    -dc MiApi.Data.AppDbContext \
    -namespace MiApi.Controllers \
    -outDir Controllers \
    --useDefaultLayout \
    --referenceScriptLibraries
```

### Generar Minimal API con CRUD

```bash
dotnet aspnet-codegenerator minimalapi \
    -dc MiApi.Data.AppDbContext \
    -m MiApi.Models.Producto \
    -e ProductosEndpoints \
    -o
```

### Generar Identity (login/registro)

```bash
dotnet aspnet-codegenerator identity \
    -dc MiApi.Data.AppDbContext \
    --files "Account.Register;Account.Login;Account.Logout"
```

### Opciones del generador

| Opción | Descripción |
|--------|-------------|
| `-name` | Nombre del controller generado |
| `-m` | Modelo/entidad (ej: `MiApi.Models.Producto`) |
| `-dc` | DbContext (ej: `MiApi.Data.AppDbContext`) |
| `-api` | Genera API controller (sin vistas) |
| `-async` | Genera acciones async (await) |
| `-nv` | No genera vistas |
| `-namespace` | Namespace del controller |
| `-outDir` | Carpeta de salida |
| `-f` | Forzar (sobreescribir si existe) |

### El nuevo `dotnet scaffold` (interactivo)

```bash
# Instalar herramienta interactiva
dotnet tool install --global Microsoft.dotnet-scaffold

# Ejecutar (menú interactivo)
dotnet scaffold

# Selecciona: Controllers, Minimal APIs, Identity, Blazor, etc.
```



## 3. Hot Reload: Desarrollo en Vivo

```bash
# Ejecutar con hot reload (recompila al guardar)
dotnet watch

# Equivalente a:
dotnet watch run

# Hot reload en tests (re-ejecuta al guardar)
dotnet watch test

# Sin hot reload (solo reinicia en cambios)
dotnet watch --no-hot-reload

# Especificar proyecto
dotnet watch run --project MiApi/MiApi.csproj

# Puerto específico
dotnet watch run --urls "http://localhost:5000"
```

> 📝 **Nota:** `dotnet watch` es tu mejor amigo durante el desarrollo. Guardas un archivo y la app se actualiza automáticamente. Si el cambio es "rude edit" (cambiar firma de método), te pregunta si quieres reiniciar.

| Comando | Qué hace |
|---------|----------|
| `Ctrl+R` | Forzar reinicio sin cambiar archivos |
| `Ctrl+C` | Detener todo |



## 4. Compilar y Ejecutar

```bash
# Compilar (verificar errores)
dotnet build

# Compilar en Release
dotnet build --configuration Release

# Ejecutar
dotnet run

# Ejecutar en puerto específico
dotnet run --urls "http://localhost:5000"

# Ejecutar proyecto específico
dotnet run --project MiApi/MiApi.csproj

# Ejecutar sin compilar (si ya está compilado)
dotnet run --no-build

# Publicar para producción
dotnet publish -c Release -o ./publish

# Publicar autocontenido (sin .NET runtime en el servidor)
dotnet publish -c Release --self-contained -r linux-x64 -o ./publish

# Limpiar archivos de compilación
dotnet clean

# Restaurar paquetes
dotnet restore

# Ejecutar archivo .cs directo (C# scripting en .NET 10)
dotnet run archivo.cs
```



## 5. Paquetes NuGet

```bash
# Añadir paquete
dotnet add package Newtonsoft.Json
dotnet add package Microsoft.EntityFrameworkCore

# Añadir versión específica
dotnet add package NUnit --version 4.3.2

# Añadir referencia a proyecto
dotnet add MiApi/MiApi.csproj reference MiLibreria/MiLibreria.csproj

# Eliminar paquete
dotnet remove package Newtonsoft.Json

# Listar paquetes del proyecto
dotnet list package

# Listar paquetes con vulnerabilidades
dotnet list package --vulnerable

# Listar paquetes desactualizados
dotnet list package --outdated
```

### Paquetes habituales del curso

```bash
# ORM
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package MongoDB.EntityFrameworkCore

# MongoDB Driver
dotnet add package MongoDB.Driver

# Redis / Caché
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis

# Logging
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File

# Validación
dotnet add package FluentValidation.AspNetCore

# Funcional
dotnet add package CSharpFunctionalExtensions

# Tests
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package NUnit.Analyzers
dotnet add package FluentAssertions
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package coverlet.collector
dotnet add package Testcontainers.PostgreSql
dotnet add package Testcontainers.MongoDb

# Swagger
dotnet add package Swashbuckle.AspNetCore

# Scaffolding
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```



## 6. .NET Tools: Herramientas Globales

```bash
# Listar herramientas instaladas globalmente
dotnet tool list -g

# Instalar herramienta global
dotnet tool install -g dotnet-ef
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet tool install -g Microsoft.dotnet-scaffold

# Actualizar herramienta
dotnet tool update -g dotnet-ef

# Desinstalar herramienta
dotnet tool uninstall -g dotnet-ef

# Herramienta local (por proyecto)
dotnet new tool-manifest
dotnet tool install --local dotnet-ef
dotnet tool list
```



## 7. Entity Framework Core

```bash
# Instalar herramienta EF (si no está)
dotnet tool install -g dotnet-ef

# Crear migración
dotnet ef migrations add InitialCreate
dotnet ef migrations add AddProductoTable

# Especificar proyecto y contexto
dotnet ef migrations add InitialCreate -p MiApi/MiApi.csproj -c AppDbContext

# Listar migraciones
dotnet ef migrations list

# Aplicar migraciones
dotnet ef database update
dotnet ef database update AddProductoTable

# Revertir a migración anterior
dotnet ef database update PreviousMigrationName

# Eliminar última migración (solo si NO está aplicada)
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script -o script.sql

# Script idempotente (se puede ejecutar múltiples veces)
dotnet ef migrations script --idempotent -o script.sql

# Script hasta una migración específica
dotnet ef migrations script InitialCreate AddProductoTable -o script.sql

# Ver SQL que genera una consulta
# En código: query.ToQueryString()
```



## 8. Tests

```bash
# Ejecutar todos los tests
dotnet test

# Con verbosidad
dotnet test --verbosity normal

# Test específico por nombre
dotnet test --filter "FullyQualifiedName~MiTest"

# Tests de un proyecto
dotnet test MiProyecto.Test/MiProyecto.Test.csproj

# Resultados detallados
dotnet test --logger "console;verbosity=detailed"

# Cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Tests en paralelo (por defecto)
dotnet test --parallel

# Tests secuenciales
dotnet test -- NUnit.NumberOfTestWorkers=0

# Generar informe de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```



## 9. Formateo de Código

```bash
# Formatear según .editorconfig
dotnet format

# Solo estilo de código
dotnet format style

# Solo formateo de espacios
dotnet format whitespace

# Verificar sin modificar (CI/CD)
dotnet format --verify-no-changes

# Formatear proyecto específico
dotnet format MiApi/MiApi.csproj

# Corregir regla específica
dotnet format style --diagnostics IDE0005 --severity info
```



## 10. User Secrets (Secretos de Desarrollo)

```bash
# Inicializar user secrets en un proyecto
dotnet user-secrets init

# Guardar un secreto
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=miapp"
dotnet user-secrets set "Jwt:Key" "mi-clave-secreta-super-larga"

# Ver todos los secretos
dotnet user-secrets list

# Eliminar un secreto
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"

# Eliminar todos los secretos
dotnet user-secrets clear
```

> 📝 **Nota:** Los user secrets solo funcionan en desarrollo y NO se suben a git. Son ideales para connection strings, API keys y contraseñas durante el desarrollo local.



## 11. Certificados de Desarrollo

```bash
# Crear certificado de desarrollo (HTTPS)
dotnet dev-certs https

# Verificar si existe
dotnet dev-certs https --check

# Limpiar certificados
dotnet dev-certs https --clean

# Exportar certificado
dotnet dev-certs https -ep "%USERPROFILE%\.aspnet\https\certificado.pfx" -p "contraseña"
```



## 12. Docker

```bash
# Compilar imagen
docker build -t mi-api .

# Ejecutar contenedor
docker run -p 5000:8080 mi-api

# Con variables de entorno
docker run -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Development mi-api

# Docker Compose
docker-compose up -d              # Levantar
docker-compose down               # Detener
docker-compose ps                 # Ver estado
docker-compose logs -f            # Ver logs en tiempo real
docker-compose build              # Reconstruir
docker-compose up -d --build      # Reconstruir y levantar

# Docker Compose con archivo específico
docker-compose -f docker-compose.api.yml up -d
```



## 13. Git

```bash
# Estado
git status
git status --short

# Añadir
git add -A                  # Todo
git add archivo.cs          # Archivo específico
git add *.cs                # Por patrón

# Commitear (mensajes en español)
git commit -m "[feat] Descripción"
git commit -m "[fix] Descripción"
git commit -m "[docs] Descripción"
git commit -m "[refactor] Descripción"
git commit -m "[chore] Descripción"

# Push/Pull
git push
git pull

# Historial
git log --oneline -10

# Diferencias
git diff
git diff --staged

# Ramas
git checkout -b feature/nueva-funcionalidad
git checkout main
git branch -d feature/nueva-funcionalidad
```



> 📝 **Nota:** Este documento se actualizará a medida que se descubran nuevos comandos útiles durante el curso.


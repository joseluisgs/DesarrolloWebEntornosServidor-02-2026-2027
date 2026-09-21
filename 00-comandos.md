# 00. Guía de Supervivencia: Comandos dotnet CLI

> 💡 **Punto de partida:** Esta guía recopila todos los comandos de `dotnet` que necesitas para crear, compilar, ejecutar y testear APIs en .NET 10. Guárdala como referencia rápida.

## Tabla de Contenidos

- [00. Guía de Supervivencia: Comandos dotnet CLI](#00-guía-de-supervivencia-comandos-dotnet-cli)
  - [1. Proyectos y Soluciones](#1-proyectos-y-soluciones)
  - [2. Compilación y Ejecución](#2-compilación-y-ejecución)
  - [3. Paquetes NuGet](#3-paquetes-nuget)
  - [4. Entity Framework Core](#4-entity-framework-core)
  - [5. Tests](#5-tests)
  - [6. Docker](#6-docker)
  - [7. Información del Entorno](#7-información-del-entorno)
  - [8. Comandos Útiles de la Consola](#8-comandos-útiles-de-la-consola)

---

## 1. Proyectos y Soluciones

### Crear proyecto

```bash
# Web API (el más usado)
dotnet new webapi -n MiApi -f net10.0

# Class Library (biblioteca de clases)
dotnet new classlib -n MiLibreria -f net10.0

# Console App
dotnet new console -n MiConsola -f net10.0

# NUnit Test Project
dotnet new nunit -n MiProyecto.Test -f net10.0

# Proyecto vacío
dotnet new web -n MiApiVacia -f net10.0
```

### Crear solución

```bash
# Crear solución (.slnx en .NET 10)
dotnet new sln -n MiSolucion

# Añadir proyectos a la solución
dotnet sln MiSolucion.slnx add MiApi/MiApi.csproj
dotnet sln MiSolucion.slnx add MiApi.Test/MiApi.Test.csproj

# Listar proyectos de la solución
dotnet sln MiSolucion.slnx list

# Eliminar proyecto de la solución
dotnet sln MiSolucion.slnx remove MiApi.Test/MiApi.Test.csproj

# Migrar de .sln a .slnx (si tienes un .sln antiguo)
dotnet sln migrate
```

### Estructura de carpetas

```bash
# Crear estructura de carpetas
mkdir -p MiApi/{Models,Services,Repositories,Controllers,Config,Infrastructures}

# En PowerShell
New-Item -ItemType Directory -Path "MiApi/Models","MiApi/Services","MiApi/Repositories" -Force
```

---

## 2. Compilación y Ejecución

```bash
# Compilar (verifica errores)
dotnet build

# Compilar sin warnings (con TreatWarningsAsErrors)
dotnet build --configuration Release

# Ejecutar
dotnet run

# Ejecutar en un puerto específico
dotnet run --urls "http://localhost:5000"

# Ejecutar un archivo .cs directo (C# 14 scripting)
dotnet run archivo.cs

# Ejecutar sin compilar (si ya está compilado)
dotnet run --no-build

# Limpiar archivos de compilación
dotnet clean

# Restaurar paquetes NuGet
dotnet restore

# Publicar para producción
dotnet publish -c Release -o ./publish
```

---

## 3. Paquetes NuGet

```bash
# Añadir paquete
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Serilog
dotnet add package CSharpFunctionalExtensions

# Añadir paquete con versión específica
dotnet add package NUnit --version 4.3.2

# Añadir referencia a proyecto
dotnet add MiApi/MiApi.csproj reference MiLibreria/MiLibreria.csproj

# Eliminar paquete
dotnet remove package Microsoft.EntityFrameworkCore

# Listar paquetes
dotnet list package

# Listar paquetes con vulnerabilidades
dotnet list package --vulnerable

# Actualizar paquete
dotnet add package Newtonsoft.Json --version 13.0.3
```

### Paquetes habituales en el curso

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
dotnet add package Microsoft.Extensions.Caching.Memory

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

# HTTP Client
dotnet add package Refit

# Swagger
dotnet add package Swashbuckle.AspNetCore
```

---

## 4. Entity Framework Core

### Migraciones

```bash
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

# Revertir migración
dotnet ef database update PreviousMigrationName

# Eliminar última migración (solo si NO está aplicada)
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script -o script.sql
dotnet ef migrations script --idempotent -o script.sql
```

### Configurar connectionString sin appsettings.json

```bash
# Añadir connection string al proyecto
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=miapp;Username=user;Password=pass"
```

---

## 5. Tests

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar con verbosidad
dotnet test --verbosity normal

# Ejecutar un test específico por nombre
dotnet test --filter "FullyQualifiedName~MiTest"

# Ejecutar tests de un proyecto específico
dotnet test MiProyecto.Test/MiProyecto.Test.csproj

# Ejecutar y mostrar resultados detallados
dotnet test --logger "console;verbosity=detailed"

# Ejecutar con cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar y generar informe de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Ejecutar tests en paralelo (por defecto)
dotnet test --parallel

# Ejecutar tests secuencialmente
dotnet test -- NUnit.NumberOfTestWorkers=0
```

---

## 6. Docker

```bash
# Compilar imagen
docker build -t mi-api .

# Ejecutar contenedor
docker run -p 5000:8080 mi-api

# Ejecutar con variables de entorno
docker run -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Development mi-api

# Docker Compose
docker-compose up -d          # Levantar servicios
docker-compose down           # Detener servicios
docker-compose ps             # Ver estado
docker-compose logs -f        # Ver logs en tiempo real
docker-compose build          # Reconstruir imágenes
docker-compose up -d --build  # Reconstruir y levantar

# Docker Compose con archivo específico
docker-compose -f docker-compose.api.yml up -d
```

---

## 7. Información del Entorno

```bash
# Versión de .NET instalada
dotnet --version

# Todas las versiones instaladas
dotnet --list-sdks

# Runtimes instalados
dotnet --list-runtimes

# Información detallada
dotnet --info

# Verificar si un paquete está instalado
dotnet list MiApi/MiApi.csproj package
```

---

## 8. Comandos Útiles de la Consola

### Windows PowerShell

```powershell
# Crear directorio
New-Item -ItemType Directory -Path "carpeta/subcarpeta" -Force

# Copiar archivo
Copy-Item "origen.cs" "destino.cs" -Force

# Copiar directorio completo
Copy-Item "carpeta1" "carpeta2" -Recurse -Force

# Eliminar archivo
Remove-Item "archivo.cs" -Force

# Eliminar directorio
Remove-Item "carpeta" -Recurse -Force

# Listar archivos
Get-ChildItem -Recurse -Filter "*.cs"

# Buscar en archivos
Get-ChildItem -Recurse -Filter "*.cs" | Select-String "class "

# Contar líneas de un archivo
(Get-Content "archivo.md" | Measure-Object -Line).Lines
```

### Git

```bash
# Ver estado
git status
git status --short

# Añadir archivos
git add -A              # Todo
git add archivo.cs      # Archivo específico
git add *.cs            # Por patrón

# Commitear (mensajes en español)
git commit -m "[feat] Descripción"
git commit -m "[fix] Descripción"
git commit -m "[docs] Descripción"
git commit -m "[refactor] Descripción"
git commit -m "[chore] Descripción"

# Push
git push

# Pull
git pull

# Ver historial
git log --oneline -10

# Ver diferencias
git diff
git diff --staged

# Crear rama
git checkout -b feature/nueva-funcionalidad

# Cambiar de rama
git checkout main

# Eliminar rama
git branch -d feature/nueva-funcionalidad
```

---

> 📝 **Nota:** Este documento se actualizará a medida que se descubran nuevos comandos útiles durante el curso.


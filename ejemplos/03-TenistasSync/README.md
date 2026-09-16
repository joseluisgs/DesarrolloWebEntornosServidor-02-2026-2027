# 🎾 Ejemplo 03: Programación Síncrona y Colecciones

## Descripción

Este ejemplo demuestra **programación síncrona** en C# y el uso de **LINQ** como equivalente al **Stream API** de Java. Se enfoca en operaciones comunes sobre colecciones sin usar programación asíncrona.

## Conceptos Java → C#

### Colecciones

| Java | C# | Descripción |
|------|-----|-------------|
| `ArrayList<T>` | `List<T>` | Lista dinámica |
| `HashMap<K,V>` | `Dictionary<K,V>` | Mapa clave-valor |
| `HashSet<T>` | `HashSet<T>` | Conjunto sin duplicados |
| `Collections.sort()` | `List.Sort()` o LINQ `OrderBy` | Ordenamiento |

### Stream API vs LINQ

| Java (Stream API) | C# (LINQ) | Descripción |
|-------------------|-----------|-------------|
| `.stream()` | *(no necesario)* | LINQ está integrado |
| `.filter(predicate)` | `.Where(predicate)` | Filtrado |
| `.map(function)` | `.Select(function)` | Transformación |
| `.sorted(comparator)` | `.OrderBy()` / `.OrderByDescending()` | Ordenamiento |
| `.limit(n)` | `.Take(n)` | Limitar resultados |
| `.skip(n)` | `.Skip(n)` | Saltar elementos |
| `.distinct()` | `.Distinct()` | Eliminar duplicados |
| `.anyMatch(predicate)` | `.Any(predicate)` | ¿Alguno cumple? |
| `.allMatch(predicate)` | `.All(predicate)` | ¿Todos cumplen? |
| `.noneMatch(predicate)` | `!Any(predicate)` | ¿Ninguno cumple? |
| `.findFirst()` | `.FirstOrDefault()` | Primer elemento |
| `.count()` | `.Count()` / `.LongCount()` | Contar elementos |
| `.collect(Collectors.toList())` | `.ToList()` | Materializar a lista |
| `.collect(Collectors.groupingBy())` | `.GroupBy().ToDictionary()` | Agrupar |
| `.collect(Collectors.summarizingInt())` | `.Sum()`, `.Average()`, etc | Estadísticas |

## Estructura del Proyecto

```
TenistasSync.Console/
├── Models/
│   └── Tenista.cs                 # Modelo POCO (equivalente a POJO)
├── Services/
│   └── TenistaService.cs          # Servicio con operaciones de negocio
├── Utils/
│   └── CollectionUtils.cs         # Utilidades para List<T> y LINQ
└── Program.cs                     # Demo completa de operaciones
```

## Ejemplos de Código

### Filtrado (Filter)

#### Java (Stream API)
```java
List<Tenista> españoles = tenistas.stream()
    .filter(t -> t.getPais().equals("España"))
    .collect(Collectors.toList());
```

#### C# (LINQ)
```csharp
var españoles = tenistas.Where(t => t.Pais == "España").ToList();
```

### Mapeo (Map)

#### Java
```java
List<String> nombres = tenistas.stream()
    .map(Tenista::getNombre)
    .collect(Collectors.toList());
```

#### C#
```csharp
var nombres = tenistas.Select(t => t.Nombre).ToList();
```

### Ordenamiento y Limitación

#### Java
```java
List<Tenista> top3 = tenistas.stream()
    .sorted(Comparator.comparing(Tenista::getRanking))
    .limit(3)
    .collect(Collectors.toList());
```

#### C#
```csharp
var top3 = tenistas.OrderBy(t => t.Ranking).Take(3).ToList();
```

### Agrupación

#### Java
```java
Map<String, List<Tenista>> porPais = tenistas.stream()
    .collect(Collectors.groupingBy(Tenista::getPais));
```

#### C#
```csharp
var porPais = tenistas
    .GroupBy(t => t.Pais)
    .ToDictionary(g => g.Key, g => g.ToList());
```

### Estadísticas

#### Java
```java
IntSummaryStatistics stats = tenistas.stream()
    .collect(Collectors.summarizingInt(Tenista::getTitulos));
    
int total = stats.getSum();
double promedio = stats.getAverage();
int min = stats.getMin();
int max = stats.getMax();
```

#### C#
```csharp
var total = tenistas.Sum(t => t.Titulos);
var promedio = tenistas.Average(t => t.Titulos);
var min = tenistas.Min(t => t.Titulos);
var max = tenistas.Max(t => t.Titulos);
```

### Any / All / None Match

#### Java
```java
boolean hayEspañoles = tenistas.stream()
    .anyMatch(t -> t.getPais().equals("España"));
    
boolean todosJovenes = tenistas.stream()
    .allMatch(t -> t.getEdad() < 30);
    
boolean ningunoViejo = tenistas.stream()
    .noneMatch(t -> t.getEdad() > 50);
```

#### C#
```csharp
bool hayEspañoles = tenistas.Any(t => t.Pais == "España");
bool todosJovenes = tenistas.All(t => t.Edad < 30);
bool ningunoViejo = !tenistas.Any(t => t.Edad > 50);
```

## Compilar y Ejecutar

### Compilar
```bash
cd ejemplos/03-TenistasSync/TenistasSync.Console
dotnet build
```

### Ejecutar
```bash
dotnet run
```

### Ejecutar Tests
```bash
cd ../TenistasSync.Tests
dotnet test --logger "console;verbosity=detailed"
```

## Salida Esperada

```
╔══════════════════════════════════════════════════════════════════════╗
║  🎾 EJEMPLO PROGRAMACIÓN SÍNCRONA - Colecciones y LINQ             ║
╚══════════════════════════════════════════════════════════════════════╝

═══════════════════════════════════════════════════════════════
📋 DEMO 1: Operaciones Básicas con List<T>
═══════════════════════════════════════════════════════════════

📊 Total de tenistas: 8
...
```

## Características Principales

### 1. **Operaciones sobre List<T>**
- Filtrado con `Where`
- Transformación con `Select`
- Ordenamiento con `OrderBy`
- Limitación con `Take`

### 2. **Agrupaciones**
- Por país
- Por rango de edad
- Estadísticas agregadas

### 3. **Búsquedas**
- Búsqueda por ID
- Búsqueda con predicados
- Verificación de existencia

### 4. **Estadísticas**
- Suma, promedio, mínimo, máximo
- Conteo con condiciones
- Particionamiento

## Query Syntax (Alternativa)

LINQ también ofrece una sintaxis similar a SQL:

### Method Syntax (más común)
```csharp
var españoles = tenistas
    .Where(t => t.Pais == "España")
    .OrderBy(t => t.Ranking)
    .ToList();
```

### Query Syntax (alternativa)
```csharp
var españoles = from t in tenistas
                where t.Pais == "España"
                orderby t.Ranking
                select t;
```

## Ventajas de LINQ sobre Stream API

| Ventaja | Descripción |
|---------|-------------|
| **Type-safe** | Compilado, no strings |
| **IntelliSense** | Autocompletado completo |
| **Refactoring** | Renombrar propiedades actualiza queries |
| **Sintaxis natural** | Más concisa y legible |
| **Deferred execution** | Lazy evaluation automática |
| **Query syntax** | Opción similar a SQL |

## Cuándo Usar Operaciones Síncronas

### ✅ Usa operaciones síncronas cuando:
- Los datos están en memoria (no I/O)
- Las operaciones son rápidas
- No hay llamadas a bases de datos o APIs
- Las colecciones son pequeñas o medianas

### ❌ Evita operaciones síncronas cuando:
- Hay operaciones I/O (base de datos, archivos, red)
- Las operaciones pueden tardar mucho tiempo
- Necesitas paralelismo
- Trabajas con streams grandes de datos

## Siguiente Paso

Para ver manejo funcional de errores con Result Pattern:
```bash
cd ../../04-TenistasResult
```

Para ver programación asíncrona:
```bash
cd ../../05-DesayunoAsincrono
```

## Recursos Adicionales

- [LINQ Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- [101 LINQ Samples](https://learn.microsoft.com/en-us/samples/dotnet/try-samples/101-linq-samples/)
- [Stream API vs LINQ](https://stackoverflow.com/questions/tagged/linq+java-stream)

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

- 🌐 Web: [https://joseluisgs.dev](https://joseluisgs.dev)
- 📧 Email: joseluis.gonzalez@profesor.com
- 🐦 Twitter: [@JoseLuisGS_](https://twitter.com/JoseLuisGS_)

## Licencia

Creative Commons BY-NC-SA 4.0

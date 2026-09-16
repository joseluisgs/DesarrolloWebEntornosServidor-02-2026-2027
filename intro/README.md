# 🔄 LINQ vs Java Stream API - Guía Completa

## Tabla de Contenidos
- [Introducción](#introducción)
- [Conceptos Fundamentales](#conceptos-fundamentales)
- [Comparación Lado a Lado](#comparación-lado-a-lado)
- [Operaciones de Filtrado](#operaciones-de-filtrado)
- [Operaciones de Transformación](#operaciones-de-transformación)
- [Operaciones de Agregación](#operaciones-de-agregación)
- [Operaciones de Ordenamiento](#operaciones-de-ordenamiento)
- [Operaciones de Agrupación](#operaciones-de-agrupación)
- [Lazy vs Eager Evaluation](#lazy-vs-eager-evaluation)
- [LINQ con Bases de Datos](#linq-con-bases-de-datos)

## Introducción

**LINQ (Language Integrated Query)** en C# y **Stream API** en Java son soluciones similares para trabajar con colecciones de datos de forma funcional y declarativa.

### Comparación de Alto Nivel

| Aspecto | Java Stream API | C# LINQ |
|---------|----------------|---------|
| **Introducción** | Java 8 (2014) | C# 3.0 (2007) |
| **Sintaxis** | Métodos encadenados | Métodos o sintaxis de consulta |
| **Type Safety** | ✅ Sí | ✅ Sí |
| **Lazy Evaluation** | ✅ Sí | ✅ Sí (IEnumerable) |
| **Parallelización** | `parallelStream()` | `AsParallel()` / PLINQ |
| **Base de datos** | No directamente | ✅ Sí (LINQ to SQL, EF Core) |

## Conceptos Fundamentales

### Java Stream API

```java
List<String> nombres = Arrays.asList("Ana", "Juan", "Pedro", "María");

// Stream API - Funcional y fluido
List<String> resultado = nombres.stream()
    .filter(n -> n.length() > 3)      // Filtrar
    .map(String::toUpperCase)          // Transformar
    .sorted()                          // Ordenar
    .collect(Collectors.toList());     // Recolectar
```

### C# LINQ - Sintaxis de Método

```csharp
var nombres = new List<string> { "Ana", "Juan", "Pedro", "María" };

// LINQ (Method Syntax) - Similar a Stream API
var resultado = nombres
    .Where(n => n.Length > 3)          // Filtrar
    .Select(n => n.ToUpper())          // Transformar
    .OrderBy(n => n)                   // Ordenar
    .ToList();                         // Recolectar
```

### C# LINQ - Sintaxis de Consulta

```csharp
// LINQ (Query Syntax) - Similar a SQL
var resultado = (from n in nombres
                 where n.Length > 3
                 orderby n
                 select n.ToUpper())
                .ToList();
```

## Comparación Lado a Lado

### Tabla de Equivalencias

| Java Stream API | C# LINQ (Method) | C# LINQ (Query) | Función |
|----------------|------------------|-----------------|---------|
| `stream()` | - | - | Crear stream (LINQ es implícito) |
| `filter(predicate)` | `Where(predicate)` | `where` | Filtrar elementos |
| `map(mapper)` | `Select(selector)` | `select` | Transformar elementos |
| `flatMap(mapper)` | `SelectMany(selector)` | `from ... from` | Aplanar colecciones |
| `distinct()` | `Distinct()` | `distinct` | Eliminar duplicados |
| `sorted()` | `OrderBy(keySelector)` | `orderby` | Ordenar ascendente |
| `sorted(comparator)` | `OrderByDescending()` | `orderby ... descending` | Ordenar descendente |
| `limit(n)` | `Take(n)` | - | Tomar primeros n |
| `skip(n)` | `Skip(n)` | - | Saltar primeros n |
| `forEach(action)` | `ForEach(action)` (List) | - | Ejecutar acción |
| `collect(Collectors.toList())` | `ToList()` | - | Convertir a lista |
| `collect(Collectors.toSet())` | `ToHashSet()` | - | Convertir a conjunto |
| `count()` | `Count()` | - | Contar elementos |
| `anyMatch(predicate)` | `Any(predicate)` | - | ¿Alguno cumple? |
| `allMatch(predicate)` | `All(predicate)` | - | ¿Todos cumplen? |
| `noneMatch(predicate)` | `!Any(predicate)` | - | ¿Ninguno cumple? |
| `findFirst()` | `FirstOrDefault()` | - | Primer elemento |
| `findAny()` | `FirstOrDefault()` | - | Cualquier elemento |
| `reduce(accumulator)` | `Aggregate(func)` | - | Reducir a un valor |
| `collect(Collectors.groupingBy())` | `GroupBy(keySelector)` | `group by` | Agrupar elementos |
| `collect(Collectors.joining())` | `string.Join()` | - | Unir strings |

## Operaciones de Filtrado

### filter / Where

#### Java
```java
List<Integer> numeros = Arrays.asList(1, 2, 3, 4, 5, 6);

// Filtrar números pares
List<Integer> pares = numeros.stream()
    .filter(n -> n % 2 == 0)
    .collect(Collectors.toList());
// Resultado: [2, 4, 6]
```

#### C# (Method Syntax)
```csharp
var numeros = new List<int> { 1, 2, 3, 4, 5, 6 };

// Filtrar números pares
var pares = numeros
    .Where(n => n % 2 == 0)
    .ToList();
// Resultado: [2, 4, 6]
```

#### C# (Query Syntax)
```csharp
var pares = (from n in numeros
             where n % 2 == 0
             select n)
            .ToList();
```

### Filtrado con Múltiples Condiciones

#### Java
```java
public class Persona {
    String nombre;
    int edad;
    // constructor, getters
}

List<Persona> mayoresDeEdad = personas.stream()
    .filter(p -> p.getEdad() >= 18)
    .filter(p -> p.getNombre().startsWith("A"))
    .collect(Collectors.toList());
```

#### C#
```csharp
public class Persona
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
}

var mayoresDeEdad = personas
    .Where(p => p.Edad >= 18)
    .Where(p => p.Nombre.StartsWith("A"))
    .ToList();

// O combinado
var mayoresDeEdad2 = personas
    .Where(p => p.Edad >= 18 && p.Nombre.StartsWith("A"))
    .ToList();
```

## Operaciones de Transformación

### map / Select

#### Java
```java
List<String> nombres = Arrays.asList("ana", "juan", "pedro");

// Convertir a mayúsculas
List<String> mayusculas = nombres.stream()
    .map(String::toUpperCase)
    .collect(Collectors.toList());
// Resultado: ["ANA", "JUAN", "PEDRO"]

// Extraer longitudes
List<Integer> longitudes = nombres.stream()
    .map(String::length)
    .collect(Collectors.toList());
// Resultado: [3, 4, 5]
```

#### C#
```csharp
var nombres = new List<string> { "ana", "juan", "pedro" };

// Convertir a mayúsculas
var mayusculas = nombres
    .Select(n => n.ToUpper())
    .ToList();
// Resultado: ["ANA", "JUAN", "PEDRO"]

// Extraer longitudes
var longitudes = nombres
    .Select(n => n.Length)
    .ToList();
// Resultado: [3, 4, 5]
```

### flatMap / SelectMany

#### Java
```java
List<List<Integer>> listas = Arrays.asList(
    Arrays.asList(1, 2),
    Arrays.asList(3, 4),
    Arrays.asList(5, 6)
);

// Aplanar a una sola lista
List<Integer> aplanada = listas.stream()
    .flatMap(List::stream)
    .collect(Collectors.toList());
// Resultado: [1, 2, 3, 4, 5, 6]
```

#### C#
```csharp
var listas = new List<List<int>>
{
    new List<int> { 1, 2 },
    new List<int> { 3, 4 },
    new List<int> { 5, 6 }
};

// Aplanar a una sola lista
var aplanada = listas
    .SelectMany(lista => lista)
    .ToList();
// Resultado: [1, 2, 3, 4, 5, 6]
```

### Transformación de Objetos

#### Java
```java
public class PersonaDTO {
    String nombre;
    int edad;
}

List<PersonaDTO> dtos = personas.stream()
    .map(p -> new PersonaDTO(p.getNombre(), p.getEdad()))
    .collect(Collectors.toList());
```

#### C#
```csharp
public class PersonaDTO
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
}

// Con constructor
var dtos = personas
    .Select(p => new PersonaDTO { Nombre = p.Nombre, Edad = p.Edad })
    .ToList();

// O con tipos anónimos
var anonimos = personas
    .Select(p => new { p.Nombre, p.Edad })
    .ToList();
```

## Operaciones de Agregación

### count / Count

#### Java
```java
long total = numeros.stream().count();
long pares = numeros.stream().filter(n -> n % 2 == 0).count();
```

#### C#
```csharp
int total = numeros.Count();
int pares = numeros.Where(n => n % 2 == 0).Count();
// O más conciso
int pares2 = numeros.Count(n => n % 2 == 0);
```

### sum / Sum, min / Min, max / Max

#### Java
```java
int suma = numeros.stream().mapToInt(Integer::intValue).sum();
OptionalInt minimo = numeros.stream().mapToInt(Integer::intValue).min();
OptionalInt maximo = numeros.stream().mapToInt(Integer::intValue).max();

// Con objetos
Optional<Persona> mayor = personas.stream()
    .max(Comparator.comparing(Persona::getEdad));
```

#### C#
```csharp
int suma = numeros.Sum();
int minimo = numeros.Min();
int maximo = numeros.Max();

// Con objetos
var mayor = personas.MaxBy(p => p.Edad);
var menor = personas.MinBy(p => p.Edad);

// O con ordenamiento
var mayor2 = personas.OrderByDescending(p => p.Edad).First();
```

### reduce / Aggregate

#### Java
```java
// Producto de todos los números
Optional<Integer> producto = numeros.stream()
    .reduce((a, b) -> a * b);

// Con valor inicial
int producto2 = numeros.stream()
    .reduce(1, (a, b) -> a * b);
```

#### C#
```csharp
// Producto de todos los números
int producto = numeros.Aggregate((a, b) => a * b);

// Con valor inicial
int producto2 = numeros.Aggregate(1, (a, b) => a * b);

// Ejemplo más complejo: concatenar nombres
string nombresConcatenados = personas
    .Aggregate("", (acc, p) => acc + p.Nombre + ", ");
```

## Operaciones de Ordenamiento

### sorted / OrderBy

#### Java
```java
// Ordenar números
List<Integer> ordenados = numeros.stream()
    .sorted()
    .collect(Collectors.toList());

// Ordenar descendente
List<Integer> descendente = numeros.stream()
    .sorted(Comparator.reverseOrder())
    .collect(Collectors.toList());

// Ordenar objetos por campo
List<Persona> ordenadosPorEdad = personas.stream()
    .sorted(Comparator.comparing(Persona::getEdad))
    .collect(Collectors.toList());

// Ordenamiento múltiple
List<Persona> ordenados = personas.stream()
    .sorted(Comparator.comparing(Persona::getEdad)
                      .thenComparing(Persona::getNombre))
    .collect(Collectors.toList());
```

#### C#
```csharp
// Ordenar números
var ordenados = numeros.OrderBy(n => n).ToList();

// Ordenar descendente
var descendente = numeros.OrderByDescending(n => n).ToList();

// Ordenar objetos por campo
var ordenadosPorEdad = personas.OrderBy(p => p.Edad).ToList();

// Ordenamiento múltiple
var ordenados = personas
    .OrderBy(p => p.Edad)
    .ThenBy(p => p.Nombre)
    .ToList();

// Con Query Syntax
var ordenados2 = (from p in personas
                  orderby p.Edad, p.Nombre
                  select p)
                 .ToList();
```

## Operaciones de Agrupación

### groupingBy / GroupBy

#### Java
```java
// Agrupar por primera letra
Map<Character, List<String>> agrupados = nombres.stream()
    .collect(Collectors.groupingBy(n -> n.charAt(0)));
// Resultado: {A=[Ana], J=[Juan], P=[Pedro], M=[María]}

// Agrupar personas por edad
Map<Integer, List<Persona>> porEdad = personas.stream()
    .collect(Collectors.groupingBy(Persona::getEdad));

// Contar por grupo
Map<Integer, Long> conteo = personas.stream()
    .collect(Collectors.groupingBy(
        Persona::getEdad,
        Collectors.counting()
    ));
```

#### C#
```csharp
// Agrupar por primera letra
var agrupados = nombres
    .GroupBy(n => n[0])
    .ToDictionary(g => g.Key, g => g.ToList());
// Resultado: {A=[Ana], J=[Juan], P=[Pedro], M=[María]}

// Agrupar personas por edad
var porEdad = personas
    .GroupBy(p => p.Edad)
    .ToDictionary(g => g.Key, g => g.ToList());

// Contar por grupo
var conteo = personas
    .GroupBy(p => p.Edad)
    .ToDictionary(g => g.Key, g => g.Count());

// Con Query Syntax (más legible)
var agrupados2 = from p in personas
                 group p by p.Edad into g
                 select new { Edad = g.Key, Personas = g.ToList() };
```

## Lazy vs Eager Evaluation

### Java Stream (Lazy)
```java
List<Integer> numeros = Arrays.asList(1, 2, 3, 4, 5);

Stream<Integer> stream = numeros.stream()
    .filter(n -> {
        System.out.println("Filtrando: " + n);
        return n % 2 == 0;
    })
    .map(n -> {
        System.out.println("Mapeando: " + n);
        return n * 2;
    });

// Hasta aquí NO se ejecuta nada (lazy)

List<Integer> resultado = stream.collect(Collectors.toList());
// Ahora sí se ejecuta todo (terminal operation)
```

### C# LINQ (Lazy con IEnumerable)
```csharp
var numeros = new List<int> { 1, 2, 3, 4, 5 };

IEnumerable<int> query = numeros
    .Where(n => {
        Console.WriteLine($"Filtrando: {n}");
        return n % 2 == 0;
    })
    .Select(n => {
        Console.WriteLine($"Mapeando: {n}");
        return n * 2;
    });

// Hasta aquí NO se ejecuta nada (lazy)

var resultado = query.ToList();
// Ahora sí se ejecuta todo (materialization)
```

## LINQ con Bases de Datos

Una ventaja de LINQ sobre Stream API es su integración directa con bases de datos:

### Entity Framework Core + LINQ

```csharp
// La query se traduce directamente a SQL
var tenistas = await context.Tenistas
    .Where(t => t.Pais == "España")
    .Where(t => t.Ranking < 10)
    .OrderBy(t => t.Ranking)
    .ToListAsync();

// SQL generado:
// SELECT * FROM tenistas 
// WHERE pais = 'España' AND ranking < 10 
// ORDER BY ranking
```

### Java (Spring Data JPA) - Requiere definir métodos

```java
// Tienes que definir métodos en el repositorio
public interface TenistaRepository extends JpaRepository<Tenista, Long> {
    List<Tenista> findByPaisAndRankingLessThanOrderByRanking(
        String pais, int ranking);
}

// O usar @Query con JPQL (strings)
@Query("SELECT t FROM Tenista t WHERE t.pais = :pais AND t.ranking < :ranking ORDER BY t.ranking")
List<Tenista> findTenistas(@Param("pais") String pais, @Param("ranking") int ranking);
```

## Ejemplos Completos

### Ejemplo 1: Procesamiento de Empleados

#### Java
```java
public class Empleado {
    String nombre;
    String departamento;
    double salario;
    // constructor, getters
}

// Obtener salario promedio por departamento
Map<String, Double> salarioPromedio = empleados.stream()
    .collect(Collectors.groupingBy(
        Empleado::getDepartamento,
        Collectors.averagingDouble(Empleado::getSalario)
    ));

// Top 5 mejor pagados
List<Empleado> top5 = empleados.stream()
    .sorted(Comparator.comparing(Empleado::getSalario).reversed())
    .limit(5)
    .collect(Collectors.toList());
```

#### C#
```csharp
public class Empleado
{
    public string Nombre { get; set; }
    public string Departamento { get; set; }
    public double Salario { get; set; }
}

// Obtener salario promedio por departamento
var salarioPromedio = empleados
    .GroupBy(e => e.Departamento)
    .ToDictionary(g => g.Key, g => g.Average(e => e.Salario));

// Top 5 mejor pagados
var top5 = empleados
    .OrderByDescending(e => e.Salario)
    .Take(5)
    .ToList();
```

### Ejemplo 2: Análisis de Texto

#### Java
```java
String texto = "Hola mundo hola java stream api";
String[] palabras = texto.split(" ");

// Contar palabras únicas
long palabrasUnicas = Arrays.stream(palabras)
    .distinct()
    .count();

// Palabra más larga
Optional<String> masLarga = Arrays.stream(palabras)
    .max(Comparator.comparing(String::length));

// Frecuencia de palabras
Map<String, Long> frecuencia = Arrays.stream(palabras)
    .collect(Collectors.groupingBy(
        String::toLowerCase,
        Collectors.counting()
    ));
```

#### C#
```csharp
string texto = "Hola mundo hola csharp linq";
string[] palabras = texto.Split(' ');

// Contar palabras únicas
int palabrasUnicas = palabras.Distinct().Count();

// Palabra más larga
string? masLarga = palabras.MaxBy(p => p.Length);

// Frecuencia de palabras
var frecuencia = palabras
    .GroupBy(p => p.ToLower())
    .ToDictionary(g => g.Key, g => g.Count());
```

## Recursos Adicionales

### Documentación Oficial
- [LINQ Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- [Java Stream API](https://docs.oracle.com/javase/8/docs/api/java/util/stream/package-summary.html)
- [101 LINQ Samples](https://docs.microsoft.com/en-us/samples/dotnet/try-samples/101-linq-samples/)

### Siguientes Pasos
1. **[Ejemplo EF Core](../../ejemplos/02-AccesoEntityFramework/)** - LINQ con bases de datos
2. **[Collections](../03-collections/)** - Estructuras de datos en C#
3. **[Async/Await](../../ejemplos/05-DesayunoAsincrono/)** - Programación asíncrona

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

## Licencia

Creative Commons BY-NC-SA 4.0

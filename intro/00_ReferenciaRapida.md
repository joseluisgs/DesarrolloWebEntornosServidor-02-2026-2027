# ⚡ Referencia Rápida Java → C# /.NET

## Sintaxis Básica

| Java | C# | Notas |
|------|-----|-------|
| `public class Main { }` | `public class Main { }` | Idéntico |
| `public static void main(String[] args)` | `public static void Main(string[] args)` | PascalCase en C# |
| `System.out.println()` | `Console.WriteLine()` | |
| `String` | `string` | Minúsculas en C# |
| `boolean` | `bool` | |
| `Integer` | `int` | No hay wrapper class necesario |
| `final` | `const` o `readonly` | `const` = compile-time, `readonly` = runtime |
| `@Override` | `override` | Keyword, no atributo |

## Propiedades vs Getters/Setters

```java
// JAVA
private String nombre;
public String getNombre() { return nombre; }
public void setNombre(String nombre) { this.nombre = nombre; }
```

```csharp
// C#
public string Nombre { get; set; }  // Auto-property
```

## Null Safety

```java
// JAVA
String texto = valor != null ? valor : "default";
Optional<String> opt = Optional.ofNullable(valor);
String result = opt.orElse("default");
```

```csharp
// C#
string texto = valor ?? "default";           // Null-coalescing
string? maybeNull = objeto?.Propiedad;       // Null-conditional

// Ejemplo más completo similar a Optional
public T? GetValue<T>() where T : class
{
    // Retorna null si no existe
    return _value;
}

var result = GetValue<string>() ?? "default";
```

## Collections

| Java | C# |
|------|-----|
| `List<String> lista = new ArrayList<>()` | `List<string> lista = new List<string>()` o `var lista = new List<string>()` |
| `Map<String, Integer> mapa = new HashMap<>()` | `Dictionary<string, int> dic = new Dictionary<string, int>()` |
| `Set<String> set = new HashSet<>()` | `HashSet<string> set = new HashSet<string>()` |

## LINQ vs Stream API

```java
// JAVA Stream API
List<String> resultado = nombres.stream()
    .filter(n -> n.length() > 3)
    .map(String::toUpperCase)
    .sorted()
    .collect(Collectors.toList());
```

```csharp
// C# LINQ
var resultado = nombres
    .Where(n => n.Length > 3)
    .Select(n => n.ToUpper())
    .OrderBy(n => n)
    .ToList();
```

## Spring Boot → ASP.NET Core

### Anotaciones/Atributos

| Spring Boot | ASP.NET Core |
|------------|--------------|
| `@SpringBootApplication` | `var builder = WebApplication.CreateBuilder()` |
| `@RestController` | `[ApiController]` |
| `@RequestMapping("/api")` | `[Route("api/[controller]")]` |
| `@GetMapping` | `[HttpGet]` |
| `@PostMapping` | `[HttpPost]` |
| `@PutMapping` | `[HttpPut]` |
| `@DeleteMapping` | `[HttpDelete]` |
| `@PathVariable` | `{id}` en ruta |
| `@RequestParam` | `[FromQuery]` |
| `@RequestBody` | `[FromBody]` |
| `@Autowired` | Constructor injection (automático) |
| `@Service` | `builder.Services.AddScoped<>()` |
| `@Repository` | `builder.Services.AddScoped<>()` |
| `@Component` | `builder.Services.AddScoped<>()` |

### Dependency Injection

```java
// JAVA - Spring
@Service
public class ProductoService {
    @Autowired
    private ProductoRepository repository;
}
```

```csharp
// C# - ASP.NET Core
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;
    
    public ProductoService(IProductoRepository repository)
    {
        _repository = repository;
    }
}

// Registro en Program.cs
builder.Services.AddScoped<IProductoService, ProductoService>();
```

### Controllers

```java
// JAVA
@RestController
@RequestMapping("/api/productos")
public class ProductoController {
    @GetMapping("/{id}")
    public ResponseEntity<Producto> getById(@PathVariable Long id) {
        return ResponseEntity.ok(service.findById(id));
    }
}
```

```csharp
// C#
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetById(long id)
    {
        var producto = await _service.GetByIdAsync(id);
        return Ok(producto);
    }
}
```

## JPA → Entity Framework Core

### Anotaciones/Atributos de Entidad

| JPA (Java) | EF Core (C#) |
|-----------|--------------|
| `@Entity` | `[Table("tablename")]` o Fluent API |
| `@Table(name="...")` | `[Table("...")]` |
| `@Id` | `[Key]` |
| `@GeneratedValue` | `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]` |
| `@Column(name="...")` | `[Column("...")]` |
| `@OneToMany` | `.HasMany()` (Fluent API) |
| `@ManyToOne` | `.HasOne()` (Fluent API) |

### Entidades

```java
// JAVA - JPA
@Entity
@Table(name = "productos")
public class Producto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    
    @Column(name = "nombre", nullable = false)
    private String nombre;
    
    // getters y setters
}
```

```csharp
// C# - EF Core
[Table("productos")]
public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }
    
    [Required]
    [Column("nombre")]
    public string Nombre { get; set; }
}
```

### Repositorios

```java
// JAVA - Spring Data JPA
@Repository
public interface ProductoRepository extends JpaRepository<Producto, Long> {
    List<Producto> findByNombre(String nombre);
}
```

```csharp
// C# - EF Core Repository Pattern
public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<List<Producto>> FindByNombreAsync(string nombre)
    {
        return await _context.Productos
            .Where(p => p.Nombre == nombre)
            .ToListAsync();
    }
}
```

### Consultas

```java
// JAVA - JPQL
@Query("SELECT p FROM Producto p WHERE p.precio > :precio")
List<Producto> findByPrecioMayorQue(@Param("precio") double precio);
```

```csharp
// C# - LINQ
public async Task<List<Producto>> FindByPrecioMayorQueAsync(double precio)
{
    return await _context.Productos
        .Where(p => p.Precio > precio)
        .ToListAsync();
}
```

## Testing

### JUnit → NUnit

```java
// JAVA - JUnit
@Test
public void deberiaRetornarProducto() {
    // Arrange
    when(repository.findById(1L)).thenReturn(Optional.of(producto));
    
    // Act
    Producto resultado = service.findById(1L);
    
    // Assert
    assertNotNull(resultado);
    assertEquals("Laptop", resultado.getNombre());
}
```

```csharp
// C# - NUnit
[Test]
public async Task DeberiaRetornarProducto()
{
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(producto);
    
    // Act
    var resultado = await _service.GetByIdAsync(1);
    
    // Assert
    Assert.IsNotNull(resultado);
    Assert.AreEqual("Laptop", resultado.Nombre);
}
```

## Async/Await

```java
// JAVA - CompletableFuture
public CompletableFuture<List<Producto>> getProductosAsync() {
    return CompletableFuture.supplyAsync(() -> {
        return repository.findAll();
    });
}
```

```csharp
// C# - async/await
public async Task<List<Producto>> GetProductosAsync()
{
    return await _repository.GetAllAsync();
}
```

## Configuración

### application.properties → appsettings.json

```properties
# JAVA - application.properties
server.port=8080
spring.datasource.url=jdbc:postgresql://localhost:5432/db
spring.datasource.username=admin
spring.datasource.password=password
```

```json
// C# - appsettings.json
{
  "Kestrel": {
    "Endpoints": {
      "Http": { "Url": "http://localhost:5000" }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=db;Username=admin;Password=password"
  }
}
```

## Comandos CLI

| Maven/Gradle | dotnet CLI |
|-------------|------------|
| `mvn clean install` | `dotnet build` |
| `mvn spring-boot:run` | `dotnet run` |
| `mvn test` | `dotnet test` |
| `mvn package` | `dotnet publish` |
| `mvn dependency:tree` | `dotnet list package` |

## Paquetes Comunes

| Java (Maven) | .NET (NuGet) |
|-------------|-------------|
| `spring-boot-starter-web` | (Built-in en .NET SDK) |
| `spring-boot-starter-data-jpa` | `Microsoft.EntityFrameworkCore` |
| `postgresql` driver | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `lombok` | (No necesario, C# tiene properties) |
| `junit` | `NUnit` o `xUnit` |
| `mockito` | `Moq` |
| `jackson` | `System.Text.Json` o `Newtonsoft.Json` |

## Convenciones de Nombres

| Java | C# |
|------|-----|
| `nombreVariable` (camelCase) | `nombreVariable` (camelCase) |
| `getNombre()` (método) | `Nombre` (property - PascalCase) |
| `NombreClase` (PascalCase) | `NombreClase` (PascalCase) |
| `CONSTANTE` (UPPER_SNAKE_CASE) | `Constante` (PascalCase) |

## Recursos Rápidos

- **[Fundamentos C#](./csharp/01-fundamentos/)** - Sintaxis completa
- **[LINQ](./csharp/04-streams-linq/)** - Stream API equivalente
- **[Web API](./netcore/02-web-api/)** - Spring Boot equivalente
- **[EF Core](./ejemplos/02-AccesoEntityFramework/)** - JPA equivalente

---

## Autor

**José Luis González Sánchez** - [https://joseluisgs.dev](https://joseluisgs.dev)

## Licencia

Creative Commons BY-NC-SA 4.0

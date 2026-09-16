# Test de ASP.NET Core - API REST

- [Test de ASP.NET Core - API REST](#test-de-aspnet-core---api-rest)
    - [Módulos y Fundamentos de ASP.NET Core (Preguntas 1-15)](#módulos-y-fundamentos-de-aspnet-core-preguntas-1-15)
    - [ASP.NET Core y Arquitectura Web (Preguntas 16-30)](#aspnet-core-y-arquitectura-web-preguntas-16-30)
    - [Procesamiento de Datos, Servicios y Validación (Preguntas 31-45)](#procesamiento-de-datos-servicios-y-validación-preguntas-31-45)
    - [Entity Framework Core y Persistencia (Preguntas 46-75)](#entity-framework-core-y-persistencia-preguntas-46-75)
    - [Testing (Preguntas 76-85)](#testing-preguntas-76-85)
    - [Almacenamiento de Ficheros y Recursos (Preguntas 86-90)](#almacenamiento-de-ficheros-y-recursos-preguntas-86-90)
    - [Comunicación Avanzada:  SignalR y WebSockets (Preguntas 91-95)](#comunicación-avanzada--signalr-y-websockets-preguntas-91-95)
    - [Resultados Avanzados, Paginación y Criterios (Preguntas 96-105)](#resultados-avanzados-paginación-y-criterios-preguntas-96-105)
    - [Protocolo HTTP y JWT (Preguntas 106-115)](#protocolo-http-y-jwt-preguntas-106-115)
    - [ASP.NET Core Identity y Seguridad (Preguntas 116-130)](#aspnet-core-identity-y-seguridad-preguntas-116-130)
    - [NoSQL, MongoDB y GraphQL (Preguntas 131-140)](#nosql-mongodb-y-graphql-preguntas-131-140)
    - [Redis, Email y Despliegue (Preguntas 141-150)](#redis-email-y-despliegue-preguntas-141-150)

---

### Módulos y Fundamentos de ASP.NET Core (Preguntas 1-15)

1. ¿Cuál de las siguientes características *no* es mencionada como un beneficio de ASP.NET Core? 
   a) Multiplataforma (Windows, Linux, macOS).
   b) Alto rendimiento con Kestrel.
   c) Gestión directa de hilos del sistema operativo.
   d) Inyección de Dependencias integrada.

2. ¿Qué principio de diseño de software invierte el control del flujo de la aplicación, dejando que el *framework* se encargue de la creación y gestión de objetos?
   a) Dependency Injection (DI).
   b) Middleware Pipeline.
   c) Inversión de Control (IoC).
   d) Modelo-Vista-Controlador (MVC).

3. En ASP.NET Core, ¿cuál es el término utilizado para referirse a los objetos que son registrados y administrados por el contenedor de Inyección de Dependencias? 
   a) Middleware.
   b) Services.
   c) Filters.
   d) Handlers.

4. ¿Cuál es el servidor web predeterminado y de alto rendimiento incluido en ASP.NET Core? 
   a) IIS.
   b) Apache.
   c) Kestrel.
   d) Nginx.

5. ASP.NET Core simplifica la configuración de aplicaciones web.  ¿Qué patrón arquitectónico permite configurar servicios y el pipeline de middleware? 
   a) Program.cs + Startup.cs (versiones antiguas).
   b) Global.asax. 
   c) Web.config.
   d) AssemblyInfo.cs.

6. ¿Cuál de los siguientes middleware NO es parte del pipeline estándar de ASP.NET Core?
   a) Routing.
   b) Authentication.
   c) Entity Framework Core.
   d) Static Files.

7. La Inyección de Dependencias (DI) en ASP.NET Core se configura típicamente en: 
   a) Controllers.
   b) Program.cs (usando `builder.Services`).
   c) appsettings.json.
   d) launchSettings.json.

8. ¿Qué tecnología de Microsoft proporciona capacidades de comunicación en tiempo real bidireccional, similar a WebSockets? 
   a) ASP.NET Core MVC.
   b) Entity Framework Core.
   c) SignalR.
   d) Razor Pages.

9. ¿Qué método se usa típicamente para registrar un servicio con lifetime "Scoped" en ASP.NET Core?
   a) `builder.Services.AddSingleton<IService, Service>()`.
   b) `builder.Services.AddScoped<IService, Service>()`.
   c) `builder.Services.AddTransient<IService, Service>()`.
   d) `builder.Services.Configure<Service>()`.

10. ¿Cuál es el propósito principal del Middleware en ASP.NET Core?
    a) Almacenar datos en base de datos.
    b) Procesar las peticiones HTTP en un pipeline.
    c) Generar vistas HTML.
    d) Validar modelos de datos.

11. ¿Qué tecnología de ASP.NET Core está diseñada para crear páginas web dinámicas con una sintaxis simplificada, sin necesidad de controladores separados?
    a) Web API.
    b) Razor Pages.
    c) Blazor.
    d) MVC.

12. Entity Framework Core es el ORM de Microsoft y proporciona soporte para diversas bases de datos, incluyendo: 
    a) SQL Server.
    b) PostgreSQL.
    c) SQLite.
    d) Todas las anteriores.

13. ¿Qué herramienta de línea de comandos se utiliza para crear, ejecutar y administrar proyectos de ASP.NET Core?
    a) npm.
    b) dotnet CLI.
    c) nuget.
    d) msbuild.

14. En ASP.NET Core, ¿cómo se registran típicamente los servicios para Inyección de Dependencias? 
    a) Usando atributos como `[Service]`.
    b) Usando archivos XML de configuración.
    c) En Program.cs con métodos como `AddScoped`, `AddTransient`, `AddSingleton`.
    d) Automáticamente por convención de nombres.

15. ¿Qué archivo de configuración de ASP.NET Core almacena configuraciones de la aplicación en formato JSON, como cadenas de conexión? 
    a) web.config.
    b) appsettings.json.
    c) launchSettings.json. 
    d) Program.cs.

---

### ASP.NET Core y Arquitectura Web (Preguntas 16-30)

16. ¿Qué atributo de ASP.NET Core marca una clase como un controlador de API que devuelve datos serializados automáticamente?
    a) `[Controller]`.
    b) `[ApiController]`.
    c) `[Route]`.
    d) `[HttpGet]`.

17. ¿Qué es un "Middleware" en ASP.NET Core? 
    a) Una base de datos en memoria.
    b) Un componente del pipeline de procesamiento de peticiones HTTP.
    c) Un validador de modelos. 
    d) Un generador de vistas. 

18. En el patrón Modelo-Vista-Controlador (MVC) implementado por ASP.NET Core MVC, ¿qué componente es responsable de renderizar el modelo en una vista?
    a) El Modelo.
    b) El Controlador.
    c) La Vista (View).
    d) El Repository.

19. ¿Qué archivo de configuración lee ASP.NET Core al iniciar para establecer propiedades como el puerto de ejecución?
    a) web.config.
    b) global.json.
    c) appsettings.json.
    d) Program.cs.

20. En ASP.NET Core Web API, ¿qué atributo se utiliza para indicar que un método de controlador responde a peticiones HTTP GET?
    a) `[Get]`.
    b) `[HttpGet]`.
    c) `[Route("GET")]`.
    d) `[Action("Get")]`.

21. ASP.NET Core proporciona soporte integrado para la validación de modelos utilizando:
    a) Data Annotations.
    b) Fluent Validation.
    c) Manual Validation.
    d) a y b.

22. Si deseas que solo se cree una instancia de un servicio durante toda la vida de la aplicación, ¿qué lifetime debes usar al registrarlo?
    a) Transient.
    b) Scoped.
    c) Singleton.
    d) Request.

23. ¿Qué patrón se utiliza típicamente para separar la lógica de acceso a datos en ASP.NET Core?
    a) Factory Pattern.
    b) Repository Pattern. 
    c) Singleton Pattern.
    d) Observer Pattern.

24. ¿Qué interfaz de ASP.NET Core te permite ejecutar código al iniciar la aplicación?
    a) `IApplicationLifetime`.
    b) `IHostedService`.
    c) `IStartupFilter`.
    d) Todas las anteriores.

25. Para devolver una respuesta HTTP con código de estado personalizado y datos, se recomienda usar:
    a) `Ok()`.
    b) `ActionResult<T>` o `IActionResult`.
    c) `View()`.
    d) `Content()`.

26. ¿Qué atributo se utiliza para obtener información de la ruta en un método de controlador (ej.  `/funkos/{id}`)?
    a) `[FromQuery]`.
    b) `[FromBody]`.
    c) `[FromRoute]`.
    d) `[FromHeader]`.

27. Para recibir datos serializados en el cuerpo de una petición POST o PUT, ¿qué atributo se debe usar en el parámetro del método?
    a) `[FromQuery]`.
    b) `[FromBody]`.
    c) `[FromRoute]`.
    d) `[FromHeader]`.

28. ¿Qué verbo HTTP se utiliza para la actualización parcial de un recurso? 
    a) POST.
    b) PUT.
    c) DELETE.
    d) PATCH.

29. ¿Qué código de respuesta HTTP indica que la solicitud ha tenido éxito y se ha creado un nuevo recurso?
    a) 200 OK.
    b) 204 No Content.
    c) 404 Not Found.
    d) 201 Created.

30. ¿Qué método de extensión se utiliza para añadir controladores al contenedor de servicios en Program.cs?
    a) `builder.Services.AddMvc()`.
    b) `builder.Services.AddControllers()`.
    c) `builder.Services.AddRazorPages()`.
    d) a y b.

---

### Procesamiento de Datos, Servicios y Validación (Preguntas 31-45)

31. ¿Cuál es la principal ventaja de usar la declaración `using` al leer o escribir archivos en C#?
    a) Permite el uso de LINQ.
    b) Se asegura de que cada recurso se libere automáticamente (Dispose).
    c) Define el formato de intercambio de datos.
    d) Solo funciona con archivos JSON.

32. ¿Qué formato de intercambio de datos utiliza delimitadores para separar campos, representando datos tabulares?
    a) JSON.
    b) XML.
    c) CSV.
    d) BSON.

33. ¿Cuál es el propósito principal del Patrón DTO (Data Transfer Object)?
    a) Encapsular la lógica de negocio. 
    b) Exponer las entidades de la base de datos directamente.
    c) Transportar datos entre capas, evitando exponer modelos internos.
    d) Administrar el ciclo de vida de los servicios.

34. En ASP.NET Core, ¿qué clase base de excepción se puede lanzar para que el framework la convierta automáticamente en una respuesta HTTP?
    a) `Exception`.
    b) `HttpResponseException`.
    c) `ApplicationException`.
    d) Middleware personalizado con try-catch.

35. ¿Qué método se debe llamar en Program.cs para habilitar el soporte de caché en memoria en ASP.NET Core?
    a) `builder.Services.AddCaching()`.
    b) `builder.Services.AddMemoryCache()`.
    c) `builder.Services.AddDistributedCache()`.
    d) `builder.Services.AddResponseCaching()`.

36. El atributo `[ResponseCache]` en un controlador se utiliza para:
    a) Almacenar datos en Redis.
    b) Configurar la caché de respuestas HTTP en el cliente o proxies.
    c) Validar modelos. 
    d) Serializar JSON.

37. ¿Qué namespace contiene las anotaciones de validación de datos en . NET? 
    a) System. Validation.
    b) System.ComponentModel.DataAnnotations.
    c) Microsoft.AspNetCore.Validation.
    d) System.Web.Validation.

38. Para que ASP.NET Core valide automáticamente un modelo que llega en el cuerpo de una petición, ¿qué debe hacerse?
    a) Añadir `[ApiController]` al controlador.
    b) Añadir `[Required]` a los campos del modelo.
    c) Ambas (el atributo `[ApiController]` habilita la validación automática).
    d) Llamar manualmente a `ModelState.IsValid`.

39. Si un campo debe ser un número positivo (mayor que cero), ¿qué atributo de validación se debe usar?
    a) `[Range(1, int.MaxValue)]`.
    b) `[PositiveNumber]`.
    c) `[GreaterThan(0)]`.
    d) `[MinValue(1)]`.

40. ¿Qué propiedad del controlador se debe verificar para comprobar si el modelo es válido después de la validación automática?
    a) `this.IsValid`.
    b) `ModelState.IsValid`.
    c) `Request.IsValidModel`.
    d) `Validator.IsValid`.

41. ¿Cuál es la función de los AutoMapper en una arquitectura de capas que utiliza DTOs?
    a) Almacenar datos en caché.
    b) Convertir automáticamente un DTO a una entidad y viceversa.
    c) Realizar la lógica de negocio. 
    d) Configurar la seguridad.

42. ¿Qué atributo de validación garantiza que un campo no sea nulo ni vacío?
    a) `[NotNull]`.
    b) `[Required]`.
    c) `[NotEmpty]`.
    d) `[MinLength(1)]`.

43. En ASP.NET Core, ¿cómo se puede retornar un código de estado HTTP específico con un mensaje de error personalizado desde un controlador?
    a) `return StatusCode(404, "No encontrado");`.
    b) `return BadRequest("Error");`.
    c) `return NotFound("No encontrado");`.
    d) Todas las anteriores.

44. ¿Cuál es la principal diferencia entre `IMemoryCache` y `IDistributedCache`?
    a) `IMemoryCache` es local; `IDistributedCache` puede ser compartida (Redis, SQL Server).
    b) `IMemoryCache` es más lenta. 
    c) `IDistributedCache` solo funciona en Windows.
    d) No hay diferencia.

45. ¿Qué atributo asegura que un string no sea nulo y contenga al menos un carácter no en blanco?
    a) `[Required]`.
    b) `[StringLength(1)]`.
    c) `[Required]` con validación adicional.
    d) No existe un atributo específico; se debe usar validación personalizada.

---

### Entity Framework Core y Persistencia (Preguntas 46-75)

46. Entity Framework Core es un ORM (Object-Relational Mapper) que se superpone a qué tecnología de acceso a datos?
    a) ADO.NET.
    b) LINQ to SQL.
    c) Dapper.
    d) NHibernate.

47. ¿Qué método de migración de EF Core crea la base de datos y aplica todas las migraciones pendientes?
    a) `Add-Migration`.
    b) `Update-Database`.
    c) `Remove-Migration`.
    d) `Script-Migration`.

48. Para definir una clase C# como una entidad mapeada a una tabla, ¿qué convención o atributo es típicamente necesario?
    a) Heredar de `DbContext`.
    b) Añadir el atributo `[Table]`.
    c) Registrarla en el `DbContext` como `DbSet<T>`.
    d) b y c.

49. ¿Qué archivo se puede usar opcionalmente para ejecutar scripts SQL de inicialización en EF Core?
    a) `Seed.sql`.
    b) Método `OnModelCreating` con `HasData`.
    c) `Migrations/<timestamp>_InitialCreate.cs`.
    d) b y c.

50. ¿Cuál de las siguientes estrategias de generación de claves se recomienda para SQL Server con identidades autoincrementales?
    a) `ValueGeneratedNever`.
    b) `ValueGeneratedOnAdd`.
    c) Configurar la columna como `IDENTITY` en la base de datos.
    d) b y c.

51. En EF Core, ¿cuál es la ventaja de usar GUIDs (UUID) como identificadores? 
    a) Son más eficientes en índices.
    b) Permiten generar IDs en cliente sin consultar la base de datos.
    c) Son más legibles.
    d) La base de datos los genera automáticamente.

52. ¿Qué método del `DbContext` se sobreescribe para configurar el modelo de datos mediante Fluent API?
    a) `OnConfiguring`.
    b) `OnModelCreating`.
    c) `SaveChanges`.
    d) `Database. Migrate()`.

53. ¿Qué método de EF Core se usa para indicar que todas las operaciones relacionadas deben aplicarse a las entidades relacionadas?
    a) Configurar relaciones con `WithMany` y `WithOne`.
    b) Usar el método `Include()`.
    c) Configurar el `DeleteBehavior` en cascada.
    d) Todas las anteriores.

54. ¿Qué opción de `DeleteBehavior` propaga la eliminación desde una entidad padre a sus entidades relacionadas?
    a) `DeleteBehavior.Restrict`.
    b) `DeleteBehavior.SetNull`.
    c) `DeleteBehavior. Cascade`.
    d) `DeleteBehavior.NoAction`.

55. ¿Qué relación se establece cuando una entidad tiene una colección de otra entidad (ej., `Categoria` tiene muchos `Productos`)?
    a) One-to-One.
    b) One-to-Many.
    c) Many-to-Many.
    d) Self-referencing.

56. ¿Qué tipo de entidad en EF Core no tiene tabla propia y se almacena en la misma tabla que la entidad propietaria?
    a) Entidad derivada.
    b) Owned Entity (entidad de propiedad).
    c) Shadow Property. 
    d) View Entity.

57. ¿Cuál es la estrategia de herencia en EF Core donde todas las clases se mapean a una sola tabla con una columna discriminadora?
    a) Table-per-Type (TPT).
    b) Table-per-Hierarchy (TPH).
    c) Table-per-Concrete-Type (TPC).
    d) No soportada.

58. Para eliminar automáticamente entidades huérfanas (sin padre), ¿qué configuración de relación se debe usar en Fluent API?
    a) `.OnDelete(DeleteBehavior. Cascade)`.
    b) `.IsRequired()`.
    c) Configurar la relación como dependiente (owned) o configurar manualmente la eliminación.
    d) EF Core lo hace automáticamente.

59. En el contexto de un borrado lógico, ¿cuál es la principal ventaja? 
    a) Ahorro de espacio inmediato.
    b) Mejor rendimiento.
    c) Recuperación de datos y auditoría.
    d) Simplifica las migraciones.

60. Para evitar la recursión infinita al serializar relaciones bidireccionales a JSON, ¿qué se debe configurar en ASP.NET Core?
    a) Usar `[JsonIgnore]` en una de las propiedades de navegación.
    b) Configurar `ReferenceHandler. Preserve` en el serializador JSON.
    c) Usar DTOs sin las propiedades de navegación problemáticas.
    d) Todas las anteriores.

61. Para ejecutar una consulta SQL raw (nativa) en EF Core, ¿qué método se utiliza?
    a) `context.Database.ExecuteSqlRaw()`.
    b) `context.Set<T>().FromSqlRaw()`.
    c) Ambos. 
    d) `context.Query<T>()`.

62. ¿Qué método de EF Core permite definir consultas LINQ complejas directamente en el repositorio?
    a) `Where()`, `Select()`, `Include()`, etc.
    b) `FromSqlRaw()`.
    c) `ExecuteSqlCommand()`.
    d) Todos los métodos LINQ estándar.

63. Si deseas usar SQL nativo para recuperar entidades, ¿qué método se recomienda?
    a) `FromSqlRaw()` o `FromSqlInterpolated()`.
    b) `ExecuteSqlRaw()`.
    c) `Database.SqlQuery()`.
    d) a y c.

64. ¿Qué interfaz base NO es específica de EF Core pero proporciona operaciones CRUD básicas?
    a) `IRepository<T>` (patrón Repository genérico).
    b) `DbSet<T>`.
    c) `DbContext`.
    d) Ninguna; todas son de EF Core.

65. Para habilitar características como paginación y ordenación, ¿qué método de extensión LINQ se utiliza?
    a) `Skip()` y `Take()`.
    b) `OrderBy()` o `OrderByDescending()`.
    c) `ToListAsync()`.
    d) a y b.

66. Para probar repositorios con EF Core de forma aislada, ¿qué proveedor de base de datos en memoria se puede usar?
    a) SQL Server.
    b) InMemory provider de EF Core.
    c) SQLite en modo memoria.
    d) b y c.

67. En una relación bidireccional, ¿cómo se especifica cuál entidad es la "principal" (propietaria de la clave foránea)?
    a) Usando Fluent API con `HasForeignKey()`.
    b) Convención de nombres (la que tiene la propiedad de clave foránea).
    c) Ambas.
    d) No es necesario especificarlo. 

68. ¿Qué método de EF Core se utiliza para aplicar migraciones pendientes a la base de datos? 
    a) `context.Database.Migrate()`.
    b) `Update-Database` (comando CLI).
    c) Ambas.
    d) `EnsureCreated()`.

69. TestContainers para .  NET lanza instancias reales de bases de datos en: 
    a) Procesos separados.
    b) Contenedores Docker.
    c) Máquinas virtuales.
    d) En memoria.

70. ¿Qué significa "Code First" en EF Core? 
    a) Escribir SQL primero. 
    b) Definir el modelo en código C# y generar la base de datos a partir de él.
    c) Crear la base de datos manualmente primero.
    d) Usar migraciones automáticamente.

71. ¿Cuál es una limitación del proveedor InMemory de EF Core? 
    a) Es muy lento.
    b) No soporta todas las funcionalidades de una base de datos real (ej., restricciones de clave foránea estrictas).
    c) Requiere Docker. 
    d) Solo funciona con SQL Server.

72. ¿Qué estrategia se recomienda para evitar ciclos infinitos al serializar JSON en ASP.NET Core?
    a) Configurar `ReferenceHandler.IgnoreCycles`.
    b) Usar DTOs. 
    c) Usar `[JsonIgnore]`.
    d) Todas las anteriores.

73. Para auditar automáticamente cambios en entidades (ej., fecha de creación), ¿qué se puede sobreescribir en el `DbContext`?
    a) `SaveChanges()` o `SaveChangesAsync()`.
    b) `OnModelCreating()`.
    c) `OnConfiguring()`.
    d) No es posible. 

74. ¿Qué estrategia de herencia en EF Core crea una tabla por cada clase concreta?
    a) TPH. 
    b) TPT.
    c) TPC.
    d) Todas las anteriores.

75. ¿Qué método se utiliza para incluir entidades relacionadas en una consulta (eager loading)?
    a) `Include()`.
    b) `ThenInclude()`.
    c) Ambas.
    d) `Load()`.

---

### Testing (Preguntas 76-85)

76. ¿Qué framework de testing se utiliza en . NET para pruebas unitarias?
    a) Moq.
    b) NUnit.
    c) xUnit.
    d) b y c.

77. En NUnit o xUnit, ¿qué atributo se utiliza para marcar un método como test?
    a) `[TestMethod]`.
    b) `[Test]` (NUnit) o `[Fact]` (xUnit).
    c) `[UnitTest]`.
    d) `[TestCase]`.

78. ¿Qué biblioteca se utiliza en .NET para crear mocks de dependencias?
    a) Moq.
    b) NSubstitute.
    c) FakeItEasy.
    d) Todas las anteriores.

79. En Moq, ¿qué método se utiliza para crear un mock de una interfaz?
    a) `new Mock<IService>()`.
    b) `Mock.Create<IService>()`.
    c) `Moq.Of<IService>()`.
    d) `Mock.Build<IService>()`.

80. Para verificar que un método de un mock fue llamado, ¿qué método de Moq se utiliza? 
    a) `mock.Verify()`.
    b) `mock.VerifyAll()`.
    c) `mock.Assert()`.
    d) a y b.

81. ¿Qué método de Assert se utiliza en xUnit para verificar que dos valores son iguales?
    a) `Assert.True()`.
    b) `Assert.Equal()`.
    c) `Assert.Same()`.
    d) `Assert.IsEqual()`.

82. ¿Cuál es la principal ventaja de usar mocks en testing?
    a) Aumentar la cobertura de código.
    b) Aislar la unidad bajo prueba de sus dependencias.
    c) Acelerar la compilación.
    d) Evitar escribir tests de integración.

83. En ASP.NET Core Identity testing, ¿qué se puede usar para simular un usuario autenticado?
    a) Crear un `ClaimsPrincipal` personalizado.
    b) Configurar el `HttpContext` con un usuario mock.
    c) Usar bibliotecas de testing de autenticación.
    d) Todas las anteriores.

84. Para probar un `DbContext` sin afectar la base de datos real, ¿qué proveedor se puede usar?
    a) SQL Server real.
    b) InMemory provider. 
    c) SQLite en memoria.
    d) b y c.

85. ¿Qué método de Moq se utiliza para configurar el valor de retorno de un método?
    a) `Setup()`.
    b) `Returns()`.
    c) Ambos (`Setup().Returns()`).
    d) `Configure()`.

---

### Almacenamiento de Ficheros y Recursos (Preguntas 86-90)

86. Para que un controlador pueda recibir un archivo, el tipo de contenido debe ser:
    a) `application/json`.
    b) `application/xml`.
    c) `multipart/form-data`.
    d) `text/plain`.

87. ¿Qué tipo de parámetro se utiliza en un método de controlador para recibir un archivo?
    a) `[FromBody] string file`.
    b) `IFormFile file`.
    c) `[FromQuery] byte[] file`.
    d) `Stream file`.

88. Para devolver un archivo desde un controlador, ¿qué método de `Controller` se utiliza?
    a) `Ok()`.
    b) `File()`.
    c) `Content()`.
    d) `Json()`.

89. ¿Qué namespace contiene `IFormFile`?
    a) `System.IO`.
    b) `Microsoft.AspNetCore.Http`.
    c) `System.Net.Http`.
    d) `System.Web`.

90. Para servir archivos estáticos (como imágenes), ¿qué middleware debe estar configurado en Program.cs?
    a) `app.UseStaticFiles()`.
    b) `app.UseFileServer()`.
    c) `app.UseDirectoryBrowser()`.
    d) a o b.

---

### Comunicación Avanzada:  SignalR y WebSockets (Preguntas 91-95)

91. ¿Qué tecnología de Microsoft proporciona comunicación en tiempo real bidireccional sobre WebSockets (con fallback a otras tecnologías)?
    a) gRPC.
    b) SignalR.
    c) WCF.
    d) MQTT.

92. ¿Cuál es el principal beneficio de usar SignalR? 
    a) Simplifica el acceso a bases de datos.
    b) Habilita comunicación en tiempo real entre servidor y clientes.
    c) Reemplaza REST. 
    d) Solo funciona en Windows.

93. En SignalR, ¿qué concepto se utiliza para enviar mensajes a un grupo específico de clientes?
    a) Channels.
    b) Groups.
    c) Topics.
    d) Queues.

94. ¿Qué clase base se debe heredar para crear un Hub de SignalR?
    a) `Controller`.
    b) `Hub`.
    c) `WebSocketHandler`.
    d) `MiddlewareBase`.

95. ¿Qué método de un Hub de SignalR se invoca cuando un cliente se conecta?
    a) `OnConnected()`.
    b) `OnConnectedAsync()`.
    c) `ConnectAsync()`.
    d) b (en versiones modernas).

---

### Resultados Avanzados, Paginación y Criterios (Preguntas 96-105)

96. ¿Qué mecanismo permite a un cliente especificar el formato de los datos (JSON/XML)?
    a) Routing.
    b) Content Negotiation.
    c) Middleware.
    d) Filters.

97. Para soportar XML en ASP.NET Core Web API, ¿qué método se debe agregar en Program.cs?
    a) `builder.Services.AddXmlSerializers()`.
    b) `builder.Services.AddControllers().AddXmlSerializerFormatters()`.
    c) `builder.Services.AddXml()`.
    d) No es necesario; está incluido por defecto.

98. Para implementar paginación en EF Core, ¿qué métodos LINQ se utilizan?
    a) `Skip()` y `Take()`.
    b) `Page()` y `PageSize()`.
    c) `Limit()` y `Offset()`.
    d) `GetPage()`.

99. ¿Qué clase o estructura se utiliza comúnmente para devolver resultados paginados en ASP.NET Core?
    a) `PagedList<T>` (biblioteca personalizada o NuGet).
    b) `IPagedList<T>`.
    c) `PaginationResult<T>` (custom).
    d) Todas las anteriores (depende de la implementación).

100. Para filtrar resultados dinámicamente (ej., por nombre o precio), ¿qué enfoque se recomienda?
     a) Pasar parámetros opcionales al controlador y aplicar `Where()` condicionalmente.
     b) Usar bibliotecas como Sieve o OData.
     c) Construir expresiones LINQ dinámicamente.
     d) Todas las anteriores.

101. En un objeto de paginación personalizado, ¿qué propiedad indica el número total de elementos?
     a) `PageSize`.
     b) `TotalCount`.
     c) `PageNumber`.
     d) `TotalPages`.

102. ¿Cuál es un motivo para implementar paginación? 
     a) Mejorar el rendimiento y reducir la transferencia de datos.
     b) Simplificar el modelo de datos.
     c) Cumplir con JWT. 
     d) Habilitar WebSockets.

103. ¿Qué clase de ASP.NET Core se puede usar para construir URIs dinámicamente?
     a) `UriBuilder`.
     b) `Url.Action()` o `Url.Link()`.
     c) `LinkGenerator`.
     d) Todas las anteriores.

104. Para permitir que un cliente solicite XML usando la URL, ¿qué parámetro de query se puede configurar?
     a) `?format=xml`.
     b) `?accept=xml`.
     c) `?type=xml`.
     d) Depende de la configuración (configurable).

105. ¿Qué biblioteca NuGet proporciona soporte avanzado para filtrado, ordenación y paginación mediante query strings?
     a) AutoMapper.
     b) Sieve.
     c) FluentValidation.
     d) Dapper.

---

### Protocolo HTTP y JWT (Preguntas 106-115)

106. ¿Cuál de los siguientes NO es un verbo HTTP estándar?
     a) GET.
     b) FETCH.
     c) PUT.
     d) DELETE.

107. ¿Qué código HTTP indica "Recurso no encontrado"?
     a) 400.
     b) 401.
     c) 403.
     d) 404.

108. REST es escalable debido a su naturaleza:
     a) Stateful.
     b) Stateless.
     c) Orientada a eventos.
     d) Basada en sesiones.

109. Un JWT se compone de tres partes separadas por puntos:
     a) Header, Payload, Signature. 
     b) Token, Claims, Secret.
     c) Type, Data, Hash.
     d) Issuer, Audience, Expiry.

110. ¿Cuál es el propósito de la Signature en un JWT?
     a) Contener los claims. 
     b) Especificar el algoritmo. 
     c) Verificar integridad y autenticidad.
     d) Cifrar el payload.

111. En el Payload de un JWT, ¿qué claim estándar indica la fecha de expiración?
     a) `iat`.
     b) `nbf`.
     c) `exp`.
     d) `sub`.

112. ¿Qué necesidad de seguridad aborda JWT?
     a) Autenticación sin estado (stateless).
     b) Cifrado de base de datos.
     c) Optimización de red.
     d) Reemplazo de TCP/IP.

113. ¿Qué biblioteca NuGet se utiliza comúnmente para generar y validar JWTs en . NET?
     a) Microsoft.AspNetCore.Authentication.JwtBearer.
     b) System.IdentityModel.Tokens.Jwt.
     c) Ambas.
     d) JWT.NET.

114. ¿Qué método de extensión se llama en Program.cs para habilitar autenticación JWT?
     a) `builder.Services.AddAuthentication(JwtBearerDefaults. AuthenticationScheme).AddJwtBearer()`.
     b) `builder.Services.AddJwt()`.
     c) `builder.Services.AddSecurity()`.
     d) `builder.Services.ConfigureJwt()`.

115. ¿Qué código HTTP indica "No autorizado" (falta autenticación)?
     a) 400.
     b) 401.
     c) 403.
     d) 404.

---

### ASP.NET Core Identity y Seguridad (Preguntas 116-130)

116. ASP.NET Core Identity proporciona funcionalidad para:
     a) Gestión de usuarios y autenticación.
     b) Manejo de roles y claims.
     c) Integración con autenticación externa (Google, Facebook).
     d) Todas las anteriores.

117. ¿Qué clase se debe heredar para definir un usuario personalizado en Identity?
     a) `User`.
     b) `IdentityUser`.
     c) `ApplicationUser`.
     d) b o c (según convención).

118. ¿Qué clase gestiona operaciones de usuarios en Identity?
     a) `UserManager<TUser>`.
     b) `SignInManager<TUser>`.
     c) `RoleManager<TRole>`.
     d) Todas las anteriores (cada una con su función).

119. Para habilitar Identity en una aplicación, ¿qué se debe llamar en Program.cs?
     a) `builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<AppDbContext>()`.
     b) `builder.Services.AddSecurity()`.
     c) `builder.Services.AddAuthorization()`.
     d) `builder.Services.AddAuthentication()`.

120. ¿Qué método de `UserManager` se utiliza para crear un nuevo usuario?
     a) `CreateAsync()`.
     b) `AddUserAsync()`.
     c) `RegisterAsync()`.
     d) `InsertAsync()`.

121. Para almacenar información de autenticación, ¿qué contexto de seguridad utiliza ASP.NET Core? 
     a) `HttpContext.User`.
     b) `Thread.CurrentPrincipal`.
     c) `ClaimsPrincipal. Current`.
     d) a (principalmente).

122. ¿Qué middleware se debe agregar al pipeline para habilitar autenticación?
     a) `app.UseAuthentication()`.
     b) `app.UseAuthorization()`.
     c) Ambos (en ese orden).
     d) Solo el segundo.

123. Para restringir el acceso a un endpoint basado en roles, ¿qué atributo se utiliza?
     a) `[Authorize]`.
     b) `[Authorize(Roles = "Admin")]`.
     c) `[AllowAnonymous]`.
     d) `[RequireRole]`.

124. ¿Qué atributo permite el acceso anónimo a un endpoint en un controlador que requiere autenticación?
     a) `[Authorize(AllowAnonymous = true)]`.
     b) `[AllowAnonymous]`.
     c) `[Public]`.
     d) `[NoAuth]`.

125. Para obtener el usuario actual en un controlador, ¿qué propiedad se utiliza?
     a) `User` (del controlador, que es un `ClaimsPrincipal`).
     b) `HttpContext.User`.
     c) Ambas (son equivalentes en controladores).
     d) `Identity.User`.

126. ¿Qué clase maneja las operaciones de inicio de sesión en Identity?
     a) `UserManager<TUser>`.
     b) `SignInManager<TUser>`.
     c) `AuthenticationManager`.
     d) `LoginManager`.

127. Para configurar la política de contraseñas en Identity, ¿qué se configura en Program.cs?
     a) `builder.Services.Configure<IdentityOptions>(options => { options.Password.RequiredLength = 8; })`.
     b) `builder.Services.AddPasswordPolicy()`.
     c) `builder.Services.ConfigurePassword()`.
     d) No es configurable.

128. ¿Qué significa que Identity trabaje con "claims"?
     a) Que almacena afirmaciones (claims) sobre el usuario (ej., roles, permisos).
     b) Que utiliza un sistema de reclamos de seguros.
     c) Que siempre requiere contraseñas.
     d) Que no soporta autenticación externa.

129. Para configurar HTTPS en ASP.NET Core, ¿qué middleware se agrega al pipeline?
     a) `app.UseHttps()`.
     b) `app.UseHttpsRedirection()`.
     c) `app.UseHsts()`.
     d) b y c.

130. ¿Qué clase representa un conjunto de claims (afirmaciones) sobre un usuario?
     a) `ClaimsIdentity`.
     b) `ClaimsPrincipal`.
     c) `Claim`.
     d) a y b (el principal contiene identidades).

---

### NoSQL, MongoDB y GraphQL (Preguntas 131-140)

131. ¿Cuál es una ventaja de NoSQL sobre SQL?
     a) Fuerte consistencia ACID siempre.
     b) Escalabilidad horizontal y esquemas flexibles.
     c) Soporte SQL estándar.
     d) Mayor madurez en todas las áreas.

132. MongoDB almacena datos en formato:
     a) XML.
     b) JSON (representado como BSON internamente).
     c) CSV.
     d) SQL.

133. Para usar MongoDB en . NET, ¿qué driver NuGet se instala?
     a) MongoDB.Driver.
     b) MongoClient.
     c) AspNetCore.MongoDB.
     d) EntityFrameworkCore.MongoDB.

134. ¿Qué método del driver de MongoDB busca un documento por filtro?
     a) `Find()` o `FindAsync()`.
     b) `Query()`.
     c) `Get()`.
     d) `Select()`.

135. Para definir un modelo en MongoDB con el driver de . NET, ¿qué atributo se usa para el ID?
     a) `[Key]`.
     b) `[BsonId]`.
     c) `[Id]`.
     d) No se requiere atributo si la propiedad se llama `Id` o `_id`.

136. En GraphQL, ¿qué elemento define operaciones de lectura?
     a) `mutation`.
     b) `subscription`.
     c) `query`.
     d) `type`.

137. ¿Cuál es una ventaja de GraphQL sobre REST? 
     a) Múltiples endpoints. 
     b) El cliente solicita exactamente los datos que necesita.
     c) Respuestas fijas.
     d) Requiere versionado estricto.

138. En GraphQL, ¿qué símbolo indica que un campo es obligatorio (non-null)?
     a) `? `.
     b) `*`.
     c) `!`.
     d) `&`.

139. ¿Qué biblioteca NuGet se utiliza para implementar GraphQL en ASP.NET Core?
     a) GraphQL.NET o HotChocolate.
     b) AspNetCore.GraphQL.
     c) EntityFrameworkCore.GraphQL. 
     d) SignalR.GraphQL.

140. Para habilitar suscripciones en GraphQL, ¿qué se necesita típicamente?
     a) SignalR o WebSockets.
     b) Solo HTTP.
     c) gRPC.
     d) MQTT.

---

### Redis, Email y Despliegue (Preguntas 141-150)

141. ¿Qué es Redis?
     a) Un sistema de archivos.
     b) Una base de datos en memoria clave-valor.
     c) Un ORM. 
     d) Un servidor web.

142. ¿Cuál es una ventaja de Redis como caché distribuida?
     a) Limitado a una instancia.
     b) Compartida entre múltiples servidores (escalabilidad).
     c) Más lento que la memoria local.
     d) No soporta TTL.

143. Para desarrollo de email, ¿qué servicio intercepta correos sin enviarlos? 
     a) Gmail.
     b) Mailtrap.
     c) SendGrid.
     d) AWS SES.

144. En . NET, ¿qué biblioteca se recomienda para enviar emails?
     a) System.Net.Mail (obsoleta).
     b) MailKit + MimeKit.
     c) SmtpClient.
     d) SendGrid SDK (para ese proveedor específico).

145. Para diferentes entornos (dev/prod), ¿cómo se configuran los archivos en ASP.NET Core?
     a) `appsettings.json` y `appsettings.{Environment}.json`.
     b) `web.config`.
     c) `settings.ini`.
     d) No es posible. 

146. Para activar un entorno específico al ejecutar una aplicación, ¿qué variable de entorno se configura?
     a) `ASPNETCORE_PROFILE`.
     b) `ASPNETCORE_ENVIRONMENT`.
     c) `DOTNET_ENV`.
     d) `ENVIRONMENT`.

147. ¿Qué es CORS? 
     a) Un protocolo de versionado. 
     b) Un mecanismo de seguridad para solicitudes entre dominios.
     c) Un formato de datos.
     d) Configuración de JWT.

148. ¿Qué biblioteca se usa para documentar APIs en ASP.NET Core con Swagger?
     a) Swashbuckle.AspNetCore.
     b) NSwag.
     c) Ambas.
     d) AspNetCore.OpenApi.

149. En un Dockerfile multi-stage, ¿cuál es el propósito de la primera etapa con el SDK de . NET?
     a) Ejecutar la aplicación.
     b) Compilar y publicar la aplicación.
     c) Servir archivos estáticos.
     d) Configurar Redis.

150. Para documentar un modelo en Swagger, ¿qué atributo de XML Comments o anotaciones se utiliza?
     a) `/// <summary>` (XML Comments).
     b) `[SwaggerSchema]` (Swashbuckle. Annotations).
     c) Ambos.
     d) `[ApiModel]`.

---

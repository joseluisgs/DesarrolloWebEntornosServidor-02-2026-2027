**Test: desarrollo de servicios web en .NET**

**Instrucciones:** Lee cada pregunta cuidadosamente y selecciona la opción que consideres correcta.

---

## PARTE 1 (temas 01-16)

**Tema 01: Servicios web**

1.  ¿Qué es un servicio web en el contexto de una aplicación moderna?
    A) Una página web con diseño responsivo
    B) Un componente del servidor que expone funcionalidad a través de la red (normalmente HTTP) para que otros sistemas la consuman
    C) Un framework de estilos CSS para el front-end
    D) Un tipo de base de datos orientada a documentos

2.  En la arquitectura cliente-servidor, ¿qué papel desempeña el back-end?
    A) Gestionar la interfaz visual que ve el usuario en el navegador
    B) Ejecutarse exclusivamente en el navegador del usuario
    C) Gestionar la lógica de negocio, los datos y atender las peticiones de los clientes
    D) Componer el CSS y el HTML de cada página

3.  Si abres Netflix y buscas una película, ¿qué componente recibe y procesa la petición?
    A) El front-end únicamente
    B) El servidor de la API de Netflix (back-end)
    C) El navegador sin intervención de ningún servidor
    D) La base de datos directamente, sin pasar por el servidor

**Tema 02: APIs REST**

4.  ¿Cuál de estos verbos HTTP se considera idempotente?
    A) POST
    B) PUT
    C) PATCH
    D) CONNECT

5.  Un endpoint crea un recurso nuevo y devuelve `201 Created`. ¿Qué cabecera debería incluir la respuesta para indicar dónde vive el recurso recién creado?
    A) `Location`
    B) `Content-Type`
    C) `Authorization`
    D) `Cache-Control`

6.  En REST, ¿por qué se recomienda usar sustantivos en plural para los endpoints (por ejemplo, `/api/funkos`)?
    A) Porque la especificación HTTP lo exige obligatoriamente
    B) Porque representan colecciones de recursos y mantienen una interfaz uniforme y predecible
    C) Porque ASP.NET Core no admite rutas en singular
    D) Para que las URLs sean más cortas

7.  REST define varias restricciones. ¿Cuál es la que indica que cada petición lleva toda la información necesaria y el servidor no guarda estado del cliente entre peticiones?
    A) Cliente-servidor
    B) Sistema en capas
    C) Sin estado (*stateless*)
    D) Cacheable

**Tema 03: Minimal APIs**

8.  En una Minimal API, ¿qué método se usa para definir un endpoint que responde a GET?
    A) `app.MapGet("/ruta", ...)`
    B) `app.UseGet("/ruta", ...)`
    C) `[HttpGet("/ruta")]`
    D) `app.Route("/ruta", ...)`

9.  ¿Cuál es la principal ventaja de las Minimal APIs frente a los controladores MVC?
    A) Permiten usar vistas Razor
    B) Ofrecen un enfoque con menos ceremonia, ideales para APIs pequeñas y microservicios
    C) No dependen de la inyección de dependencias
    D) Generan documentación Swagger sin ninguna configuración

10. En `app.MapGet("/api/funkos/{id}", (long id) => ...)`, ¿cómo llega el valor de la URL al parámetro `id`?
    A) Por *binding* de ruta automático
    B) Desde un fichero de configuración
    C) Por la cabecera HTTP `X-Id`
    D) Solo se puede obtener con `HttpContext.Request.Query`

11. ¿Qué código de estado devuelve `Results.NotFound()` en una Minimal API?
    A) 200 OK con cuerpo vacío
    B) 404 Not Found
    C) 400 Bad Request
    D) 500 Internal Server Error

**Tema 04: Controladores y MVC APIs**

12. ¿Qué hace el atributo `[ApiController]` en una clase de controlador?
    A) Habilita la validación automática del modelo y devuelve 400 con `ModelState` sin comprobarlo a mano
    B) Desactiva la inyección de dependencias del controlador
    C) Convierte el controlador en una Minimal API
    D) Obliga a usar vistas Razor en todas las acciones

13. Con `[Route("api/[controller]")]` en `FunkosController`, ¿cuál es la ruta resultante para una acción `[HttpGet("{id}")]`?
    A) `/api/funkos/{id}`
    B) `/api/FunkosController/{id}`
    C) `/{id}/api`
    D) `/funkos/api/{id}`

14. ¿Cuál es la diferencia principal entre devolver `IActionResult` y `ActionResult<T>` en una acción?
    A) No hay ninguna diferencia, son sinónimos exactos
    B) `ActionResult<T>` aporta tipado en el cuerpo de éxito (mejor documentación y refactorizado) además de los resultados de error
    C) `ActionResult<T>` solo puede devolver XML
    D) `IActionResult` no puede devolver 404

**Tema 05: Arquitectura y pipeline**

15. En el pipeline de middleware de ASP.NET Core, ¿por qué importa el orden en que se registran?
    A) Porque solo se puede registrar un middleware por aplicación
    B) Porque cada middleware decide si procesa la petición o la pasa al siguiente; el orden define qué ve primero la petición
    C) Porque el compilador lo exige sintácticamente
    D) Porque los middleware solo funcionan con HTTPS

16. Cuando un middleware "corta" la petición (*short-circuit*), por ejemplo devolviendo 401, ¿qué ocurre?
    A) Se ejecutan igualmente todos los middleware posteriores
    B) El resto de la pipeline no se ejecuta para esa petición
    C) La aplicación se reinicia
    D) Se lanza una excepción no controlada

17. ¿Cómo se añade un middleware personalizado al pipeline?
    A) Con `app.UseMiddleware<MiMiddleware>()` o `app.Use(...)`
    B) Con `builder.Services.AddMiddleware<MiMiddleware>()`
    C) Añadiéndolo a un fichero `web.config`
    D) Con el atributo `[Middleware]` sobre el controlador

**Tema 06: Inyección de dependencias**

18. ¿Cuál es el ciclo de vida de un servicio registrado con `AddScoped`?
    A) Una única instancia para toda la aplicación
    B) Una instancia por petición HTTP (scope)
    C) Una nueva instancia cada vez que se resuelve
    D) Se crea en el arranque y nunca se elimina

19. ¿Qué problema puede ocurrir si registras un servicio con estado como `Transient`?
    A) Ninguno, es el ciclo de vida más seguro siempre
    B) Cada consumidor recibe una instancia distinta, así que el estado no se comparte entre ellos
    C) El servicio solo existe durante una petición HTTP
    D) El contenedor lanza una excepción en el arranque

20. ¿Para qué sirve `builder.Services` (`IServiceCollection`)?
    A) Para registrar los servicios que el contenedor de inyección de dependencias podrá resolver
    B) Para ejecutar los middleware del pipeline
    C) Para compilar los controladores del proyecto
    D) Para crear los ficheros de log de la aplicación

**Tema 07: Excepciones y patrón result**

21. ¿Cuál es la función de un manejador global de excepciones (`GlobalExceptionHandler` / `UseExceptionHandler`)?
    A) Evitar que se lanzen excepciones en la aplicación
    B) Capturar excepciones no controladas y devolver una respuesta coherente (500 + Problem Details) sin exponer detalles internos
    C) Registrar las excepciones en la base de datos del cliente
    D) Convertir todas las excepciones automáticamente en 404

22. ¿En qué se diferencia el patrón Result respecto al uso de excepciones?
    A) Result devuelve el éxito o el error como parte del tipo de retorno (`Result<T>`), haciendo el error explícito y composable sin saltos de control
    B) Result siempre es más lento que lanzar excepciones
    C) Result solo sirve para operaciones asíncronas
    D) No hay diferencia, es otro nombre para las excepciones

23. Si la validación de un DTO falla, ¿qué respuesta es la más apropiada?
    A) 200 OK con cuerpo `null`
    B) 400 Bad Request con los detalles de los errores (Problem Details)
    C) 500 Internal Server Error
    D) 301 Moved Permanently

**Tema 08: DTOs, mapeadores, validaciones y consultas**

24. ¿Por qué se usan DTOs en lugar de devolver las entidades de EF Core directamente?
    A) Porque las entidades no se pueden serializar a JSON
    B) Para desacopiar el contrato público de la base de datos, exponer solo lo necesario y evitar problemas como los ciclos de serialización
    C) Porque los DTOs se guardan más rápido en la base de datos
    D) Para no necesitar controladores en la API

25. Con `[Required]`, `[StringLength(50)]` y `[Range(0, 1000)]` en un DTO, ¿qué ocurre si llega un JSON inválido con `[ApiController]`?
    A) El controlador se ejecuta igualmente y hay que comprobarlo a mano
    B) ASP.NET Core responde automáticamente 400 Bad Request con los errores de validación
    C) Se guarda el dato corrupto en la base de datos
    D) Se lanza una excepción 500

26. En el patrón mapeador (`ToFunkoDto`, `ToFunko`), ¿qué responsabilidad cumple?
    A) Conectar con la base de datos
    B) Traducir entre entidades y DTOs en ambos sentidos
    C) Validar los permisos del usuario
    D) Firmar los tokens JWT

**Tema 09: Configuración y logging**

27. ¿Qué fichero se usa para la configuración específica del entorno de producción?
    A) `appsettings.Development.json`
    B) `appsettings.Production.json`
    C) `launchSettings.json`
    D) `global.json`

28. ¿En qué orden se cargan las fuentes de configuración por defecto (de menor a mayor prioridad)?
    A) `appsettings.json` → `appsettings.{Entorno}.json` → variables de entorno
    B) Variables de entorno → `appsettings.json` → `appsettings.{Entorno}.json`
    C) `launchSettings.json` → `appsettings.json` → `web.config`
    D) Solo `appsettings.json`, las demás no existen

29. ¿Qué es el "log estructurado" (*structured logging*)?
    A) Escribir todo el log en una sola línea de texto sin estructura
    B) Registrar eventos con campos nombrados (`{Nombre}`, `{Precio}`) que después se pueden buscar y filtrar
    C) Guardar los logs en ficheros Markdown
    D) Un tipo de base de datos de eventos

**Tema 10: Pruebas y despliegue básicos**

30. ¿Qué significa el patrón AAA en los tests?
    A) Arrange (preparar), Act (ejecutar), Assert (verificar)
    B) Add, Assign, Await
    C) Analyze, Approve, Archive
    D) Assign, Arrange, Assert

31. ¿Qué permite `WebApplicationFactory<T>` en los tests?
    A) Levantar la API real en memoria con su pipeline (sin abrir puerto externo) para pruebas de integración
    B) Generar la documentación Swagger
    C) Compilar el proyecto más rápido
    D) Reemplazar a Docker en producción

32. ¿Qué comando produce los ficheros listos para desplegar (publicación)?
    A) `dotnet build`
    B) `dotnet publish`
    C) `dotnet clean`
    D) `dotnet restore`

**Tema 11: Arquitecturas para servicios**

33. En Clean Architecture, ¿qué depende de qué?
    A) El dominio depende de la infraestructura
    B) Las capas externas (infraestructura, UI) dependen de las internas (dominio); el dominio no depende de nadie
    C) Todas las capas dependen de la capa de presentación
    D) Los servicios dependen de los controladores

34. ¿Cuál es la diferencia entre Repository y Service?
    A) Son sinónimos
    B) El repositorio abstrae el acceso a datos; el servicio encapsula la lógica de negocio
    C) El servicio solo lee y el repositorio solo escribe
    D) El repositorio se registra en el controlador y el servicio en la base de datos

35. Según el Principio de Inversión de Dependencias, ¿debe el negocio depender de interfaces o de implementaciones concretas?
    A) De implementaciones concretas para ir más rápido
    B) De abstracciones (interfaces) que la infraestructura implementa después
    C) De la base de datos directamente
    D) De los controladores de la API

**Tema 12: Entity Framework Core SQL**

36. ¿Qué operación persiste los cambios realizados en el `DbContext`?
    A) `db.SaveChanges()`
    B) `db.Flush()`
    C) `db.UpdateAll()`
    D) `db.CommitAll()`

37. ¿Para qué sirven las migraciones de EF Core?
    A) Para mover la base de datos a otro servidor
    B) Para versionar el esquema de la base de datos y aplicar cambios incrementales (tablas, columnas, índices)
    C) Para migrar datos de MySQL a MongoDB
    D) Para comprimir las tablas grandes

38. ¿Qué hace `HasQueryFilter(e => !e.IsDeleted)` en `OnModelCreating`?
    A) Borra físicamente los registros que cumplen el filtro
    B) Aplica un filtro global (borrado lógico) a todas las consultas de ese tipo
    C) Crea un índice en la columna `IsDeleted`
    D) Desactiva el seguimiento de cambios del `DbContext`

**Tema 13: Entity Framework Core NoSQL con MongoDB**

39. ¿Cuál es la unidad básica de datos en MongoDB?
    A) La fila
    B) El documento (JSON/BSON) dentro de una colección
    C) La tabla
    D) La columna

40. ¿Qué identificador usa MongoDB por defecto para sus documentos?
    A) Un autoincremental SQL
    B) Un `ObjectId` de 12 bytes
    C) Un GUID de Windows
    D) El nombre del documento

41. En MongoDB, ¿cuándo conviene embeber documentos en lugar de referenciarlos?
    A) Cuando la relación es "uno a muchos" simple y se lee siempre junto, evitando consultas adicionales
    B) Nunca, hay que referenciar siempre entre colecciones
    C) Cuando la colección embebida supera los 16 MB
    D) Solo si además se usa SQL Server

**Tema 14: Sistemas de caché: Redis y memcached**

42. ¿Qué es Redis?
    A) Un ORM para SQL Server
    B) Un almacén de datos en memoria (clave-valor) usado habitualmente como caché distribuida
    C) Un navegador web
    D) Un framework web de front-end

43. ¿Cuál es la diferencia entre `IMemoryCache` e `IDistributedCache`?
    A) La primera guarda en la memoria del proceso actual; la segunda en un almacén externo compartible por varias instancias (Redis)
    B) La primera solo admite strings y la segunda solo objetos
    C) No hay ninguna diferencia
    D) La segunda solo funciona en Windows

44. ¿Por qué es importante definir una estrategia de invalidación de caché?
    A) Porque si los datos cambian y la caché no se actualiza, los usuarios ven información obsoleta
    B) Porque Redis no admite más de 10 claves
    C) Porque la caché ocupa disco duro indefinidamente
    D) Porque invalidar borra también la base de datos

**Tema 15: Transacciones, concurrencia e identificadores**

45. ¿Qué propiedades garantiza el acrónimo ACID en una transacción?
    A) Atomicidad, Consistencia, Aislamiento y Durabilidad
    B) Autenticación, Cifrado, Identificación y Despliegue
    C) Acceso, Concurrencia, Integridad y Disponibilidad
    D) Almacenamiento, Caché, Índices y Durabilidad

46. En concurrencia optimista, ¿qué ocurre si otro usuario modificó el registro entre tu lectura y tu escritura?
    A) Se sobrescribe el cambio sin más
    B) Se detecta el conflicto (por ejemplo, con `RowVersion`) y se cancela la operación avisando al usuario
    C) Se bloquea la tabla para siempre
    D) La base de datos elige la versión al azar

47. ¿Cuándo suele preferirse un `Guid` como clave primaria?
    A) Cuando se necesita generar el identificador sin guardar (útil en pruebas y entornos distribuidos)
    B) Cuando se quiere el número más pequeño posible
    C) Solo en tablas sin relaciones
    D) Nunca, siempre es peor que `IDENTITY`

**Tema 16: Autenticación JWT y BCrypt**

48. ¿Cuántas partes tiene un JWT y cuáles son?
    A) Una sola: el payload
    B) Tres: header, payload (claims) y firma
    C) Dos: usuario y contraseña
    D) Cabecera HTTP y cookie de sesión

49. ¿Por qué se usa BCrypt (o un hash similar con sal) para las contraseñas?
    A) Porque es más rápido que MD5
    B) Porque añade sal y coste de cálculo, haciendo inviable el ataque por fuerza bruta frente a hashes simples
    C) Porque comprime las contraseñas para ahorrar espacio
    D) Porque permite descifrar la contraseña cuando hace falta

50. En un flujo de login con JWT, ¿qué devuelve el servidor si las credenciales son correctas?
    A) Una sesión guardada en memoria del servidor
    B) Un token JWT firmado que el cliente envía después en la cabecera `Authorization`
    C) Una cookie obligatoria de sesión
    D) El hash de la contraseña en texto plano

---

## PARTE 2 (temas 17-32)

**Tema 17: Autorización: roles, claims y políticas**

51. ¿Qué hace `[Authorize(Roles = "Admin")]`?
    A) Permite el acceso solo a usuarios autenticados que tengan el rol `Admin`
    B) Crea el rol `Admin` en la base de datos
    C) Desautentica al usuario actual
    D) Inicia sesión automáticamente con rol `Admin`

52. ¿Cuál es la principal ventaja de las políticas de autorización frente a los roles simples?
    A) Las políticas son solo decorativas
    B) Permiten requisitos complejos combinados (claims, roles y requisitos personalizados con `IAuthorizationHandler`)
    C) No requieren que el usuario esté autenticado
    D) Solo funcionan con sesiones, no con JWT

53. ¿Qué son los *claims*?
    A) Peticiones HTTP enviadas al servidor
    B) Afirmaciones o datos del usuario (nombre, email, rol) incluidas en el token
    C) Cookies de sesión
    D) Cabeceras CORS

**Tema 18: Tiempo real con WebSockets y SignalR**

54. ¿Qué aporta SignalR respecto a usar WebSockets "a pelo"?
    A) Reimplementa TCP desde cero
    B) Abstrae la transportación (WebSockets, *long polling*...) y ofrece una API de alto nivel con reconexión automática
    C) Elimina la necesidad de un servidor
    D) Solo funciona con HTTP/2

55. En SignalR, si el servidor ejecuta `Clients.All.SendAsync("nuevoFunko", funko)`, ¿quién recibe el mensaje?
    A) Solo el cliente que abrió la conexión
    B) Todos los clientes conectados a ese Hub
    C) Solo los usuarios con rol administrador
    D) Nadie, hay que hacer polling para recibirlo

56. ¿Para qué sirven los grupos (`Groups`) en SignalR?
    A) Para dividir el código en carpetas
    B) Para enviar mensajes a subconjuntos de clientes (salas, canales, partidas)
    C) Para agrupar varios hubs en procesos distintos
    D) Para comprimir los mensajes enviados

57. Frente a un enfoque de polling repetido, ¿qué ventaja principal tiene la comunicación en tiempo real con SignalR?
    A) Consume menos recursos y reduce latencia: el servidor emite solo cuando hay novedad
    B) Es idéntica al polling, solo cambia el nombre
    C) Requiere que el cliente consulte cada segundo de forma indefinida
    D) No admite más de un cliente conectado

**Tema 19: Apis con GraphQL**

58. En GraphQL, ¿qué operación modifica los datos del servidor?
    A) Query
    B) Mutation
    C) Subscription
    D) Fetch

59. ¿Qué problema resuelve GraphQL frente a REST (*overfetching*)?
    A) El cliente puede pedir exactamente los campos que necesita en una sola petición
    B) Permite ejecutar SQL directamente desde el navegador
    C) Elimina la necesidad de autenticar al cliente
    D) Comprime los JSON a la mitad siempre

60. ¿Qué son las suscripciones en GraphQL?
    A) Un plan de pago del servicio
    B) Operaciones que mantienen una conexión abierta para recibir actualizaciones en tiempo real desde el servidor
    C) Un tipo de índice de base de datos
    D) Cabeceras HTTP de caché

**Tema 20: Almacenamiento de ficheros**

61. ¿Qué tipo se usa en ASP.NET Core para recibir un fichero subido en una petición?
    A) `string`
    B) `IFormFile`
    C) `FileInfo`
    D) `Uri`

62. En una subida de ficheros, ¿qué comprobaciones mínimas debes hacer antes de guardarla?
    A) Ninguna, el navegador ya valida el fichero
    B) Extensión o tipo MIME permitido y tamaño máximo
    C) Únicamente el nombre del fichero
    D) Que el usuario tenga JavaScript activado

63. ¿Dónde suele guardarse el fichero subido en un primer ejemplo simple?
    A) En una carpeta accesible del sitio (p. ej. `wwwroot/uploads`) o en un almacén externo (Azure Blob, S3), sirviéndolo como fichero estático
    B) Dentro de la base de datos en texto plano siempre
    C) En el fichero `Program.cs`
    D) En la memoria RAM de forma indefinida

**Tema 21: Email services**

64. ¿Qué protocolo se usa para enviar correos electrónicos desde la API?
    A) SMTP
    B) FTP
    C) IMAP
    D) POP3

65. ¿Por qué conviene enviar el email fuera del hilo de la petición (*fire-and-forget* / segundo plano)?
    A) Porque el SMTP no funciona con HTTPS
    B) Para no bloquear la respuesta al cliente mientras el servidor de correo responde
    C) Porque los emails solo se pueden enviar de noche
    D) Para reducir el tamaño del JSON de respuesta

66. ¿Para qué sirven las plantillas de email (`EmailTemplates`)?
    A) Reutilizar el HTML del mensaje (bienvenida, reset de contraseña) manteniendo un estilo consistente
    B) Son ficheros ejecutables (.exe) del servidor
    C) Sustituyen a los controladores de la API
    D) Encriptan el contenido del correo

**Tema 22: Tareas programadas**

67. ¿Qué clase se usa en ASP.NET Core para ejecutar trabajo en segundo plano mientras la aplicación está viva?
    A) `BackgroundService` (o `IHostedService`)
    B) `HttpContext`
    C) `ActionResult`
    D) `ClaimsPrincipal`

68. ¿Cuándo arranca un `BackgroundService`?
    A) Solo cuando llega una petición HTTP
    B) Al arrancar la aplicación, junto con el host
    C) Únicamente cuando se llama desde un endpoint concreto
    D) Nunca, es una interfaz vacía

69. ¿Cuál es un ejemplo típico de tarea programada en una API?
    A) Enviar un resumen de newslettre periódico o purgar registros caducados
    B) Pintar la interfaz gráfica del navegador
    C) Compilar el proyecto C#
    D) Traducir el HTML al idioma del usuario

**Tema 23: Optimización de servicios web**

70. ¿Por qué paginar las listas largas en vez de devolver todos los registros?
    A) Para reducir el tamaño de la respuesta y el trabajo del servidor, mejorando los tiempos de respuesta
    B) Porque las bases de datos no admiten más de 10 filas por consulta
    C) Para obligar al cliente a iniciar sesión
    D) Porque JSON no admite arrays con muchos elementos

71. ¿Qué hace el *rate limiting*?
    A) Limita el número de peticiones que un cliente puede hacer en un periodo de tiempo
    B) Limita el tamaño del disco duro
    C) Comprime los ficheros del servidor
    D) Controla la frecuencia del ventilador del servidor

72. ¿Qué aporta la compresión (gzip/brotli) en producción?
    A) Reduce el tamaño de las respuestas transmitidas por la red
    B) Cifra la comunicación con el cliente
    C) Acelera las consultas a la base de datos
    D) Elimina la necesidad de usar caché

73. ¿Para qué sirve un health check (`/health`)?
    A) Para que el equilibrador u orquestador sepa si la instancia está sana y puede seguir recibiendo tráfico
    B) Para medir el ancho de banda del usuario
    C) Para guardar los datos en caché
    D) Para documentar la API

**Tema 24: Documentación mediante Swagger y OpenAPI**

74. ¿Cuál es la relación entre OpenAPI y Swagger?
    A) OpenAPI es la especificación del contrato; Swagger es el conjunto de herramientas que lo implementan
    B) Son exactamente lo mismo
    C) Swagger es la base de datos y OpenAPI el cliente
    D) OpenAPI solo funciona con Java

75. ¿Cómo llegan las descripciones de los endpoints a Swagger desde tu código?
    A) Mediante comentarios XML en el código, que se habilitan en el `.csproj` (`GenerateDocumentationFile`)
    B) Se generan solos a partir de las consultas SQL
    C) Manualmente en un fichero PDF aparte
    D) Mediante cookies de sesión

76. En Swagger UI, ¿qué botón permite probar un endpoint directamente desde el navegador?
    A) "Try it out" (Probar)
    B) "Compile"
    C) "Deploy"
    D) "Commit"

**Tema 25: Perfiles y configuración**

77. ¿Qué contiene `launchSettings.json`?
    A) Perfiles de ejecución con URLs, puertos y variables de entorno para desarrollo
    B) La configuración de producción del servidor
    C) Las contraseñas de la base de datos
    D) El código fuente de los tests

78. ¿Para qué sirve seleccionar un perfil de lanzamiento en el IDE?
    A) Para arrancar la aplicación con las URLs y el entorno configurados en ese perfil
    B) Para publicar la aplicación en Azure automáticamente
    C) Para compilar el proyecto más rápido
    D) Para cambiar el código fuente según el perfil

79. ¿Cómo se establece el entorno activo de ASP.NET Core desde la línea de comandos o el servidor?
    A) Definiendo la variable de entorno `ASPNETCORE_ENVIRONMENT` (p. ej. `Production`)
    B) Editando `Program.cs` a mano cada vez
    C) Renombrando el fichero `.exe`
    D) Con la cabecera HTTP `X-Environment`

**Tema 26: Organización e infraestructuras de Program.cs**

80. ¿Qué patrón se recomienda para no alargar `Program.cs`?
    A) Métodos de extensión en clases estáticas (p. ej. `AddRepositories()`, `UseMiMiddleware()`) agrupados por concernimiento
    B) Dejar todo en un solo fichero aunque tenga más de mil líneas
    C) Mover el código de configuración a ficheros HTML
    D) Usar etiquetas `goto` para saltar entre secciones

81. ¿Dónde se registran los servicios del contenedor de inyección de dependencias?
    A) En `builder.Services`, antes de `builder.Build()`
    B) Después de `app.Run()`
    C) Únicamente en `appsettings.json`
    D) Dentro de los controladores

82. ¿Cuál es la convención de nombres de los métodos de configuración de ASP.NET Core?
    A) `AddXxx()` para registrar servicios y `UseXxx()`/`MapXxx()` para pipeline y endpoints
    B) `CreateXxx()` para todo
    C) `InitXxx()` para servicios y `RunXxx()` para endpoints
    D) No existe ninguna convención

**Tema 27: Logging y monitoreo**

83. ¿Cuál es el orden correcto de niveles de log de menor a mayor severidad?
    A) Trace → Debug → Information → Warning → Error → Critical
    B) Critical → Error → Warning → Information → Debug → Trace
    C) Debug → Trace → Critical → Error → Warning → Information
    D) Info → Warn → Fatal → Fine

84. ¿Qué ventaja aporta inyectar `ILogger<T>`?
    A) Registra eventos con la categoría igual al nombre de la clase, facilitando filtrar por componente
    B) Permite crear controladores nuevos
    C) Compila los logs en tiempo de ejecución
    D) Reemplaza a la base de datos de la aplicación

85. En Serilog, ¿qué es un *sink*?
    A) Un destino de salida de los logs (consola, fichero, Seq...)
    B) Un nivel de severidad
    C) Un middleware del pipeline
    D) Un formato obligatorio de exportación CSV

**Tema 28: Testing de servicios web**

86. En Moq, ¿qué comprueba `mock.Verify(s => s.Crear(It.IsAny<Funko>()), Times.Once)`?
    A) Que el método se llamó exactamente una vez
    B) Que el objeto real se creó en la base de datos
    C) Que el Swagger está actualizado
    D) Que la cobertura de código es del 100%

87. ¿Qué es Testcontainers?
    A) Una librería que levanta contenedores Docker reales (BD, Redis) de forma efímera para los tests
    B) Un contenedor que empaqueta el front-end
    C) Una alternativa al framework de tests NUnit
    D) Un registro público de imágenes Docker

88. ¿Cuál es la diferencia entre un test unitario y uno de integración?
    A) El unitario prueba unidades aislando dependencias con mocks; el integración prueba varias piezas juntas (p. ej. contra una BD real)
    B) El unitario siempre es más lento que el de integración
    C) El de integración no verifica resultados
    D) No hay diferencia práctica entre ambos

89. ¿Cuál es el objetivo de la cobertura de código?
    A) Medir qué porcentaje del código está ejercitado por los tests
    B) Comprimir el código fuente antes de publicarlo
    C) Cubrir los ficheros de log con permisos
    D) Documentar la API con Swagger

**Tema 29: Docker y despliegue**

90. ¿Cuál es la diferencia entre imagen y contenedor?
    A) La imagen es la plantilla de solo lectura; el contenedor es una instancia en ejecución de esa imagen
    B) El contenedor es la plantilla y la imagen está en ejecución
    C) Son exactamente lo mismo
    D) La imagen solo existe en Windows

91. ¿Por qué usar un Dockerfile multi-etapa con `sdk` para compilar y `aspnet` para ejecutar?
    A) Para que la imagen final no lleve el SDK, reduciendo tamaño y superficie de ataque
    B) Porque `aspnet` no puede ejecutar la aplicación
    C) Para poder usar Windows dentro de Linux
    D) No tiene ninguna ventaja, solo complica el fichero

92. ¿Para qué sirve `docker-compose`?
    A) Para definir y arrancar varios servicios (app, BD, Redis) con una sola configuración
    B) Para escribir código C#
    C) Para reemplazar a GitHub Actions
    D) Para compilar imágenes más rápido

**Tema 30: CQRS: command query responsibility segregation**

93. ¿Qué significa CQRS?
    A) Separar el modelo de lectura (*queries*) del modelo de escritura (*commands*)
    B) Crear Quitar Resultados
    C) Control de Queries y Request Seguros
    D) Un patrón de caché distribuida

94. En MediatR, ¿cómo se define una operación?
    A) Con un `IRequest<T>` y su `IRequestHandler<,>` correspondiente
    B) Con una clase `Controller`
    C) Con un fichero SQL
    D) Con un atributo `[Command]`

95. ¿Cuándo compensa CQRS frente a una arquitectura CRUD clásica?
    A) Cuando lecturas y escrituras tienen requisitos de rendimiento o modelo muy distintos y el dominio lo justifica
    B) Siempre, es obligatorio en todo proyecto
    C) Solo en aplicaciones de una sola página
    D) Cuando no hay base de datos

**Tema 31: API Gateway y microservicios**

96. ¿Qué es un API Gateway?
    A) Un punto único de entrada que enruta, agrega y aplica preocupaciones transversales (auth, límites) a los microservicios
    B) Un controlador más de la API principal
    C) Una base de datos de microservicios
    D) Un navegador especial para APIs

97. ¿Cuál es la principal ventaja de exponer un único Gateway al cliente?
    A) El cliente no necesita conocer la topología interna de servicios; el Gateway enruta y agrega por él
    B) Se elimina la necesidad de autenticación en el conjunto
    C) Los microservicios dejan de ser necesarios
    D) No hace falta red entre componentes

98. ¿Qué es YARP?
    A) Un *reverse proxy* de Microsoft que reenvía peticiones a los servicios de fondo
    B) Un cliente FTP
    C) Un ORM para bases de datos
    D) Un framework de front-end

**Tema 32: Resumen**

99. Un endpoint protegido con `[Authorize]` recibe una petición sin token o con un token caducado. ¿Qué código de estado devuelve la API y por qué?
    A) 401 Unauthorized: el cliente no está autenticado
    B) 403 Forbidden: el cliente está autenticado pero no tiene permisos suficientes
    C) 404 Not Found: el recurso solicitado no existe
    D) 400 Bad Request: la petición está mal formada

100. ¿Cuál de estas afirmaciones sobre la API de Funkos es correcta?
    A) El `DbContext` debe registrarse como Singleton para no repetir la conexión
    B) Los DTOs devuelven las entidades de EF Core tal cual para ahorrar los mapeos
    C) Los middleware se ejecutan en el orden en que se registran y el servidor valida el JWT en cada petición sin guardar sesión
    D) Swagger genera el contrato únicamente si la API está construida con controladores MVC

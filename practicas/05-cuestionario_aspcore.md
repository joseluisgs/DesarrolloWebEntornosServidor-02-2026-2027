### Preguntas de Investigación y Desarrollo (I+D) - ASP.NET Core

- [Preguntas de Investigación y Desarrollo (I+D) - ASP.NET Core](#preguntas-de-investigación-y-desarrollo-id---aspnet-core)
  - [I. Arquitectura y Diseño de Servicios (Preguntas 1-8)](#i-arquitectura-y-diseño-de-servicios-preguntas-1-8)
  - [II. Persistencia Avanzada (Entity Framework Core y MongoDB) (Preguntas 9-14)](#ii-persistencia-avanzada-entity-framework-core-y-mongodb-preguntas-9-14)
  - [III. Implementación y Configuración Avanzada (Preguntas 15-20)](#iii-implementación-y-configuración-avanzada-preguntas-15-20)
  - [IV. Arquitecturas Modernas (GraphQL y SignalR) (Preguntas 21-25)](#iv-arquitecturas-modernas-graphql-y-signalr-preguntas-21-25)

---

#### I. Arquitectura y Diseño de Servicios (Preguntas 1-8)

1. **Comparativa de Servicios Orientados a la Conexión vs. Sin Conexión:** Justifique, desde una perspectiva de I+D, por qué el protocolo HTTP es inherentemente sin estado y cómo esta característica simplifica la implementación del servidor y facilita la escalabilidad en aplicaciones ASP.NET Core, a pesar de las desventajas que presenta la falta de fiabilidad y control de flujo en la transferencia de datos.

2. **Decisión Arquitectónica (REST vs.  SOAP):** Un nuevo proyecto requiere una alta fiabilidad, seguridad integrada mediante WS-Security y soporte para transacciones complejas con ACID. Argumente por qué SOAP con WCF (o servicios equivalentes en . NET) podría ser considerado, citando sus ventajas sobre REST en estos aspectos específicos, a pesar de que SOAP es generalmente más lento y verboso. 

3. **Diseño de API RESTful en ASP.NET Core:** Explique los principios de la **Interfaz Uniforme** en REST. ¿Por qué se recomienda en I+D utilizar sustantivos en plural para los *endpoints* (ej. `/productos`) en controladores de ASP.NET Core Web API y qué implicaciones tiene esta convención para el uso de los atributos de enrutamiento (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`)?

4. **Gestión de Respuestas HTTP en ASP.NET Core:** En el diseño de una API REST usando ASP.NET Core, justifique la elección del código de estado **204 No Content** (usando `NoContent()`) para la operación DELETE, en lugar de `Ok()` (200 OK). ¿Qué implicaciones tiene esta elección para la eficiencia de la red y la semántica HTTP?

5. **Análisis de Seguridad y Protocolos (SignalR y WebSockets):** Describa el protocolo WebSockets y cómo SignalR en ASP.NET Core abstrae su implementación, resolviendo problemas de rendimiento y latencia en comparación con técnicas anteriores como *polling* o *long polling*. ¿Por qué SignalR es fundamental para aplicaciones que requieren comunicación bidireccional y en tiempo real?

6. **Principios de ASP.NET Core:** Explique la relación entre la **Inversión de Control (IoC)** y la **Inyección de Dependencias (DI)** en el contexto de ASP. NET Core, específicamente mediante el uso del contenedor de servicios (`IServiceCollection` y `IServiceProvider`). ¿Cómo facilita la DI la capacidad del equipo de I+D para realizar pruebas unitarias mediante el uso de mocks (ej. con Moq o NSubstitute)?

7. **Versionado de APIs en ASP.NET Core:** Argumente por qué el versionado de una API (ej. usando `[ApiVersion("1.0")]` con el paquete `Microsoft.AspNetCore.Mvc. Versioning`) es una práctica crucial en el desarrollo. ¿Qué riesgo principal de ruptura de contratos con clientes existentes intenta mitigar el versionado al realizar cambios en la API? 

8. **Patrón DTO y Seguridad en ASP.NET Core:** Explique el rol del patrón DTO (Data Transfer Object) en la arquitectura de un servicio ASP.NET Core Web API. ¿Cómo el uso de DTOs, especialmente en combinación con herramientas como AutoMapper, contribuye a la modularidad, facilita el ensamblaje de objetos y proporciona una capa de seguridad al evitar la exposición directa de las entidades de Entity Framework Core?

---

#### II. Persistencia Avanzada (Entity Framework Core y MongoDB) (Preguntas 9-14)

9. **Identificadores (GUID vs. Autoincrementales en EF Core):** En un escenario de desarrollo donde se requiere que los objetos sean creados y relacionados en la lógica de negocio o en pruebas unitarias **sin depender de la persistencia inmediata** en la base de datos (antes de `SaveChanges()`), justifique la elección del identificador **GUID (Guid)** sobre el Autoincremental (`IDENTITY`), destacando las implicaciones en la responsabilidad de generación y la facilidad para testing.

10. **Borrado Lógico vs. Físico en EF Core:** El equipo debe implementar una estrategia de eliminación de datos que mantenga la integridad referencial y permita la auditoría y recuperación de registros eliminados. Justifique la implementación del **Borrado Lógico** (mediante una propiedad `IsDeleted` y Query Filters globales en `OnModelCreating`) sobre el Borrado Físico para este requisito, detallando sus ventajas y el uso de `HasQueryFilter()`.

11. **Diseño de Relaciones Bidireccionales EF Core:** Describa el problema de la **recursión infinita** que surge al serializar relaciones bidireccionales de Entity Framework Core a JSON usando `System.Text.Json` o `Newtonsoft.Json`. Mencione al menos dos estrategias (`[JsonIgnore]`, configurar `ReferenceHandler. IgnoreCycles`, o usar DTOs) para resolver este problema sin afectar la persistencia de las entidades. 

12. **Mapeo de Herencia en EF Core:** Si se requiere que cada clase en una jerarquía de herencia se mapee a su propia tabla y que estas tablas se unan mediante claves foráneas para mantener la integridad relacional, ¿qué estrategia de herencia de EF Core (`UseTptMappingStrategy()` para Table-per-Type) debe seleccionarse en el método `OnModelCreating` del `DbContext`?

13. **Diseño NoSQL (Documentos Embebidos vs. Referencias en MongoDB. Driver):** Al trabajar con el driver oficial de MongoDB para .NET (`MongoDB.Driver`), justifique la decisión de utilizar **documentos embebidos** (clases anidadas con `[BsonElement]`) para modelar una relación "Uno a Muchos" simple (ej. un pedido y sus líneas de pedido). ¿Qué ventaja de rendimiento (reducción de JOINs o consultas adicionales) se busca con esta técnica en comparación con el uso de referencias entre colecciones?

14. **Pruebas de Repositorios MongoDB en . NET:** Explique las diferencias metodológicas entre el uso de **MongoDB en memoria** (usando `MongoClient` con un servidor local de prueba) y **TestContainers** (que levanta contenedores Docker de MongoDB real) para el testeo de repositorios.  ¿En qué escenarios de I+D se preferiría usar TestContainers para garantizar comportamiento idéntico a producción?

---

#### III. Implementación y Configuración Avanzada (Preguntas 15-20)

15. **Caché en Servicios ASP.NET Core:** Describa la funcionalidad de `IMemoryCache` y `IDistributedCache` (con implementaciones como Redis) en ASP.NET Core. ¿Por qué es fundamental registrar el servicio de caché en `Program.cs` usando `builder.Services.AddMemoryCache()` o `builder.Services.AddStackExchangeRedisCache()` para que estos componentes funcionen correctamente mediante inyección de dependencias?

16. **Criterios de Búsqueda Dinámicos en EF Core:** Para permitir que la capa de servicio reciba parámetros de filtrado opcionales y construya una consulta dinámica con LINQ que combine múltiples criterios (ej. `Where(p => p.Nombre.Contains(nombre) && p.Precio < precioMax)`), ¿qué técnicas de construcción dinámica de expresiones Lambda o el uso de bibliotecas como **DynamicLinq** o **Sieve** facilitan esta tarea en Entity Framework Core?

17. **Inicialización de Componentes con IHostedService:** Describa el uso de `IHostedService` y `BackgroundService` en ASP.NET Core para tareas de inicialización o tareas programadas. Si un servicio de almacenamiento necesita crear directorios o ejecutar limpieza de datos *después* de que todas las dependencias hayan sido inyectadas por el contenedor DI, ¿cómo garantiza `IHostedService` que estas operaciones se ejecuten en el momento apropiado del ciclo de vida de la aplicación?

18. **Gestión de Configuración por Entornos:** Explique cómo el uso del sistema de configuración de ASP.NET Core con archivos específicos por entorno (`appsettings.Development.json`, `appsettings.Production.json`) y la variable de entorno `ASPNETCORE_ENVIRONMENT` facilita el despliegue y el testeo en diferentes entornos (Desarrollo, Staging, Producción). ¿Cómo se establece el entorno activo al ejecutar la aplicación desde la línea de comandos o en un contenedor Docker?

19. **Seguridad y CORS en ASP. NET Core:** Explique la función del mecanismo de seguridad CORS (Cross-Origin Resource Sharing) configurado mediante `builder.Services.AddCors()` y `app.UseCors()` en el contexto de una aplicación de *frontend* (React, Angular, Vue) que consume una API *backend* ASP.NET Core desde un dominio diferente. ¿Qué riesgo de seguridad relacionado con solicitudes maliciosas entre sitios se mitiga al configurar correctamente las políticas CORS?

20. **Integridad de Datos y Validación con Data Annotations:** El equipo de desarrollo utiliza DTOs de entrada anotados con atributos de `System.ComponentModel.DataAnnotations`. ¿Cómo se utiliza el atributo `[ApiController]` en un controlador (que habilita la validación automática) en conjunto con las restricciones como `[Required]`, `[StringLength]`, `[Range]` para asegurar la integridad de los datos y retornar automáticamente `400 BadRequest` con `ModelState` antes de que lleguen a la capa de servicio?

---

#### IV. Arquitecturas Modernas (GraphQL y SignalR) (Preguntas 21-25)

21. **GraphQL y *Overfetching* con HotChocolate:** Defina el problema de *overfetching* en las APIs REST tradicionales de ASP.NET Core. ¿Cómo resuelve GraphQL (implementado con bibliotecas como **HotChocolate**) este problema mediante el uso de queries selectivas, y qué ventaja principal ofrece esto en el rendimiento de aplicaciones con recursos limitados como aplicaciones móviles o con ancho de banda restringido?

22. **Modelo de Tipado en GraphQL Schema:** GraphQL se basa en un esquema con tipado fuerte definido en lenguaje SDL (Schema Definition Language). Explique el significado de la sintaxis `[Producto!]!` en un esquema GraphQL implementado en una aplicación ASP.NET Core con HotChocolate, y qué implica en términos de obligatoriedad (non-null) tanto de la lista como de cada uno de sus elementos.

23. **Operaciones GraphQL con HotChocolate:** Describa las tres categorías principales de operaciones soportadas por GraphQL (`Query`, `Mutation`, `Subscription`) en el contexto de una implementación con HotChocolate en ASP.NET Core. ¿Cuál de ellas está destinada específicamente a modificar los datos del lado del servidor (equivalente a POST/PUT/DELETE en REST)?

24. **Suscripciones GraphQL y Comunicación en Tiempo Real:** Las suscripciones de GraphQL permiten recibir notificaciones en tiempo real mediante push de eventos. En una aplicación ASP.NET Core con HotChocolate, ¿qué protocolo de comunicación subyacente (WebSockets) debe estar configurado y habilitado en el pipeline de middleware (usando `app.UseWebSockets()`) para que estas suscripciones puedan funcionar correctamente y entregar información al cliente de forma bidireccional?

25. **Seguridad de la Comunicación (SSL/TLS en ASP.NET Core):** En entornos de producción, se requiere que toda la comunicación HTTP sea cifrada mediante HTTPS. Explique brevemente cómo se configura ASP.NET Core para utilizar SSL/TLS en Kestrel, mencionando la necesidad de certificados (archivos `.pfx` o desde Certificate Store), la configuración en `appsettings.json` bajo la sección `Kestrel:Endpoints: Https`, y el uso de `app.UseHttpsRedirection()` para redirigir automáticamente el tráfico HTTP a HTTPS.

---

# Cuestionario de Investigación y Desarrollo (I+D): Desarrollo de Servicios Web en .NET

**Instrucciones:** Responde cada pregunta de forma clara y concisa. Puedes usar ejemplos de código si es necesario.

## PARTE 1 (Temas 01-16)

1.  **Diseño REST de una API de Funkos:** A partir del recurso `Funko` (`Id`, `Nombre`, `Precio`, `Stock`, `Categoria`), diseña los endpoints de su API REST indicando verbo HTTP, ruta y código de estado de respuesta. ¿Por qué usas sustantivos en plural (`/api/funkos`) y qué restricción de REST respeta cada elección?

2.  **Minimal APIs frente a controladores:** Un equipo debate si construir la API con Minimal APIs o con controladores MVC. Argumenta qué enfoque elegiría para un microservicio pequeño con cinco endpoints y cuál para una API grande con validaciones, filtros y autorización compleja. ¿Qué ventaja aporta cada uno en su escenario?

3.  **Ciclos de vida de la inyección de dependencias:** Explica la diferencia entre `Singleton`, `Scoped` y `Transient`. Si un servicio inyecta un `DbContext`, ¿qué ciclo de vida debe registrarse y por qué? ¿Qué problemas de rendimiento o estado podría causar usar `Singleton` para un repositorio con `DbContext`?

4.  **Patrón Result frente a excepciones:** Compara la filosofía del patrón `Result<T>` con el manejo de excepciones. En un método `CrearFunko` de tu API, ¿qué devolverías ante un nombre duplicado y ante un fallo inesperado de la base de datos? Justifica cada decisión.

5.  **DTOs como capa de contrato:** Un endpoint devuelve directamente la entidad `Funko` de EF Core con sus relaciones. Enumera al menos tres problemas de esta decisión (seguridad, contrato, serialización) y explica cómo los resuelve el patrón DTO con mapeador y validador.

6.  **Configuración por entornos y secretos:** Explica el orden de carga de `appsettings.json`, `appsettings.{Entorno}.json` y variables de entorno en ASP.NET Core. ¿Dónde guardarías la cadena de conexión a la base de datos en producción para que no suba a GitHub? ¿Y la clave secreta de JWT?

7.  **Migraciones y borrado lógico:** Describe el flujo de trabajo con migraciones de EF Core (`Add-Migration`, `Update-Database`) cuando el modelo cambia. Si la legislación exige conservar los registros borrados para auditoría, ¿cómo implementarías el borrado lógico con `HasQueryFilter()` y qué cuidados requiere en las consultas?

8.  **SQL frente a NoSQL para FunkoApp:** Tu tienda necesita consultas transaccionales con integridad referencial entre pedidos y stock. Justifica la elección de PostgreSQL. ¿En qué situación concreta de este proyecto cambiarías a MongoDB y qué perderías en el camino?

9.  **Estrategia de caché con Redis:** Diseña una estrategia de caché para el listado de funkos: ¿qué datos cachearías, con cuánto TTL y cómo invalidarías la caché cuando se crea o modifica un funko? ¿Por qué no cachearía la operación de compra de un pedido?

10. **Transacciones y concurrencia:** Explica las propiedades ACID con un ejemplo de compra de un funko con stock limitado. Si dos clientes compran el último ejemplar al mismo tiempo, ¿cómo lo resolverías con concurrencia optimista (`RowVersion`)? ¿Qué respuesta HTTP devolverías al segundo cliente?

## PARTE 2 (Temas 17-32)

11. **JWT: escalabilidad y riesgos:** Un JWT es *stateless* y por eso escala bien. Explica cómo valida el servidor el token en cada petición sin consultar la base de datos de sesiones. ¿Qué problema de "revocación" surge si un token se filtra antes de caducar y qué mitigaciones existen?

12. **Autorización: roles frente a políticas:** Tu API necesita que solo los administradores borren funkos y que los vendedores solo editen los suyos. ¿Por qué no bastan los roles solos? Diseña la solución con políticas y claims, mencionando quién evalúa la política y en qué punto del pipeline.

13. **Tiempo real con SignalR:** Comparando el *polling* periódico con SignalR, explica por qué la segunda opción consume menos recursos. Si quieres que solo los clientes suscritos a la categoría "Star Wars" reciban las novedades, ¿qué elemento de SignalR usarías y cómo enviarías el mensaje desde el servidor?

14. **GraphQL frente a REST:** Define los problemas de *overfetching* e *underfetching* que sufren las APIs REST. ¿En qué tipo de pantalla de tu aplicación de funkos causaría problemas REST y cómo lo resolvería GraphQL con una *query* selectiva? ¿Qué operación de GraphQL usarías para actualizar el stock?

15. **Optimización para producción:** Tu API sufre un ataque de miles de peticiones por minuto al listado de funkos. Ordena estas soluciones por prioridad y justifica cada una: *rate limiting*, caché Redis, compresión gzip y paginación. ¿Qué *health check* configurarías para que el balanceador retire una instancia dañada?

16. **Contrato OpenAPI como verificación:** Explica la diferencia entre OpenAPI (especificación) y Swagger (herramientas). ¿Cómo protegerías el contrato de tu API frente a cambios incompatibles por accidente? Describe una estrategia de test de contrato que compare la especificación publicada con la generada por la aplicación.

17. **Estrategia de testing completa:** Define cuándo escribes tests unitarios, de integración y E2E en tu API. ¿Por qué Testcontainers da más confianza que una base de datos en memoria? ¿Qué piezas de tu API probarías con `WebApplicationFactory` y cuáles NO se pueden probar bien sin levantar Docker?

18. **Docker multi-etapa y seguridad:** Explica qué gana tu imagen si compila con `mcr.microsoft.com/dotnet/sdk` y ejecuta con `mcr.microsoft.com/dotnet/aspnet`. ¿Qué riesgos de seguridad desaparecen al quitar el SDK de la imagen final? ¿Cómo pasarías la configuración de producción al contenedor sin recompilarlo?

19. **CQRS: cuándo compensa:** CQRS añade complejidad (más tipos, handlers, modelos separados). Argumenta en qué escenario de una tienda justificaría esa complejidad (p. ej. catálogo con miles de lecturas y pocas escrituras) y en cuál no. ¿Qué papel juega MediatR en la implementación concreta?

20. **API Gateway y microservicios:** Tu sistema está partido en Auth, Productos y Pedidos. Explica qué gana el cliente al exponer un único API Gateway (YARP) y qué preocupaciones transversales conviene centralizar en él. ¿Qué desventajas o riesgos añade un punto único de entrada?

---

**Instrucciones para el alumnado:**

Estas preguntas requieren **investigación, análisis y justificación técnica**. Se espera que:

1. **Investigues** en la documentación oficial de .NET, artículos técnicos y recursos educativos.
2. **Comparen** diferentes enfoques y tecnologías.
3. **Justifiques** tus respuestas con argumentos técnicos sólidos.
4. **Relaciones** los conceptos con principios de diseño de software (SOLID, arquitectura, rendimiento, seguridad).

**Formato de respuesta esperado:**

- Respuestas de **desarrollo** (no solo definiciones).
- Incluir **ventajas/desventajas**, **casos de uso** y **ejemplos concretos**.
- Mencionar **trade-offs** (compromisos) cuando sea aplicable.
- Citar fuentes cuando sea necesario.

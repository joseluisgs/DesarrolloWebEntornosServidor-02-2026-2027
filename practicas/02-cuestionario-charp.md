### Cuestionario de Investigación y Desarrollo para C# . NET (10 Preguntas)

---

1. **Justificación Arquitectónica:** Cuando se diseña una aplicación de servidor de alto rendimiento y con múltiples funcionalidades en . NET, ¿cuál es la principal ventaja de elegir una **Arquitectura basada en Microservicios** sobre una **Arquitectura Monolítica**, y cómo esta elección refuerza el **Principio de Responsabilidad Única (SRP)** de SOLID en el diseño de los componentes?  Considere aspectos como el despliegue independiente, la escalabilidad y el mantenimiento.

2. **Trade-off de Asincronía:** Una aplicación ASP.NET Core necesita realizar numerosas llamadas a una API externa, lo que implica una alta latencia de E/S. Justifique la necesidad de utilizar el modelo **async/await** en lugar de un enfoque síncrono tradicional. ¿Qué recurso crítico del sistema se busca liberar para mejorar la escalabilidad y el rendimiento, especialmente en un servidor web bajo alta carga?

3. **Flujos Reactivos:** En el contexto de la Programación Reactiva con **System.Reactive (Rx.NET)**, defina la diferencia fundamental entre un **Flujo Frío (*Cold Observable*)** y un **Flujo Caliente (*Hot Observable*)**. Si estuviera desarrollando un sistema de notificaciones en tiempo real donde los suscriptores tardíos deben perder los mensajes pasados (como un stream de eventos en vivo), ¿cuál de los dos tipos de flujo sería apropiado implementar y qué tipo de `Subject<T>` utilizaría? 

4. **Manejo de Errores Predictivo:** Explique la filosofía del **Railway Oriented Programming (ROP)** que utiliza tipos como `Result<T>` o `Result<T, TError>` de la librería **CSharpFunctionalExtensions**, en contraste con el manejo de errores mediante **Excepciones** tradicionales. ¿Cómo logra ROP mantener explícito el "camino de error" como parte del tipo de retorno sin interrumpir el flujo de control del programa, y qué ventajas ofrece esto en términos de composición de funciones?

5. **Modernización de Acceso a Datos:** En un proyecto . NET que actualmente utiliza **ADO.NET** tradicional con `SqlConnection` y `SqlCommand` para acceder a bases de datos relacionales, ¿cuál es el argumento principal para migrar a un **ORM** como **Entity Framework Core**? Mencione qué mecanismo o componente de EF Core se utiliza para resolver el "desfase objeto-relacional" (Object-Relational Impedance Mismatch) entre las filas SQL y los objetos C#, y cómo esto se relaciona con LINQ. 

6. **Principios de Diseño y Mutabilidad:** Analice la diferencia entre una **clase estándar** de C# (potencialmente mutable con propiedades públicas con `set`) y un **Record** (C# 9.0+, inmutable por defecto). En el contexto de la **Programación Funcional**, ¿por qué se recomienda utilizar **Records** para modelar los datos transferidos a través de APIs (DTOs), y cómo se relaciona esto con el principio de **Inmutabilidad** y la igualdad basada en valores?

7. **Testing y Aislamiento:** Al escribir pruebas unitarias en . NET utilizando **NUnit** o **xUnit**, ¿por qué es indispensable recurrir a una librería de **Mocking** como **Moq**? Describa cómo el uso de objetos simulados (*mocks*) permite a un desarrollador adherirse al patrón **AAA (Arrange, Act, Assert)**, garantizar el **aislamiento** de la unidad de código bajo prueba, y verificar las interacciones con las dependencias mediante el método `Verify()`.

8. **Optimización de Despliegue:** Una aplicación .NET genera una imagen Docker final de gran tamaño al incluir el SDK completo.  Explique el beneficio de utilizar un **Dockerfile multi-etapa** (usando imágenes `mcr.microsoft.com/dotnet/sdk` para compilación y `mcr.microsoft.com/dotnet/aspnet` para runtime) para la dockerización y el despliegue de esta aplicación en comparación con un Dockerfile simple.  ¿Qué recurso se reduce significativamente en el contenedor de producción final gracias a este enfoque, y por qué es importante para el rendimiento y la seguridad?

9. **Arquitectura de Comunicación:** Compare el modelo de comunicación de una API **RESTful** construida con ASP.NET Core (basada en el protocolo HTTP sin estado y request-response) con la tecnología **SignalR** (que utiliza WebSockets). Si el requerimiento de I+D es crear un servicio que transmita datos en tiempo real de forma continua y bidireccional (sin que el cliente lo solicite constantemente mediante polling), ¿cuál de las dos tecnologías sería técnicamente más adecuada y por qué?  Mencione el concepto de "conexión persistente". 

10. **Seguridad y Estado en HTTP:** Un **JSON Web Token (JWT)** es fundamental para la seguridad en APIs REST de ASP.NET Core. ¿Cuál es el propósito principal de la **Firma (*Signature*)** en un JWT (relacionado con integridad y autenticidad), y cómo contribuye el token en su conjunto a que la arquitectura RESTful sea **sin estado (*stateless*)** y altamente escalable, eliminando la necesidad de almacenar sesiones en el servidor?  Explique cómo el servidor valida el JWT en cada petición sin consultar una base de datos de sesiones.

---

**Instrucciones para el alumnado:**

Estas preguntas requieren **investigación, análisis y justificación técnica**. Se espera que: 

1. **Investiguen** en la documentación oficial de . NET, artículos técnicos y recursos educativos. 
2. **Comparen** diferentes enfoques y tecnologías. 
3. **Justifiquen** sus respuestas con argumentos técnicos sólidos.
4. **Relacionen** los conceptos con principios de diseño de software (SOLID, arquitectura, rendimiento, seguridad).

**Formato de respuesta esperado:**

- Respuestas de **desarrollo** (no solo definiciones).
- Incluir **ventajas/desventajas**, **casos de uso**, y **ejemplos concretos**. 
- Mencionar **trade-offs** (compromisos) cuando sea aplicable.
- Citar fuentes cuando sea necesario. 

---

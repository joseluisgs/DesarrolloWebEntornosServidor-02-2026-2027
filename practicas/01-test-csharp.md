### Cuestionario Tipo Test de C# .  NET (100 Preguntas)

- [Cuestionario Tipo Test de C# .  NET (100 Preguntas)](#cuestionario-tipo-test-de-c---net-100-preguntas)
  - [I. Aspectos Fundamentales, POO y Estructuras del Lenguaje](#i-aspectos-fundamentales-poo-y-estructuras-del-lenguaje)
  - [II. Colecciones y LINQ](#ii-colecciones-y-linq)
  - [III.  E/S y Ficheros](#iii--es-y-ficheros)
  - [IV. Bases de Datos](#iv-bases-de-datos)
  - [V. Concurrencia y Asincronía](#v-concurrencia-y-asincronía)
  - [VI. Programación Reactiva (System.Reactive)](#vi-programación-reactiva-systemreactive)
  - [VII. Railway Oriented Programming (ROP)](#vii-railway-oriented-programming-rop)
  - [VIII. HTTP REST y JWT](#viii-http-rest-y-jwt)
  - [IX. Testing, Mocking y Herramientas](#ix-testing-mocking-y-herramientas)

---

#### I. Aspectos Fundamentales, POO y Estructuras del Lenguaje

1. ¿Cuál es la función principal del **Common Language Runtime (CLR)** en el proceso de ejecución de un programa . NET?
   A.  Compilar el código fuente (`.cs`) a código máquina. 
   B. Traducir el código IL (Intermediate Language) a código máquina específico del sistema mediante compilación JIT (Just-In-Time).
   C. Gestionar la interfaz de usuario. 
   D. Ejecutar consultas SQL en la base de datos.

2. En el proceso de compilación de C#, ¿qué formato intermedio genera el compilador `csc` o `dotnet build`?
   A. Código fuente. 
   B. Código máquina nativo.
   C. IL (Intermediate Language) o MSIL. 
   D. Bytecode de Java.

3. ¿Cuál de los siguientes es un tipo de dato por valor (value type) en C# que **no puede ser nulo** por defecto?
   A. `string`.
   B. `int?` (nullable int).
   C. `int`.
   D. `object`.

4. ¿Qué modificador de acceso restringe la visibilidad de un miembro a **solo dentro de la misma clase**?
   A. `public`.
   B. `protected`.
   C. `private`.
   D. `internal`.

5. Si se aplica el modificador `sealed` a una **clase**, ¿cuál es la restricción que se impone?
   A.  El objeto no puede ser modificado (inmutabilidad).
   B. Impide que sea heredada por otras clases.
   C.  Todos sus métodos deben ser abstractos.
   D. Solo puede tener una instancia (Singleton).

6. ¿Cuál es el propósito principal de los **constructores** en una clase?
   A. Leer los valores de las propiedades.
   B.  Devolver un valor. 
   C. Inicializar las propiedades y campos de un objeto al momento de su creación.
   D.  Sobrescribir los métodos de la clase base.

7. El principio de **Encapsulamiento** en POO se logra al agrupar datos y métodos y ocultar la implementación interna, principalmente mediante:
   A. El uso de herencia.
   B. La sobrecarga de constructores.
   C.  Modificadores de acceso (`private`) y propiedades con *getters* y *setters*. 
   D. El uso de la interfaz `IDisposable`.

8. ¿Qué significa el principio de **Polimorfismo** ("muchas formas")?
   A. Que una clase debe tener una sola responsabilidad.
   B.  Que los objetos de diferentes clases relacionadas por herencia pueden ser tratados como objetos de una clase base común.
   C.  Que el código está cerrado a modificación y abierto a extensión.
   D. La ocultación de la implementación interna de una clase.

9. Una **interfaz** es un "contrato" en C#.  ¿Qué debe hacer una clase que implementa una interfaz?
   A. Heredar de otra clase abstracta.
   B. Ser declarada como `sealed`.
   C. Proporcionar una implementación para todos sus miembros abstractos.
   D. Utilizar el patrón *Builder*.

10. ¿Qué representa un tipo **Record** (C# 9.0+)?
    A. Un objeto que puede ser nulo.
    B. Un tipo de referencia especial, inmutable por defecto y conciso, diseñado para ser un contenedor de datos con igualdad basada en valores.
    C. Una función de orden superior. 
    D. Una implementación de `List<T>`.

11. ¿Cuál es el propósito principal de los **tipos anulables** (`? `) en C# (ej. `int?`)?
    A. Gestionar la concurrencia.
    B.  Prevenir errores de `InvalidCastException`.
    C. Permitir que tipos por valor (value types) puedan representar la ausencia de valor (`null`).
    D. Forzar el uso de `try-catch`.

12. ¿Qué excepción se lanza cuando se intenta acceder al valor de un tipo anulable (`int?`) usando `.Value` sin verificar si tiene valor?
    A. `NullReferenceException`.
    B. `IOException`.
    C. `InvalidOperationException`.
    D. `ArgumentNullException`.

13. ¿Qué garantiza el bucle **`do-while`**?
    A. Que la condición se evalúa al principio. 
    B. Que solo se usa para colecciones.
    C.  Que el bloque de código se ejecuta al menos una vez.
    D. Que se ejecuta de forma asíncrona.

14. En C#, ¿qué estructura se usa para asegurar que los recursos que implementan `IDisposable` (archivos, conexiones DB) se liberen automáticamente?
    A. El bloque `finally`.
    B. El bloque `catch`.
    C. `using` statement (o declaración).
    D. `Task. Run`.

15. ¿Qué tipo de excepción en .  NET **deriva de `Exception`** y generalmente representa errores recuperables que deben manejarse?
    A. `SystemException`.
    B. `ApplicationException` (obsoleto, pero conceptualmente correcto).
    C. Excepciones personalizadas que heredan de `Exception`.
    D. `OutOfMemoryException`.

16. Para crear una excepción personalizada en C#, ¿de qué clase debe heredar?
    A.  `Error`.
    B. `Throwable`.
    C. `Exception`.
    D. `SystemException`.

17. ¿Cuál es la principal ventaja de usar **genéricos** en C# (ej. `List<T>`)?
    A. Permitir la herencia múltiple de clases.
    B. Proporcionar seguridad de tipos en tiempo de compilación y reutilización de código sin pérdida de rendimiento por boxing. 
    C. Asegurar que los datos sean mutables.
    D. Forzar la implementación de `IDisposable`.

18. En una restricción de tipo genérico, ¿qué indica la sintaxis `where T : IComparable`?
    A. Que T es un tipo concreto `IComparable`.
    B. Que T debe implementar la interfaz `IComparable`.
    C. Que T es cualquier tipo. 
    D. Que T debe ser una clase. 

19. ¿Dónde se debe colocar la declaración de restricción de tipo genérico (`where T : ... `) en la sintaxis de un **método genérico**?
    A. Antes del tipo de retorno.
    B.  Después de la lista de parámetros del método.
    C. Dentro de los parámetros de tipo. 
    D. En la declaración de la clase.

20. ¿Qué principio SOLID establece que una clase debe tener **una, y solo una, razón para cambiar**?
    A.  Principio Abierto/Cerrado (OCP).
    B. Principio de Sustitución de Liskov (LSP).
    C. Principio de Responsabilidad Única (SRP).
    D. Principio de Inversión de Dependencias (DIP).

21. ¿Qué principio SOLID promueve que los módulos de alto nivel no deben depender de los módulos de bajo nivel, sino que ambos deben depender de abstracciones?
    A. Principio de Sustitución de Liskov (LSP).
    B. Principio de Segregación de Interfaces (ISP).
    C. Principio de Inversión de Dependencias (DIP).
    D. Principio Abierto/Cerrado (OCP).

22. En C#, ¿cuál es el propósito de las **propiedades** (properties)?
    A. Modificar el valor de un campo privado directamente.
    B. Inicializar los campos del objeto. 
    C. Proporcionar acceso controlado (get/set) a los campos privados de forma encapsulada.
    D. Implementar la lógica de negocio. 

23. ¿Cuál es la diferencia principal entre un **struct** y una **class** en C#?
    A.  Los structs son tipos por referencia, las clases son tipos por valor.
    B. Los structs son tipos por valor, las clases son tipos por referencia.
    C. Los structs no pueden tener métodos. 
    D. Las clases no pueden ser anulables.

24. ¿Cuál de los siguientes es un patrón de **comportamiento**?
    A. Singleton.
    B. Adapter.
    C. Observer.
    D. Factory Method.

25. ¿Qué patrón arquitectónico divide la aplicación en capas (presentación, negocio, datos) mejorando la modularidad? 
    A. Microservicios.
    B.  Monolítica.
    C. Arquitectura de capas (Layered Architecture).
    D. Singleton.

---

#### II. Colecciones y LINQ

26. ¿Qué interfaz de colección **no permite elementos duplicados** y es ideal para manejar un conjunto de valores únicos?
    A. `IList<T>`.
    B. `IDictionary<TKey, TValue>`.
    C. `ISet<T>` (implementado por `HashSet<T>`).
    D. `List<T>`.

27. ¿Qué implementación de `IList<T>` es ideal para el acceso rápido por índice?
    A. `LinkedList<T>`.
    B. `SortedSet<T>`.
    C. `Dictionary<TKey, TValue>`.
    D. `List<T>`.

28. ¿Qué estructura de datos almacena pares de **clave-valor** donde cada clave debe ser única?
    A. `IList<T>`.
    B. `ISet<T>`.
    C. `IDictionary<TKey, TValue>`.
    D. `Array`.

29. Un Tipo de Dato Abstracto (TDA) es una estructura de datos que define un conjunto de operaciones y propiedades, pero ¿qué oculta? 
    A. La interfaz. 
    B. La implementación interna.
    C. Las expresiones lambda.
    D. Los *getters* y *setters*. 

30. En LINQ, ¿qué tipo de operación **no ejecuta** la lógica inmediatamente, sino que la prepara (evaluación perezosa o deferred execution)?
    A. Operación de ejecución inmediata (ej. `ToList()`).
    B. Operación de agregación (ej. `Count()`).
    C. Operación de consulta (ej. `Where()`, `Select()`).
    D. Operación de ordenamiento terminal. 

31. ¿Qué operación de LINQ se utiliza para transformar cada elemento de una secuencia en un nuevo valor?
    A. `Where()`.
    B. `Aggregate()`.
    C. `Select()`.
    D. `Count()`.

32. ¿Qué operación de LINQ es análoga a la cláusula `WHERE` en SQL?
    A. `Select(...)`.
    B. `Where(...)`.
    C. `GroupBy(...)`.
    D. `Aggregate()`.

33. ¿Cuál es el propósito del método **`AsParallel()`** en PLINQ (Parallel LINQ)?
    A. Forzar la ejecución secuencial.
    B.  Dividir la colección en múltiples subtareas que se ejecutan simultáneamente en diferentes núcleos del procesador.
    C.  Asegurar la inmutabilidad de los datos originales.
    D. Reemplazar la necesidad de `Task`.

34. ¿Qué es una **expresión lambda** en C#?
    A. Un método de clase abstracta. 
    B. La sintaxis de C# para crear una función anónima (ej. `x => x * 2`).
    C. Un tipo de excepción no comprobada.
    D. El tipo de retorno de un `Task<T>`.

35. Una **función pura** en programación funcional se caracteriza por que, dadas las mismas entradas, siempre produce la misma salida y: 
    A. Lanza una excepción.
    B. Modifica variables globales o estado externo.
    C. No causa efectos secundarios (side effects).
    D. Siempre devuelve un tipo anulable.

36. ¿Qué operación de LINQ se utiliza para combinar los elementos en un único resultado acumulado?
    A. `Select()`.
    B. `ToList()`.
    C. `Aggregate()`.
    D. `ForEach()`.

37. ¿Cuál es un ejemplo de **operación de cortocircuito** en LINQ?
    A. `Select()`.
    B. `Where()`.
    C. `Any()`.
    D. `OrderBy()`.

38. ¿Cuál es el tipo de delegado predefinido de C# utilizado para **evaluar un booleano** (predicado)?
    A. `Action<T>`.
    B. `Func<T, TResult>`.
    C. `Predicate<T>` o `Func<T, bool>`.
    D. `Func<T>`.

39. ¿Qué se promueve con el enfoque de la inmutabilidad en la programación funcional?
    A. Modificar las variables en su lugar.
    B. Crear nuevos objetos con los datos modificados en lugar de cambiar los existentes.
    C. Aumentar el uso de excepciones no controladas.
    D. Uso exclusivo de `ArrayList`.

40. El uso de LINQ permite escribir código **declarativo**, lo que significa que el código se enfoca en: 
    A. El "cómo" se ejecutan los pasos (imperativo).
    B. El "qué" se quiere lograr. 
    C. La inyección de dependencias.
    D. El manejo de hilos manualmente.

---

#### III.  E/S y Ficheros

41. ¿Cuál es el formato de intercambio de datos más utilizado que se basa en pares clave-valor y es legible por humanos?
    A. XML. 
    B.  BSON.
    C. CSV.
    D. JSON. 

42. Para manipular archivos de texto en . NET, ¿qué clase se recomienda para operaciones simples como leer o escribir todo el contenido? 
    A. `StreamReader` / `StreamWriter`.
    B. `File` (métodos estáticos como `File.ReadAllText`).
    C. `BinaryReader`.
    D. `Console.ReadLine`.

43. En el manejo de ficheros, ¿qué método de la clase `File` permite procesar el contenido línea por línea retornando un `IEnumerable<string>`?
    A. `File.ReadAllText`.
    B. `File.WriteAllLines`.
    C. `File.ReadLines`.
    D. `File.Delete`.

44. ¿Qué biblioteca/namespace se utiliza comúnmente en .  NET para la **serialización** y **deserialización** de JSON?
    A. `NUnit`.
    B. `Dapper`.
    C. `System.Text.Json` o `Newtonsoft.Json` (Json.NET).
    D. `Refit`.

45. ¿Cuál es el formato de intercambio de datos que representa información en forma tabular, con campos separados por un delimitador (ej. coma)?
    A. JSON.
    B. XML.
    C. JWT.
    D. CSV. 

46. ¿Qué sintaxis introducida en C# 11+ se utiliza para crear cadenas multilínea sin usar caracteres de escape, usando tres comillas dobles?
    A.  Concatenación con `+`.
    B. `string.Format()`.
    C. Raw string literals (usando `"""`).
    D. Uso de `StringBuilder`.

47. ¿Qué clase de C# se utiliza típicamente para la entrada de datos por consola?
    A. `Console. WriteLine`.
    B. `File`.
    C. `Console. ReadLine`.
    D. `StreamReader`.

---

#### IV. Bases de Datos

48. ¿Qué significa el acrónimo CRUD? 
    A.  Compile, Read, Update, Deploy.
    B. Connect, Run, Use, Database.
    C. Create, Read, Update, Delete.
    D. Change, Run, Update, Disconnect.

49. ¿Cuál es la principal ventaja de usar **parámetros** en las consultas SQL con ADO.NET (ej. `SqlCommand` con `@parametro`)?
    A. Solo se utiliza para operaciones `SELECT`.
    B. Es más lento pero más legible.
    C. Ayuda a prevenir ataques de inyección SQL y mejora el rendimiento al reutilizar planes de ejecución.
    D. Permite realizar consultas reactivas.

50. El patrón **Repository** se utiliza para:
    A. Gestionar la lógica de negocio. 
    B. Abstraer el acceso a los datos y proporcionar una interfaz para interactuar con la capa de persistencia.
    C. Simular objetos para pruebas.
    D.  Definir la arquitectura de microservicios.

51. **Entity Framework Core** es un ORM para .  NET que simplifica el acceso a datos mediante: 
    A. Anotaciones de pruebas como `[Test]`.
    B. Atributos HTTP como `[HttpGet]`.
    C. El mapeo de clases C# a tablas de base de datos y el uso de LINQ para consultas.
    D. La interfaz `IDisposable`.

52. En Entity Framework Core, ¿qué clase hereda tu `DbContext` y representa la sesión con la base de datos?
    A. `SqlConnection`.
    B. `IDbConnection`.
    C. `DbContext`.
    D. `IHttpClientFactory`.

53. El método `SaveChangesAsync()` en Entity Framework Core se utiliza para:
    A.  Registrar el `DbContext`.
    B. Persistir (guardar) todos los cambios realizados en el contexto a la base de datos.
    C. Ejecutar migraciones. 
    D. Gestionar la conexión a la base de datos.

54. En .  NET, ¿qué librería proporciona un **driver nativo** para trabajar con MongoDB?
    A. Entity Framework Core.
    B.  Dapper.
    C. `MongoDB.Driver`.
    D. ADO.NET.

55. Al usar **Dapper** (un micro-ORM), ¿qué ventaja principal ofrece sobre ADO.NET tradicional?
    A. Abstracción completa de SQL.
    B. Mapeo automático de resultados de consultas SQL a objetos C# sin necesidad de código manual de mapeo.
    C.  Generación automática de migraciones.
    D. Solo funciona con MongoDB.

56. ¿Qué librería proporciona un acceso **reactivo** y no bloqueante a bases de datos relacionales en . NET?
    A. ADO.NET.
    B.  Entity Framework Core (tradicional).
    C. No hay un equivalente directo maduro como R2DBC en Java; se usa async/await con EF Core o bibliotecas específicas.
    D. Refit.

57. En Entity Framework Core, ¿qué método se utiliza para aplicar filtros (equivalente al `WHERE` en SQL) en una consulta LINQ?
    A. `Include()`.
    B. `Where()`.
    C. `Select()`.
    D. `Add()`.

58. **ADO.NET** es el conjunto de clases en .NET para interactuar con qué tipo de bases de datos?
    A.  Bases de datos NoSQL orientadas a documentos exclusivamente.
    B. Bases de datos en memoria únicamente.
    C. Bases de datos relacionales y otras fuentes de datos.
    D.  Archivos CSV. 

59. El `using` statement es una buena práctica en bases de datos para asegurar la liberación de qué recurso?
    A. El repositorio.
    B. El ORM.
    C. La conexión a la base de datos (`IDbConnection`) y otros objetos `IDisposable`.
    D. El `DbContext` sin cerrarlo (incorrecto, el `DbContext` también debe disponerse).

60. ¿Cuál es el principal desafío que los ORMs como Entity Framework Core buscan mitigar entre el modelo de datos de una base de datos relacional y el modelo de objetos de C#?
    A. El problema de la inyección SQL.
    B. El desfase objeto-relacional (Object-Relational Impedance Mismatch).
    C. La falta de un mapper. 
    D. La necesidad de `Task. Run`.

---

#### V. Concurrencia y Asincronía

61. ¿Qué es la **Programación Asíncrona** en .NET?
    A. La ejecución de tareas de manera secuencial, una después de la otra.
    B. Un paradigma que permite que las tareas se ejecuten sin bloquear el hilo actual, usando `async` y `await`.
    C. El uso exclusivo de `Thread`.
    D. La capacidad de ejecutar múltiples procesos. 

62. ¿En qué escenario surge principalmente la necesidad de utilizar `async` y `await`?
    A. En la manipulación de tipos primitivos.
    B. En escenarios de **E/S (Entrada/Salida) intensiva** (red, archivos, base de datos) para no bloquear el hilo. 
    C.  Cuando se usan `record`.
    D. Al implementar interfaces. 

63. ¿Cuál es la palabra clave en C# que marca un método como asíncrono?
    A.  `await`.
    B. `Task`.
    C. `async`.
    D. `thread`.

64. ¿Qué clase de .  NET representa una operación asíncrona que **devuelve un valor**?
    A. `Task`.
    B. `Thread`.
    C. `Task<TResult>`.
    D. `IObservable<T>`.

65. La palabra clave **`await`** en C# se utiliza para: 
    A. Iniciar una nueva tarea.
    B.  Bloquear el hilo actual indefinidamente.
    C.  Suspender la ejecución del método `async` hasta que la operación asíncrona se complete, sin bloquear el hilo.
    D. Crear un `IObservable`.

66. ¿Qué ocurre cuando el código llama a `task.Result` o `task.Wait()` de forma síncrona en una tarea asíncrona? 
    A.  Continúa la ejecución del código inmediatamente.
    B. **Bloquea el hilo actual** hasta que la tarea termine, pudiendo causar deadlocks.
    C. Se lanza una `TaskCanceledException`.
    D. La tarea se mueve a otro hilo.

67. ¿Qué es una **Sección Crítica** en concurrencia?
    A. Un método que lanza una excepción controlada.
    B. Un bloque de código que accede a un recurso compartido y que debe ser ejecutado de forma exclusiva por un solo hilo a la vez.
    C. Un `using` statement.
    D. Un patrón de diseño de creación.

68. ¿Qué mecanismo en C# se utiliza para sincronizar el acceso a una sección crítica usando una palabra clave?
    A. `async`.
    B. `lock`.
    C. `await`.
    D. `using`.

69. ¿Qué colección del namespace `System.Collections.Concurrent` está diseñada para ser utilizada en entornos multi-hilo de forma segura?
    A. `List<T>`.
    B. `Dictionary<TKey, TValue>`.
    C. `ConcurrentDictionary<TKey, TValue>`.
    D. `SortedList<TKey, TValue>`.

70. ¿Qué método de la clase `Task` se utiliza para esperar a que múltiples tareas se completen? 
    A. `Task. Delay`.
    B. `Task.Run`.
    C. `Task.WhenAll` o `Task.WaitAll`.
    D. `Task. Yield`.

---

#### VI. Programación Reactiva (System.Reactive)

71. La Programación Reactiva se basa en el concepto de: 
    A. Ejecución secuencial y sincrónica. 
    B. La gestión de **flujos de datos asincrónicos** y la propagación de cambios mediante observables.
    C. El patrón *Builder*. 
    D. La inmutabilidad de los tipos primitivos exclusivamente.

72. En Rx. NET (System.Reactive), ¿qué componente es la **fuente de datos** que emite una secuencia de elementos?
    A. `IObserver<T>`.
    B. `IScheduler`.
    C. `IObservable<T>`.
    D. `Task<T>`.

73. ¿Qué significa que un flujo reactivo sea **"Cold" (frío)**?
    A. La emisión de datos ya ha comenzado antes de cualquier suscripción.
    B.  El trabajo no comienza hasta que alguien se suscribe, y cada suscriptor recibe su propia secuencia completa de datos.
    C. El flujo utiliza un `Subject`.
    D. El flujo está optimizado para cálculos intensivos exclusivamente.

74. En Rx.NET, ¿qué interfaz representa al **consumidor** de los datos emitidos por un `IObservable<T>`?
    A. `IEnumerable<T>`.
    B. `IObserver<T>`.
    C. `Task<T>`.
    D. `Func<T>`.

75. ¿Qué operador reactivo es crucial para **encadenar llamadas asíncronas** al transformar un elemento en un nuevo `IObservable<T>` y luego aplanar los resultados?
    A. `Select`.
    B. `Where`.
    C. `Zip`.
    D. `SelectMany` (equivalente a `flatMap`).

76. En Rx.NET, ¿qué componente controla en qué **hilo o contexto de sincronización** se llevan a cabo las operaciones del flujo? 
    A. `IObserver<T>`.
    B. `Subject<T>`.
    C. `IScheduler`.
    D. `Task<T>`.

77. ¿Cuál es el propósito de un `IScheduler` como `TaskPoolScheduler`?
    A. Ejecutar operaciones en el hilo de UI.
    B. Ejecutar operaciones en el ThreadPool.
    C. Bloquear el hilo principal. 
    D. Solo para pruebas unitarias.

78. En Rx.NET, ¿qué tipo de `Subject` emite a los nuevos suscriptores el **último valor** emitido y todos los valores futuros?
    A. `Subject<T>`.
    B. `ReplaySubject<T>`.
    C. `BehaviorSubject<T>`.
    D. `AsyncSubject<T>`.

79. ¿Qué librería es el equivalente de RxJava para . NET?
    A.  LINQ. 
    B. Entity Framework. 
    C. System.Reactive (Rx.NET).
    D. Refit.

80. En Rx.NET, un `Subject<T>` actúa a la vez como:
    A. `Task` y `Thread`.
    B. `IObservable<T>` y `IObserver<T>`.
    C. `List` y `Array`.
    D. `async` y `await`.

---

#### VII. Railway Oriented Programming (ROP)

81. ¿Cuál es el problema que ROP busca mitigar en el manejo de errores?
    A. La imposibilidad de usar `try-catch`.
    B. La interrupción del flujo normal de ejecución y la verbosidad al manejar errores con excepciones.
    C. La inmutabilidad de los datos.
    D. La necesidad de `Task<T>`.

82. Para implementar ROP en C#, ¿qué librería popular proporciona el tipo `Result<T>` o `Result<T, TError>`?
    A. `System.Linq`.
    B. `Newtonsoft.Json`.
    C. `CSharpFunctionalExtensions`.
    D. `System.Reactive`.

83. En el encadenamiento de operaciones ROP con `Result`, ¿qué método permite que si ocurre un fallo, el error se propague automáticamente? 
    A. `Map()`.
    B. `Match()`.
    C. `Bind()` (equivalente a `flatMap`).
    D. `Tap()`.

84. ¿Qué método de `Result` recibe dos funciones (una para éxito y otra para error) para "fusionar" los dos posibles caminos en un único valor final?
    A. `Map()`.
    B. `Match()`.
    C. `Bind()`.
    D. `Ensure()`.

85. ¿Cuál es un beneficio clave de ROP en aplicaciones C#?
    A. Garantiza la ejecución secuencial en un solo hilo.
    B.  El manejo de errores es explícito y parte del tipo de retorno, mejorando la legibilidad y el flujo del código.
    C.  Reemplaza todas las excepciones por completo.
    D. Permite la mutabilidad de los datos.

---

#### VIII. HTTP REST y JWT

86. ¿Cuál es el protocolo fundamental y sin estado que se utiliza para la creación de APIs REST?
    A. TCP.
    B. WebSocket.
    C. HTTP (HyperText Transfer Protocol).
    D. SMTP.

87. ¿Qué verbo HTTP se utiliza para **actualizar parcialmente** un recurso en el servidor?
    A. `PUT`.
    B. `POST`.
    C. `GET`.
    D. `PATCH`.

88. ¿Qué código de respuesta HTTP indica que la solicitud ha tenido éxito y **se ha creado un nuevo recurso**? 
    A. 200 OK.
    B. 204 No Content.
    C.  201 Created.
    D. 404 Not Found.

89. **Refit** en .NET permite definir la API como una **interfaz de C#** utilizando:
    A. Entity Framework. 
    B. Atributos HTTP (`[Get]`, `[Post]`, etc.).
    C. El patrón Repository.
    D. `try-catch`.

90. ¿Qué atributo de Refit se utiliza en el parámetro de un método para enviar el objeto serializado en el **cuerpo de la petición**? 
    A. `[AliasAs]`.
    B. `[Query]`.
    C. `[Body]`.
    D. `[Header]`.

91. Para usar `HttpClient` de forma eficiente en . NET Core/5+, ¿qué servicio se debe registrar para gestionar instancias de `HttpClient` y evitar problemas como el agotamiento de sockets?
    A. `DbContext`.
    B. `IHttpClientFactory`.
    C. `IServiceCollection`.
    D. `Task. Run`.

92. Un JSON Web Token (JWT) se compone de tres partes.  ¿Cuál de ellas contiene las **reclamaciones** (*claims*) sobre la entidad (ej.  el usuario)?
    A. Header (Encabezado).
    B. Signature (Firma).
    C. Payload (Carga Útil).
    D. Clave Secreta.

93. ¿Cuál es el propósito de la **Firma** en un JWT?
    A.  Incluir el algoritmo de cifrado.
    B. Codificar los *claims* en Base64Url.
    C. Verificar la integridad del token y asegurar que no ha sido alterado.
    D. Definir el tipo de token.

94. En una API REST de ASP.NET Core, si el servidor encuentra una situación que no sabe cómo manejar, ¿qué código de error HTTP debe devolver?
    A. 400 Bad Request.
    B.  404 Not Found.
    C.  500 Internal Server Error. 
    D. 401 Unauthorized.

95. ¿Qué tecnología permite la comunicación **bidireccional y en tiempo real** entre cliente y servidor a través de una conexión persistente en .  NET?
    A. API REST.
    B. gRPC.
    C. WebSockets (ej. SignalR).
    D. HTTP/1.1.

---

#### IX. Testing, Mocking y Herramientas

96. ¿Cuál es uno de los *frameworks* más populares en . NET para escribir y ejecutar **pruebas unitarias**? 
    A.  Moq.
    B. MSBuild.
    C. XML Documentation. 
    D. NUnit, xUnit, o MSTest.

97. ¿Qué patrón de estructura de pruebas unitarias divide cada prueba en preparación, acción y verificación del resultado?
    A. SRP (Single Responsibility Principle).
    B. ROP (Railway Oriented Programming).
    C. CRUD (Create, Read, Update, Delete).
    D. AAA (Arrange, Act, Assert).

98. ¿Qué librería se utiliza en .  NET para crear **objetos simulados (*mocks*)** para aislar el código bajo prueba?
    A. NUnit.
    B. MSBuild.
    C. Moq.
    D.  Refit.

99. En Moq, ¿qué método se utiliza para **verificar** que el código bajo prueba interactuó con su dependencia (*mock*) llamando a un método específico?
    A.  `Setup()`.
    B. `Assert. AreEqual()`.
    C. `Assert.Throws()`.
    D. `Verify()`.

100. ¿Qué herramienta de compilación y gestión de proyectos se utiliza en el ecosistema moderno de .NET (.  NET Core / .  NET 5+)?
     A. Docker.
     B. XML Documentation.
     C. `dotnet` CLI y archivos `.csproj` (MSBuild).
     D. Testcontainers.

---


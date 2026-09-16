# De C# a Java: Un Tutorial Detallado

- [De C# a Java: Un Tutorial Detallado](#de-c-a-java-un-tutorial-detallado)
    - [C#](#c)
    - [Java](#java)
  - [Manejo de Tipos Nulos](#manejo-de-tipos-nulos)
    - [C#](#c-1)
    - [Java](#java-1)
  - [Programación Orientada a Objetos: Visibilidad, Propiedades y Métodos, Herencia, Interfaces](#programación-orientada-a-objetos-visibilidad-propiedades-y-métodos-herencia-interfaces)
    - [C#](#c-2)
    - [Java](#java-2)
  - [Programación Funcional](#programación-funcional)
    - [C#](#c-3)
    - [Java](#java-3)
  - [Programación con Colecciones y Streams](#programación-con-colecciones-y-streams)
    - [C#](#c-4)
    - [Java](#java-4)
  - [Excepciones y Control de Errores](#excepciones-y-control-de-errores)
    - [C#](#c-5)
    - [Java](#java-5)
  - [Lectura y Escritura de Ficheros de Texto](#lectura-y-escritura-de-ficheros-de-texto)
    - [C#](#c-6)
    - [Java](#java-6)
  - [Acceso a Bases de Datos Relacionales](#acceso-a-bases-de-datos-relacionales)
    - [C#](#c-7)
    - [Java](#java-7)
  - [Consideraciones Adicionales](#consideraciones-adicionales)
    - [Coroutines en Kotlin vs. Async/Await en C#](#coroutines-en-kotlin-vs-asyncawait-en-c)
    - [Ejemplo de Async/Await en C#](#ejemplo-de-asyncawait-en-c)
    - [Ejemplo de Threads en Java](#ejemplo-de-threads-en-java)


### C\#

En C\#, la unidad básica es la **clase**. Aquí tienes un ejemplo de un programa básico:

```csharp
using System;

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        // Declaración de variables
        const string readOnly = "Este es un valor inmutable"; // `const` para inmutabilidad
        int mutable = 10; // Variable mutable
    }
}
```

### Java

En Java, la unidad básica es también la **clase**. El ejemplo equivalente en un archivo Java (`.java`) es:

```java
public class Main {
    public static void main(String[] args) {
        System.out.println("Hello, World!");

        // Declaración de variables
        final String readOnly = "Este es un valor inmutable"; // `final` para inmutabilidad
        int mutable = 10; // Variable mutable
    }
}
```

-----

## Manejo de Tipos Nulos

### C\#

C\# tiene un sistema de **tipos nulos más robusto** a partir de C\# 8.0, lo que ayuda a prevenir `NullReferenceExceptions`.

```csharp
string? nullableString = null; // Puede ser nulo
int? length = nullableString?.Length; // Safe call (llamada segura)
int defaultLength = nullableString?.Length ?? 0; // Operador Elvis (??)
```

### Java

En Java, no hay un sistema de tipos nulos similar. Debes **verificar manualmente** si una referencia es nula para evitar excepciones.

```java
String nullableString = null; // Puede ser nulo
Integer length = (nullableString != null) ? nullableString.length() : null; // Safe call
int defaultLength = (nullableString != null) ? nullableString.length() : 0; // Operador Elvis
```

-----

## Programación Orientada a Objetos: Visibilidad, Propiedades y Métodos, Herencia, Interfaces

### C\#

```csharp
public class Animal {
    private string name = "Animal";

    public string Name {
        get { return name; }
        set { name = value; }
    }

    public virtual void Sound() {
        Console.WriteLine("Some sound");
    }
}

public class Dog : Animal {
    public override void Sound() {
        Console.WriteLine("Bark");
    }
}

public interface IPet {
    void Play();
}

public class Cat : Animal, IPet {
    public override void Sound() {
        Console.WriteLine("Meow");
    }

    public void Play() {
        Console.WriteLine("Playing with ball");
    }
}
```

### Java

```java
class Animal {
    private String name = "Animal";

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void sound() {
        System.out.println("Some sound");
    }
}

class Dog extends Animal {
    @Override
    public void sound() {
        System.out.println("Bark");
    }
}

interface Pet {
    void play();
}

class Cat extends Animal implements Pet {
    @Override
    public void sound() {
        System.out.println("Meow");
    }

    @Override
    public void play() {
        System.out.println("Playing with ball");
    }
}
```

-----

## Programación Funcional

### C\#

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class FunctionalExample {
    public static void Main(string[] args) {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
        List<int> filtered = numbers.Where(n => n % 2 == 0).ToList();
        Console.WriteLine(string.Join(", ", filtered));
        
        // Definición de lambdas
        Action<string> printMessage = message => Console.WriteLine(message);
        printMessage("Hello, World!");
        
        Func<int, int, int> sum = (a, b) => a + b;
        Console.WriteLine(sum(5, 3));
        
        // Función de orden superior
        List<int> greaterThanTwo = FilterUsingPredicate(numbers, n => n > 2);
        Console.WriteLine(string.Join(", ", greaterThanTwo));
    }

    public static List<T> FilterUsingPredicate<T>(List<T> list, Predicate<T> predicate) {
        return list.Where(predicate.Invoke).ToList();
    }
}
```

### Java

```java
import java.util.Arrays;
import java.util.List;
import java.util.function.Consumer;
import java.util.function.BiFunction;
import java.util.function.Predicate;
import java.util.stream.Collectors;

public class FunctionalExample {
    public static void main(String[] args) {
        List<Integer> numbers = Arrays.asList(1, 2, 3, 4, 5);
        List<Integer> filtered = numbers.stream()
                                         .filter(n -> n % 2 == 0)
                                         .collect(Collectors.toList());
        System.out.println(filtered);
        
        // Definición de lambdas
        Consumer<String> printMessage = message -> System.out.println(message);
        printMessage.accept("Hello, World!");
        
        BiFunction<Integer, Integer, Integer> sum = (a, b) -> a + b;
        System.out.println(sum.apply(5, 3));
        
        // Función de orden superior
        List<Integer> greaterThanTwo = filterUsingPredicate(numbers, n -> n > 2);
        System.out.println(greaterThanTwo);
    }

    public static <T> List<T> filterUsingPredicate(List<T> list, Predicate<T> predicate) {
        return list.stream().filter(predicate).collect(Collectors.toList());
    }
}
```

-----

## Programación con Colecciones y Streams

### C\#

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class CollectionsExample {
    public static void Main(string[] args) {
        List<string> fruits = new List<string> { "Apple", "Banana", "Cherry" };
        List<string> upperCaseFruits = fruits.Select(fruit => fruit.ToUpper()).ToList();
        Console.WriteLine(string.Join(", ", upperCaseFruits));
    }
}
```

### Java

```java
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

public class CollectionsExample {
    public static void main(String[] args) {
        List<String> fruits = Arrays.asList("Apple", "Banana", "Cherry");
        List<String> upperCaseFruits = fruits.stream()
                                             .map(String::toUpperCase)
                                             .collect(Collectors.toList());
        System.out.println(upperCaseFruits);
    }
}
```

-----

## Excepciones y Control de Errores

### C\#

```csharp
using System;

public class ExceptionExample {
    public static void Main(string[] args) {
        try {
            int result = Divide(4, 0);
            Console.WriteLine(result);
        } catch (DivideByZeroException) {
            Console.WriteLine("Cannot divide by zero");
        }
    }

    public static int Divide(int a, int b) {
        return a / b;
    }
}
```

### Java

```java
public class ExceptionExample {
    public static void main(String[] args) {
        try {
            int result = divide(4, 0);
            System.out.println(result);
        } catch (ArithmeticException e) {
            System.out.println("Cannot divide by zero");
        }
    }

    public static int divide(int a, int b) {
        return a / b;
    }
}
```

-----

## Lectura y Escritura de Ficheros de Texto

### C\#

```csharp
using System;
using System.IO;

public class FileExample {
    public static void Main(string[] args) {
        string text = File.ReadAllText("input.txt");
        Console.WriteLine(text);

        File.WriteAllText("output.txt", "Hello, World");
    }
}
```

### Java

```java
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class FileExample {
    public static void main(String[] args) throws IOException {
        String text = new String(Files.readAllBytes(Paths.get("input.txt")));
        System.out.println(text);

        Files.write(Paths.get("output.txt"), "Hello, World".getBytes());
    }
}
```

-----

## Acceso a Bases de Datos Relacionales

### C\#

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

public class DatabaseExample {
    public static void Main(string[] args) {
        string connectionString = "Server=localhost;Database=testdb;User Id=username;Password=password;";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            connection.Open();
            using (SqlCommand command = new SqlCommand("SELECT * FROM users", connection)) {
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        Console.WriteLine(reader["name"]);
                    }
                }
            }
        }
    }
}
```

### Java

```java
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;

public class DatabaseExample {
    public static void main(String[] args) {
        try (Connection connection = DriverManager.getConnection("jdbc:mysql://localhost:3306/testdb", "username", "password")) {
            Statement statement = connection.createStatement();
            ResultSet resultSet = statement.executeQuery("SELECT * FROM users");
            while (resultSet.next()) {
                System.out.println(resultSet.getString("name"));
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
```

-----

## Consideraciones Adicionales

### Coroutines en Kotlin vs. Async/Await en C\#

  - Java usa **threads** para programación concurrente, mientras que C\# utiliza el modelo de programación asíncrona **`async/await`**.

### Ejemplo de Async/Await en C\#

```csharp
using System;
using System.Threading.Tasks;

public class AsyncExample {
    public static async Task Main(string[] args) {
        await Task.Run(async () => {
            await Task.Delay(1000);
            Console.WriteLine("World!");
        });
        Console.WriteLine("Hello");
    }
}
```

### Ejemplo de Threads en Java

```java
public class ThreadExample {
    public static void main(String[] args) throws InterruptedException {
        Thread thread = new Thread(() -> {
            try {
                Thread.sleep(1000);
                System.out.println("World!");
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        });
        thread.start();
        System.out.println("Hello");
        thread.join();
    }
}
```
# 📖 Fundamentos de C# para Desarrolladores Java

## Tabla de Contenidos
- [Introducción](#introducción)
- [Similitudes y Diferencias Clave](#similitudes-y-diferencias-clave)
- [Sintaxis Básica](#sintaxis-básica)
- [Tipos de Datos](#tipos-de-datos)
- [Variables y Constantes](#variables-y-constantes)
- [Operadores](#operadores)
- [Estructuras de Control](#estructuras-de-control)
- [Funciones y Métodos](#funciones-y-métodos)
- [Propiedades](#propiedades)
- [Null Safety](#null-safety)

## Introducción

C# y Java son lenguajes muy similares, ambos orientados a objetos, con gestión automática de memoria y fuertemente tipados. Si conoces Java, aprenderás C# rápidamente.

### Tabla Comparativa de Alto Nivel

| Aspecto | Java | C# |
|---------|------|-----|
| **Creador** | Sun Microsystems (Oracle) | Microsoft |
| **Plataforma** | JVM (multiplataforma) | .NET (multiplataforma desde .NET Core) |
| **Paradigma** | OOP, funcional (desde Java 8) | OOP, funcional, imperativo |
| **Archivo** | `.java` | `.cs` |
| **Compilado** | Bytecode `.class` | IL (Intermediate Language) `.dll` |
| **Gestión memoria** | Garbage Collector | Garbage Collector |
| **Tipado** | Fuerte, estático | Fuerte, estático |

## Similitudes y Diferencias Clave

### ✅ Similitudes

```java
// JAVA - Ambos lenguajes son muy similares en sintaxis básica
public class MiClase {
    private int numero = 10;
    
    public void metodo() {
        if (numero > 5) {
            System.out.println("Mayor que 5");
        }
    }
}
```

```csharp
// C# - Sintaxis prácticamente idéntica
public class MiClase
{
    private int numero = 10;
    
    public void Metodo()
    {
        if (numero > 5)
        {
            Console.WriteLine("Mayor que 5");
        }
    }
}
```

### ⚠️ Diferencias Principales

| Concepto | Java | C# |
|----------|------|-----|
| **Convención nombres** | camelCase para métodos | PascalCase para métodos |
| **Propiedades** | getters/setters manuales | Properties automáticas |
| **Null safety** | `Optional<T>` | `T?` (nullable reference types) |
| **Strings** | `String` | `string` (lowercase) |
| **Arrays** | `int[]` o `int array[]` | `int[]` |
| **Boxing** | Automático | Automático |
| **Genéricos** | Type erasure | Reified (información en runtime) |
| **Extensiones** | No nativo | Métodos de extensión |

## Sintaxis Básica

### Programa Principal

#### Java
```java
public class Main {
    public static void main(String[] args) {
        System.out.println("Hola Mundo");
    }
}
```

#### C# (Tradicional)
```csharp
using System;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hola Mundo");
    }
}
```

#### C# (Top-level statements - C# 9+)
```csharp
// Sin clase ni método Main explícito
Console.WriteLine("Hola Mundo");
```

### Imports vs Using

#### Java
```java
import java.util.List;
import java.util.ArrayList;
import java.util.*;  // Wildcard import
```

#### C#
```csharp
using System.Collections.Generic;
using System.Linq;
using System;  // No hay wildcard, se especifica el namespace
```

### Paquetes vs Namespaces

#### Java
```java
package com.ejemplo.proyecto;

public class MiClase {
    // ...
}
```

#### C#
```csharp
namespace Com.Ejemplo.Proyecto
{
    public class MiClase
    {
        // ...
    }
}

// O con file-scoped namespace (C# 10+)
namespace Com.Ejemplo.Proyecto;

public class MiClase
{
    // ...
}
```

## Tipos de Datos

### Tipos Primitivos

| Java | C# | Descripción | Tamaño |
|------|-----|------------|--------|
| `byte` | `byte` | Entero con signo | 8 bits |
| `short` | `short` | Entero con signo | 16 bits |
| `int` | `int` | Entero con signo | 32 bits |
| `long` | `long` | Entero con signo | 64 bits |
| `float` | `float` | Punto flotante | 32 bits |
| `double` | `double` | Punto flotante | 64 bits |
| `boolean` | `bool` | Booleano | 1 bit |
| `char` | `char` | Carácter Unicode | 16 bits |
| - | `decimal` | Decimal de alta precisión | 128 bits |

### Diferencias Importantes

#### Java
```java
boolean estaActivo = true;     // boolean en minúsculas
Boolean objetoBoolean = true;  // Boolean como objeto (wrapper)
```

#### C#
```csharp
bool estaActivo = true;        // bool en minúsculas
Boolean objetoBoolean = true;  // Boolean es alias de System.Boolean
```

### Tipos de Referencia

#### Java
```java
String texto = "Hola";         // String con S mayúscula
Integer numero = 10;           // Wrapper class
List<String> lista = new ArrayList<>();
```

#### C#
```csharp
string texto = "Hola";         // string en minúsculas (alias de System.String)
int numero = 10;               // int ya es un tipo de valor
List<string> lista = new List<string>();
// O con var (inferencia de tipos)
var lista2 = new List<string>();
```

## Variables y Constantes

### Declaración de Variables

#### Java
```java
// Variables
int numero = 10;
String texto = "Hola";
final int CONSTANTE = 100;  // final para constantes

// Inferencia de tipos (Java 10+)
var lista = new ArrayList<String>();
var numero2 = 42;  // Inferido como int
```

#### C#
```csharp
// Variables
int numero = 10;
string texto = "Hola";
const int CONSTANTE = 100;  // const para constantes en compile-time
readonly int constante2 = 100;  // readonly para constantes en runtime

// Inferencia de tipos
var lista = new List<string>();
var numero2 = 42;  // Inferido como int
```

### Const vs Readonly vs Final

| Java `final` | C# `const` | C# `readonly` |
|-------------|-----------|--------------|
| Runtime | Compile-time | Runtime |
| Instancia o estática | Siempre estática | Instancia o estática |
| Puede ser mutable (objetos) | Solo tipos primitivos/strings | Puede ser mutable (objetos) |

```csharp
public class Ejemplo
{
    const int CONSTANTE_COMPILACION = 100;      // Evaluada en compile-time
    readonly int constante_runtime;             // Evaluada en constructor
    static readonly int CONSTANTE_ESTATICA = 50; // Estática y runtime
    
    public Ejemplo(int valor)
    {
        constante_runtime = valor;  // Se puede asignar en constructor
    }
}
```

## Operadores

### Operadores Básicos (Idénticos)

```csharp
// Aritméticos: + - * / %
// Comparación: == != > < >= <=
// Lógicos: && || !
// Asignación: = += -= *= /= %=
```

### Operadores Específicos de C#

#### Operador Null-Coalescing

```java
// Java - Ternario o Optional
String texto = valor != null ? valor : "default";
String texto2 = Optional.ofNullable(valor).orElse("default");
```

```csharp
// C# - Operador ?? (null-coalescing)
string texto = valor ?? "default";

// Operador ??= (null-coalescing assignment)
texto ??= "default";  // Asigna solo si texto es null
```

#### Operador Null-Conditional

```java
// Java - Verificación manual o Optional
String nombre = persona != null ? persona.getNombre() : null;
String nombre2 = Optional.ofNullable(persona)
                        .map(p -> p.getNombre())
                        .orElse(null);
```

```csharp
// C# - Operador ?. (null-conditional)
string? nombre = persona?.Nombre;

// Encadenamiento
string? ciudad = persona?.Direccion?.Ciudad;
```

## Estructuras de Control

### If-Else (Idéntico)

```csharp
if (condicion)
{
    // código
}
else if (otraCondicion)
{
    // código
}
else
{
    // código
}
```

### Switch

#### Java (Tradicional)
```java
switch (valor) {
    case 1:
        System.out.println("Uno");
        break;
    case 2:
        System.out.println("Dos");
        break;
    default:
        System.out.println("Otro");
}
```

#### Java (Switch Expressions - Java 14+)
```java
String resultado = switch (valor) {
    case 1 -> "Uno";
    case 2 -> "Dos";
    default -> "Otro";
};
```

#### C# (Tradicional)
```csharp
switch (valor)
{
    case 1:
        Console.WriteLine("Uno");
        break;
    case 2:
        Console.WriteLine("Dos");
        break;
    default:
        Console.WriteLine("Otro");
        break;
}
```

#### C# (Switch Expressions - C# 8+)
```csharp
string resultado = valor switch
{
    1 => "Uno",
    2 => "Dos",
    _ => "Otro"  // _ es el default
};
```

### Bucles

```csharp
// For loop (idéntico)
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}

// While (idéntico)
while (condicion)
{
    // código
}

// Do-while (idéntico)
do
{
    // código
} while (condicion);

// Foreach
foreach (var item in lista)
{
    Console.WriteLine(item);
}
```

## Funciones y Métodos

### Declaración de Métodos

#### Java
```java
public class Calculadora {
    // Método de instancia
    public int sumar(int a, int b) {
        return a + b;
    }
    
    // Método estático
    public static int multiplicar(int a, int b) {
        return a * b;
    }
    
    // Método con sobrecarga
    public double sumar(double a, double b) {
        return a + b;
    }
}
```

#### C#
```csharp
public class Calculadora
{
    // Método de instancia
    public int Sumar(int a, int b)
    {
        return a + b;
    }
    
    // Método estático
    public static int Multiplicar(int a, int b)
    {
        return a * b;
    }
    
    // Método con sobrecarga
    public double Sumar(double a, double b)
    {
        return a + b;
    }
    
    // Expression-bodied member (C# 6+)
    public int Restar(int a, int b) => a - b;
}
```

### Parámetros

#### Java
```java
// Parámetros por valor (primitivos)
public void metodo(int numero) { }

// Parámetros por referencia (objetos)
public void metodo(String texto) { }

// Varargs
public void metodo(String... argumentos) { }
```

#### C#
```csharp
// Parámetros por valor (default)
public void Metodo(int numero) { }

// Parámetros por referencia
public void Metodo(ref int numero) { }

// Parámetros de salida
public void Metodo(out int resultado) { resultado = 10; }

// Parámetros con nombre
Metodo(numero: 10, texto: "Hola");

// Parámetros opcionales
public void Metodo(int numero, string texto = "default") { }

// Params (como varargs)
public void Metodo(params string[] argumentos) { }
```

## Propiedades

### Getters y Setters vs Properties

#### Java - Getters/Setters Manuales
```java
public class Persona {
    private String nombre;
    private int edad;
    
    // Getter
    public String getNombre() {
        return nombre;
    }
    
    // Setter
    public void setNombre(String nombre) {
        this.nombre = nombre;
    }
    
    public int getEdad() {
        return edad;
    }
    
    public void setEdad(int edad) {
        if (edad >= 0) {
            this.edad = edad;
        }
    }
}

// Uso
Persona p = new Persona();
p.setNombre("Juan");
String nombre = p.getNombre();
```

#### C# - Properties Automáticas
```csharp
public class Persona
{
    // Propiedad auto-implementada
    public string Nombre { get; set; }
    
    // Propiedad con lógica personalizada
    private int _edad;
    public int Edad
    {
        get { return _edad; }
        set
        {
            if (value >= 0)
                _edad = value;
        }
    }
    
    // Propiedad de solo lectura
    public string NombreCompleto { get; }
    
    // Propiedad calculada (expression-bodied)
    public bool EsMayorDeEdad => Edad >= 18;
    
    // Propiedad con inicializador
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}

// Uso
var p = new Persona();
p.Nombre = "Juan";  // Se ve como un campo pero es un método
string nombre = p.Nombre;
```

## Null Safety

### Java - Opcional y @Nullable

#### Java (Pre-Optional)
```java
public class Ejemplo {
    public String obtenerNombre(Persona persona) {
        if (persona != null && persona.getNombre() != null) {
            return persona.getNombre();
        }
        return "Desconocido";
    }
}
```

#### Java (Con Optional - Java 8+)
```java
public class Ejemplo {
    public Optional<String> obtenerNombre(Persona persona) {
        return Optional.ofNullable(persona)
                      .map(p -> p.getNombre());
    }
    
    // Uso
    String nombre = obtenerNombre(persona)
                   .orElse("Desconocido");
}
```

### C# - Nullable Reference Types

#### C# (Pre-Nullable Reference Types)
```csharp
public class Ejemplo
{
    public string ObtenerNombre(Persona persona)
    {
        if (persona != null && persona.Nombre != null)
        {
            return persona.Nombre;
        }
        return "Desconocido";
    }
}
```

#### C# (Con Nullable Reference Types - C# 8+)
```csharp
#nullable enable

public class Ejemplo
{
    // ? indica que puede ser null
    public string? ObtenerNombre(Persona? persona)
    {
        // Operador null-conditional
        return persona?.Nombre ?? "Desconocido";
    }
    
    // Sin ? indica que NO debe ser null
    public string ObtenerNombreSeguro(Persona persona)
    {
        // El compilador da warning si persona puede ser null
        return persona.Nombre ?? "Desconocido";
    }
}
```

## Recursos Adicionales

### Documentación Oficial
- [C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [C# for Java Developers](https://docs.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Language Comparison](https://en.wikipedia.org/wiki/Comparison_of_C_Sharp_and_Java)

### Siguientes Pasos
1. **[POO en C#](../02-oop/)** - Clases, herencia, polimorfismo
2. **[Collections](../03-collections/)** - Estructuras de datos
3. **[LINQ](../04-streams-linq/)** - Consultas integradas (Stream API)

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://twitter.com/JoseLuisGS_)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto
- 🌐 Web: [https://joseluisgs.dev](https://joseluisgs.dev)

## Licencia

Este proyecto está licenciado bajo **Creative Commons BY-NC-SA 4.0**

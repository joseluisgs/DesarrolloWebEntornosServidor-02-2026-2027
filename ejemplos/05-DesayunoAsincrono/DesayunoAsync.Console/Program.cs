using DesayunoAsync.Console.Services;

// ==================================================================================
// 🚀 EJEMPLO 05: PROGRAMACIÓN ASÍNCRONA - Task vs CompletableFuture
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ⚡ EJEMPLO ASYNC/AWAIT - CompletableFuture → Task                 ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new DesayunoService();

// Preparación síncrona (lenta)
var desayuno1 = await service.PrepararDesayunoSincronoAsync();
System.Console.WriteLine($"Resultado: {desayuno1}");

// Preparación asíncrona paralela (rápida)
var desayuno2 = await service.PrepararDesayunoParaleloAsync();
System.Console.WriteLine($"Resultado: {desayuno2}");

// ==================================================================================
// RESUMEN
// ==================================================================================
System.Console.WriteLine();
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN CompletableFuture vs Task/async-await              ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌─────────────────────────────────┬───────────────────────────────┐");
System.Console.WriteLine("│ JAVA (CompletableFuture)        │ C# (Task/async-await)         │");
System.Console.WriteLine("├─────────────────────────────────┼───────────────────────────────┤");
System.Console.WriteLine("│ CompletableFuture<T>            │ Task<T>                       │");
System.Console.WriteLine("│ .supplyAsync(() -> ...)         │ async Task<T> Method()        │");
System.Console.WriteLine("│ .thenApply(f)                   │ await task                    │");
System.Console.WriteLine("│ .thenCompose(f)                 │ await task                    │");
System.Console.WriteLine("│ CompletableFuture.allOf()       │ Task.WhenAll()                │");
System.Console.WriteLine("│ CompletableFuture.anyOf()       │ Task.WhenAny()                │");
System.Console.WriteLine("│ .join()                         │ await task                    │");
System.Console.WriteLine("│ .get()                          │ .Result (no recomendado)      │");
System.Console.WriteLine("└─────────────────────────────────┴───────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 VENTAJAS de async/await en C#:");
System.Console.WriteLine("  ✅ Sintaxis más natural y legible");
System.Console.WriteLine("  ✅ Manejo de excepciones integrado");
System.Console.WriteLine("  ✅ Menor boilerplate que CompletableFuture");
System.Console.WriteLine("  ✅ Soporte del lenguaje (no biblioteca)");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Async/Await                                ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

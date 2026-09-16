using TenistasAsync.Console.Services;

// ==================================================================================
// 🚀 EJEMPLO 06: ASYNC STREAMS - IAsyncEnumerable
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  🌊 EJEMPLO ASYNC STREAMS - Java Streams → IAsyncEnumerable       ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new TenistaAsyncService();

// ==================================================================================
// DEMO 1: ASYNC STREAM BÁSICO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🌊 DEMO 1: Async Stream Básico");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

await foreach (var tenista in service.GetTenistasStreamAsync())
{
    System.Console.WriteLine($"  → {tenista}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 2: FILTRADO ASÍNCRONO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔍 DEMO 2: Filtrado Asíncrono");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Tenistas españoles:");
await foreach (var tenista in service.FiltrarPorPaisAsync("España"))
{
    System.Console.WriteLine($"  → {tenista}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 3: MAPEO ASÍNCRONO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔄 DEMO 3: Mapeo Asíncrono");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Solo nombres:");
await foreach (var nombre in service.MapearANombresAsync())
{
    System.Console.WriteLine($"  → {nombre}");
}

System.Console.WriteLine();

// ==================================================================================
// RESUMEN
// ==================================================================================
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN Java Streams vs IAsyncEnumerable                   ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌─────────────────────────────────┬───────────────────────────────┐");
System.Console.WriteLine("│ JAVA (Stream<T>)                │ C# (IAsyncEnumerable<T>)      │");
System.Console.WriteLine("├─────────────────────────────────┼───────────────────────────────┤");
System.Console.WriteLine("│ Stream<T>                       │ IAsyncEnumerable<T>           │");
System.Console.WriteLine("│ stream.forEach()                │ await foreach                 │");
System.Console.WriteLine("│ stream.filter()                 │ await foreach con if          │");
System.Console.WriteLine("│ stream.map()                    │ await foreach con yield       │");
System.Console.WriteLine("│ stream.collect()                │ ToListAsync() (LINQ async)    │");
System.Console.WriteLine("│ (síncrono o bloqueante)         │ completamente asíncrono       │");
System.Console.WriteLine("└─────────────────────────────────┴───────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 VENTAJAS de IAsyncEnumerable:");
System.Console.WriteLine("  ✅ Streaming verdaderamente asíncrono");
System.Console.WriteLine("  ✅ Eficiente en memoria (lazy evaluation)");
System.Console.WriteLine("  ✅ Ideal para I/O operations (BD, API, archivos)");
System.Console.WriteLine("  ✅ Composición con LINQ asíncrono");
System.Console.WriteLine("  ✅ Backpressure automático");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Async Streams                              ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

using TenistasSync.Console.Services;
using TenistasSync.Console.Utils;

// ==================================================================================
// 🚀 EJEMPLO 03: PROGRAMACIÓN SÍNCRONA Y COLECCIONES
// ==================================================================================
// 
// Este ejemplo demuestra:
// 1. Operaciones síncronas (sin async/await)
// 2. Uso de List<T> (equivalente a ArrayList en Java)
// 3. LINQ (equivalente a Stream API en Java)
// 4. Operaciones comunes sobre colecciones
//
// CONCEPTOS JAVA → C#:
// - ArrayList<T> → List<T>
// - Stream API → LINQ
// - Collectors → LINQ + ToList/ToDictionary/etc
// - Optional<T> → T? (nullable reference types)
// - Predicate<T> → Func<T, bool>
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  🎾 EJEMPLO PROGRAMACIÓN SÍNCRONA - Colecciones y LINQ             ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new TenistaService();

// ==================================================================================
// DEMO 1: OPERACIONES BÁSICAS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📋 DEMO 1: Operaciones Básicas con List<T>");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var todos = service.ObtenerTodos();
System.Console.WriteLine($"📊 Total de tenistas: {todos.Count}");
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   List<Tenista> tenistas = service.getAllTenistas();");
System.Console.WriteLine("   int count = tenistas.size();");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   var tenistas = service.ObtenerTodos();");
System.Console.WriteLine("   int count = tenistas.Count;");
System.Console.WriteLine();

// ==================================================================================
// DEMO 2: FILTRADO CON LINQ (Similar a Stream API)
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔍 DEMO 2: Filtrado con LINQ (Stream API → C#)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var españoles = service.ObtenerPorPais("España");
System.Console.WriteLine($"🇪🇸 Tenistas españoles: {españoles.Count}");
foreach (var t in españoles)
{
    System.Console.WriteLine($"  → {t.Nombre} (Ranking: {t.Ranking})");
}
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java (Stream API):");
System.Console.WriteLine("   List<Tenista> españoles = tenistas.stream()");
System.Console.WriteLine("       .filter(t -> t.getPais().equals(\"España\"))");
System.Console.WriteLine("       .collect(Collectors.toList());");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C# (LINQ):");
System.Console.WriteLine("   var españoles = tenistas.Where(t => t.Pais == \"España\").ToList();");
System.Console.WriteLine();

// ==================================================================================
// DEMO 3: TOP N RANKINGS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🏆 DEMO 3: Top 3 Tenistas");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var top3 = service.ObtenerTopN(3);
System.Console.WriteLine("🥇 Top 3 del ranking:");
foreach (var t in top3)
{
    System.Console.WriteLine($"  {t.Ranking}. {t.Nombre} - {t.Titulos} títulos");
}
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   List<Tenista> top3 = tenistas.stream()");
System.Console.WriteLine("       .sorted(Comparator.comparing(Tenista::getRanking))");
System.Console.WriteLine("       .limit(3)");
System.Console.WriteLine("       .collect(Collectors.toList());");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   var top3 = tenistas.OrderBy(t => t.Ranking).Take(3).ToList();");
System.Console.WriteLine();

// ==================================================================================
// DEMO 4: AGRUPACIÓN POR PAÍS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🌍 DEMO 4: Agrupación por País");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var porPais = service.AgruparPorPais();
System.Console.WriteLine($"📊 Tenistas por país ({porPais.Count} países):");
foreach (var (pais, tenistas) in porPais)
{
    System.Console.WriteLine($"  {pais}: {tenistas.Count} tenista(s)");
    foreach (var t in tenistas)
    {
        System.Console.WriteLine($"    → {t.Nombre}");
    }
}
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   Map<String, List<Tenista>> porPais = tenistas.stream()");
System.Console.WriteLine("       .collect(Collectors.groupingBy(Tenista::getPais));");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   var porPais = tenistas.GroupBy(t => t.Pais)");
System.Console.WriteLine("       .ToDictionary(g => g.Key, g => g.ToList());");
System.Console.WriteLine();

// ==================================================================================
// DEMO 5: ESTADÍSTICAS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📈 DEMO 5: Estadísticas de Títulos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var stats = service.ObtenerEstadisticasTitulos();
System.Console.WriteLine("📊 Estadísticas de títulos:");
System.Console.WriteLine($"  Total: {stats.Total}");
System.Console.WriteLine($"  Promedio: {stats.Promedio:F2}");
System.Console.WriteLine($"  Mínimo: {stats.Minimo}");
System.Console.WriteLine($"  Máximo: {stats.Maximo}");
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   IntSummaryStatistics stats = tenistas.stream()");
System.Console.WriteLine("       .collect(Collectors.summarizingInt(Tenista::getTitulos));");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   var total = tenistas.Sum(t => t.Titulos);");
System.Console.WriteLine("   var promedio = tenistas.Average(t => t.Titulos);");
System.Console.WriteLine("   var minimo = tenistas.Min(t => t.Titulos);");
System.Console.WriteLine("   var maximo = tenistas.Max(t => t.Titulos);");
System.Console.WriteLine();

// ==================================================================================
// DEMO 6: BÚSQUEDA CON PREDICADOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔎 DEMO 6: Búsqueda con Predicados (Func<T, bool>)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var conMuchosGrandSlams = service.ObtenerConMasDeTitulos(10);
System.Console.WriteLine($"🏆 Tenistas con más de 10 Grand Slams: {conMuchosGrandSlams.Count}");
foreach (var t in conMuchosGrandSlams)
{
    System.Console.WriteLine($"  → {t.Nombre}: {t.Titulos} títulos");
}
System.Console.WriteLine();

// ==================================================================================
// DEMO 7: ANY / ALL (Similar a anyMatch / allMatch)
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("✅ DEMO 7: Any / All (anyMatch / allMatch)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var hayEspañoles = service.ExisteConCondicion(t => t.Pais == "España");
var todosJovenes = CollectionUtils.TodosCumplen(todos, t => t.Edad < 30);
var algunJoven = CollectionUtils.AlgunoCumple(todos, t => t.Edad < 25);

System.Console.WriteLine($"¿Hay algún español?: {hayEspañoles}");
System.Console.WriteLine($"¿Todos son menores de 30?: {todosJovenes}");
System.Console.WriteLine($"¿Alguno es menor de 25?: {algunJoven}");
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   boolean hayEspañoles = tenistas.stream()");
System.Console.WriteLine("       .anyMatch(t -> t.getPais().equals(\"España\"));");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   bool hayEspañoles = tenistas.Any(t => t.Pais == \"España\");");
System.Console.WriteLine();

// ==================================================================================
// DEMO 8: MAP / SELECT (Transformación)
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔄 DEMO 8: Map / Select (Transformación)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var nombres = service.ObtenerNombres();
System.Console.WriteLine($"📝 Lista de nombres ({nombres.Count}):");
System.Console.WriteLine($"  {string.Join(", ", nombres)}");
System.Console.WriteLine();

System.Console.WriteLine("💡 En Java:");
System.Console.WriteLine("   List<String> nombres = tenistas.stream()");
System.Console.WriteLine("       .map(Tenista::getNombre)");
System.Console.WriteLine("       .collect(Collectors.toList());");
System.Console.WriteLine();

System.Console.WriteLine("💡 En C#:");
System.Console.WriteLine("   var nombres = tenistas.Select(t => t.Nombre).ToList();");
System.Console.WriteLine();

// ==================================================================================
// DEMO 9: AGRUPACIÓN POR RANGO DE EDAD
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("👥 DEMO 9: Agrupación por Rango de Edad");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var porEdad = service.AgruparPorRangoEdad();
foreach (var (rango, tenistas) in porEdad)
{
    System.Console.WriteLine($"  {rango}: {tenistas.Count} tenista(s)");
    foreach (var t in tenistas)
    {
        System.Console.WriteLine($"    → {t.Nombre} ({t.Edad} años)");
    }
}
System.Console.WriteLine();

// ==================================================================================
// DEMO 10: TENISTA CON MÁS TÍTULOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("👑 DEMO 10: Tenista con Más Títulos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var campeon = service.ObtenerTenistaConMasTitulos();
if (campeon != null)
{
    System.Console.WriteLine($"🏆 Campeón con más títulos: {campeon.Nombre} ({campeon.Titulos} Grand Slams)");
}
System.Console.WriteLine();

// ==================================================================================
// RESUMEN Y COMPARACIÓN
// ==================================================================================
System.Console.WriteLine();
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN JAVA (Stream API) vs C# (LINQ)                     ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌─────────────────────────────┬─────────────────────────────────────┐");
System.Console.WriteLine("│ JAVA (Stream API)           │ C# (LINQ)                           │");
System.Console.WriteLine("├─────────────────────────────┼─────────────────────────────────────┤");
System.Console.WriteLine("│ stream()                    │ (no necesario - LINQ integrado)     │");
System.Console.WriteLine("│ filter(predicate)           │ Where(predicate)                    │");
System.Console.WriteLine("│ map(function)               │ Select(function)                    │");
System.Console.WriteLine("│ sorted(comparator)          │ OrderBy / OrderByDescending         │");
System.Console.WriteLine("│ limit(n)                    │ Take(n)                             │");
System.Console.WriteLine("│ skip(n)                     │ Skip(n)                             │");
System.Console.WriteLine("│ distinct()                  │ Distinct()                          │");
System.Console.WriteLine("│ anyMatch(predicate)         │ Any(predicate)                      │");
System.Console.WriteLine("│ allMatch(predicate)         │ All(predicate)                      │");
System.Console.WriteLine("│ noneMatch(predicate)        │ !Any(predicate)                     │");
System.Console.WriteLine("│ findFirst()                 │ FirstOrDefault()                    │");
System.Console.WriteLine("│ count()                     │ Count() / LongCount()               │");
System.Console.WriteLine("│ collect(Collectors...)      │ ToList() / ToDictionary() / ...     │");
System.Console.WriteLine("│ groupingBy(classifier)      │ GroupBy(selector).ToDictionary()    │");
System.Console.WriteLine("│ summarizingInt()            │ Sum/Average/Min/Max separados       │");
System.Console.WriteLine("└─────────────────────────────┴─────────────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 VENTAJAS de LINQ:");
System.Console.WriteLine("  ✅ Sintaxis más concisa y natural");
System.Console.WriteLine("  ✅ Type-safe en tiempo de compilación");
System.Console.WriteLine("  ✅ IntelliSense y refactoring automático");
System.Console.WriteLine("  ✅ Query syntax opcional (similar a SQL)");
System.Console.WriteLine("  ✅ Deferred execution (lazy evaluation)");
System.Console.WriteLine();
System.Console.WriteLine("💡 Query Syntax (alternativa):");
System.Console.WriteLine("   var españoles = from t in tenistas");
System.Console.WriteLine("                   where t.Pais == \"España\"");
System.Console.WriteLine("                   orderby t.Ranking");
System.Console.WriteLine("                   select t;");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Programación Síncrona y LINQ              ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

// ==================================================================================
// 👨‍💻 Autor: José Luis González Sánchez
// 📧 Email: joseluis.gonzalez@profesor.com
// 🌐 Web: https://joseluisgs.dev
// 📅 Fecha: Octubre 2025
// 📝 Licencia: Creative Commons BY-NC-SA 4.0
// ==================================================================================

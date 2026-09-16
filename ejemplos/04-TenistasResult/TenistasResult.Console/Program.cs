using TenistasResult.Console.Services;

// ==================================================================================
// 🚀 EJEMPLO 04: RESULT PATTERN - Railway Oriented Programming
// ==================================================================================
// 
// Este ejemplo demuestra:
// 1. Result Pattern para manejo funcional de errores
// 2. Railway Oriented Programming
// 3. Alternativa a exceptions para control de flujo
//
// CONCEPTOS JAVA → C#:
// - Optional<T> → Result<T> (más completo)
// - Either<L, R> → Result<T> con error string
// - Try<T> → Result<T>
// - Vavr/Arrow → CSharpFunctionalExtensions
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  🎯 EJEMPLO RESULT PATTERN - Manejo Funcional de Errores           ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new TenistaService();

// ==================================================================================
// DEMO 1: OPERACIONES EXITOSAS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("✅ DEMO 1: Operaciones Exitosas");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var result1 = service.FindById(1);
if (result1.IsSuccess)
{
    System.Console.WriteLine($"✅ Encontrado: {result1.Value}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 2: MANEJO DE ERRORES
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("❌ DEMO 2: Manejo de Errores");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var result2 = service.FindById(999);
if (result2.IsFailure)
{
    System.Console.WriteLine($"❌ Error: {result2.Error}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 3: RAILWAY ORIENTED PROGRAMMING
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🚂 DEMO 3: Railway Oriented Programming");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Creando tenista válido:");
var createResult1 = service.CreateTenista("Roger Federer", 4, "Suiza", 20);
if (createResult1.IsSuccess)
{
    System.Console.WriteLine($"✅ Creado: {createResult1.Value}");
}
else
{
    System.Console.WriteLine($"❌ Error: {createResult1.Error}");
}

System.Console.WriteLine();
System.Console.WriteLine("Creando tenista inválido (ranking duplicado):");
var createResult2 = service.CreateTenista("Andy Murray", 1, "Reino Unido", 3);
if (createResult2.IsFailure)
{
    System.Console.WriteLine($"❌ Error esperado: {createResult2.Error}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 4: PATTERN MATCHING CON RESULT
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔍 DEMO 4: Pattern Matching");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var findResult = service.FindById(2);
var mensaje = findResult.IsSuccess
    ? $"Encontrado: {findResult.Value.Nombre}"
    : $"Error: {findResult.Error}";
System.Console.WriteLine(mensaje);

System.Console.WriteLine();

// ==================================================================================
// DEMO 5: COMPOSICIÓN DE OPERACIONES
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔗 DEMO 5: Composición de Operaciones");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var updateResult = service.UpdateRanking(1, 2);
System.Console.WriteLine(updateResult.IsSuccess 
    ? $"✅ Ranking actualizado: {updateResult.Value}"
    : $"❌ Error: {updateResult.Error}");

System.Console.WriteLine();

// ==================================================================================
// DEMO 6: TOP N CON VALIDACIÓN
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🏆 DEMO 6: Top N con Validación");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var topResult = service.GetTopN(3);
if (topResult.IsSuccess)
{
    System.Console.WriteLine("Top 3:");
    foreach (var t in topResult.Value)
    {
        System.Console.WriteLine($"  {t.Ranking}. {t.Nombre}");
    }
}

System.Console.WriteLine();

// ==================================================================================
// RESUMEN
// ==================================================================================
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN Optional/Either vs Result                           ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌─────────────────────────────┬─────────────────────────────────────┐");
System.Console.WriteLine("│ JAVA                        │ C# (CSharpFunctionalExtensions)     │");
System.Console.WriteLine("├─────────────────────────────┼─────────────────────────────────────┤");
System.Console.WriteLine("│ Optional<T>                 │ Result<T>                           │");
System.Console.WriteLine("│ Either<L, R>                │ Result<T> con error                 │");
System.Console.WriteLine("│ .map()                      │ .Map()                              │");
System.Console.WriteLine("│ .flatMap()                  │ .Bind()                             │");
System.Console.WriteLine("│ .filter()                   │ .Ensure()                           │");
System.Console.WriteLine("│ .orElse()                   │ Match() / GetValueOrDefault()       │");
System.Console.WriteLine("│ .ifPresent()                │ .Tap()                              │");
System.Console.WriteLine("└─────────────────────────────┴─────────────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 VENTAJAS del Result Pattern:");
System.Console.WriteLine("  ✅ Errores explícitos en la signatura");
System.Console.WriteLine("  ✅ No exceptions para control de flujo");
System.Console.WriteLine("  ✅ Composición funcional de operaciones");
System.Console.WriteLine("  ✅ Railway Oriented Programming");
System.Console.WriteLine("  ✅ Type-safe error handling");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Result Pattern                             ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

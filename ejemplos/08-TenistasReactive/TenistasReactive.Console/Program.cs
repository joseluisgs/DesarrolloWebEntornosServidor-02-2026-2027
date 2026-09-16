using System.Reactive.Linq;
using TenistasReactive.Console.Services;
using TenistasReactive.Console.Models;

// ==================================================================================
// 🚀 EJEMPLO 08: REACTIVE AVANZADO - RxJava Advanced → Rx.NET
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  🔥 EJEMPLO REACTIVE AVANZADO - RxJava → Rx.NET                    ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new TenistaReactivoService();

// ==================================================================================
// DEMO 1: COLD vs HOT OBSERVABLES
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("❄️  DEMO 1: Cold Observable (cada suscriptor recibe todo)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var coldObservable = service.GetColdObservable();

System.Console.WriteLine("Suscriptor 1:");
await coldObservable
    .Take(3)
    .Do(t => System.Console.WriteLine($"  [Sub1] → {t}"))
    .LastOrDefaultAsync();

System.Console.WriteLine("\nSuscriptor 2:");
await coldObservable
    .Take(3)
    .Do(t => System.Console.WriteLine($"  [Sub2] → {t}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 2: HOT OBSERVABLES - SUBJECTS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔥 DEMO 2: Hot Observables - Subjects");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

// PublishSubject
System.Console.WriteLine("📡 PublishSubject (sin buffer):");
service.GetPublishSubject()
    .Subscribe(t => System.Console.WriteLine($"  [Sub1] → {t.Nombre}"));

service.EmitirTenista(new Tenista { Id = 9, Nombre = "Andy Murray", Ranking = 9, Pais = "Reino Unido", GrandSlams = 3 });

service.GetPublishSubject()
    .Subscribe(t => System.Console.WriteLine($"  [Sub2] → {t.Nombre} (llegó tarde)"));

service.EmitirTenista(new Tenista { Id = 10, Nombre = "Stan Wawrinka", Ranking = 10, Pais = "Suiza", GrandSlams = 3 });

System.Console.WriteLine();

// BehaviorSubject
System.Console.WriteLine("💾 BehaviorSubject (último valor):");
service.GetBehaviorSubject()
    .Take(2)
    .Subscribe(t => System.Console.WriteLine($"  [Sub1] → {t.Nombre}"));

service.ActualizarTenistaActual(new Tenista { Id = 11, Nombre = "Roger Federer", Ranking = 11, Pais = "Suiza", GrandSlams = 20 });

await Task.Delay(200);
System.Console.WriteLine();

// ReplaySubject
System.Console.WriteLine("📼 ReplaySubject (buffer de 3):");
service.AgregarAlReplay(new Tenista { Id = 12, Nombre = "Pete Sampras", Ranking = 12, Pais = "USA", GrandSlams = 14 });
service.AgregarAlReplay(new Tenista { Id = 13, Nombre = "Andre Agassi", Ranking = 13, Pais = "USA", GrandSlams = 8 });
service.AgregarAlReplay(new Tenista { Id = 14, Nombre = "Boris Becker", Ranking = 14, Pais = "Alemania", GrandSlams = 6 });

System.Console.WriteLine("Suscriptor nuevo recibe últimos 3:");
await service.GetReplaySubject()
    .Take(3)
    .Do(t => System.Console.WriteLine($"  [SubNew] → {t.Nombre}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 3: SCHEDULERS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("⚙️  DEMO 3: Schedulers (Control de Threading)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine($"Thread principal: {Environment.CurrentManagedThreadId}");
System.Console.WriteLine();

System.Console.WriteLine("SubscribeOn (cambia thread de ejecución):");
await service.GetObservableConScheduler()
    .Do(t => System.Console.WriteLine($"  [Observer] Thread: {Environment.CurrentManagedThreadId} - {t.Nombre}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

System.Console.WriteLine("ObserveOn (cambia thread de observación):");
await service.GetObservableConObserveOn()
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 4: BUFFER Y WINDOW
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📦 DEMO 4: Buffer y Window (Agrupación)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Buffer de 3 tenistas:");
var bufferIndex = 1;
await service.GetBufferedObservable(3)
    .Do(buffer =>
    {
        System.Console.WriteLine($"  Lote {bufferIndex++}:");
        foreach (var t in buffer)
        {
            System.Console.WriteLine($"    → {t.Nombre}");
        }
    })
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 5: THROTTLE Y SAMPLE
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("⏱️  DEMO 5: Throttle y Sample (Control de Flujo)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Sample (último valor cada 200ms):");
await service.GetSampledObservable(TimeSpan.FromMilliseconds(200))
    .Take(5)
    .Do(t => System.Console.WriteLine($"  → {t.Nombre}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 6: SCAN (Acumulación)
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("➕ DEMO 6: Scan (Acumulación de Grand Slams)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

await service.GetTotalGrandSlamsAcumulado()
    .Do(total => System.Console.WriteLine($"  Total acumulado: {total} Grand Slams"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 7: ZIP Y COMBINELATEST
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔗 DEMO 7: Zip y CombineLatest (Combinación)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Zip (combina por posición):");
await service.ZipTenistas()
    .Do(partido => System.Console.WriteLine($"  → {partido}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 8: MERGE Y CONCAT
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔀 DEMO 8: Merge y Concat");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Concat (secuencial):");
await service.ConcatenarTenistas()
    .Do(t => System.Console.WriteLine($"  → {t.Nombre}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 9: DISTINCT
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🎯 DEMO 9: Distinct (Países únicos)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

await service.GetPaisesUnicos()
    .Do(pais => System.Console.WriteLine($"  → {pais}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 10: RETRY
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔄 DEMO 10: Retry (Reintentos en error)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    await service.GetObservableConRetry(5)
        .Do(t => System.Console.WriteLine($"  ✓ Éxito: {t.Nombre}"))
        .LastOrDefaultAsync();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error final: {ex.Message}");
}

System.Console.WriteLine();

// ==================================================================================
// DEMO 11: TIMEOUT
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("⏰ DEMO 11: Timeout (Detectar inactividad)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    await service.GetObservableConTimeout(TimeSpan.FromMilliseconds(300))
        .Do(t => System.Console.WriteLine($"  → {t.Nombre}"))
        .LastOrDefaultAsync();
}
catch (TimeoutException)
{
    System.Console.WriteLine("  ⏰ Timeout: Observable demoró demasiado");
}

System.Console.WriteLine();

// ==================================================================================
// RESUMEN COMPARATIVO
// ==================================================================================
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 CONCEPTOS AVANZADOS RxJava vs Rx.NET                            ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌─────────────────────────────────┬─────────────────────────────────┐");
System.Console.WriteLine("│ CONCEPTO                        │ DESCRIPCIÓN                     │");
System.Console.WriteLine("├─────────────────────────────────┼─────────────────────────────────┤");
System.Console.WriteLine("│ COLD OBSERVABLE                 │ Cada suscriptor recibe todo     │");
System.Console.WriteLine("│ HOT OBSERVABLE                  │ Suscriptores comparten stream   │");
System.Console.WriteLine("│ PublishSubject                  │ Hot sin buffer                  │");
System.Console.WriteLine("│ BehaviorSubject                 │ Hot con último valor            │");
System.Console.WriteLine("│ ReplaySubject                   │ Hot con buffer histórico        │");
System.Console.WriteLine("│ SubscribeOn                     │ Thread de ejecución             │");
System.Console.WriteLine("│ ObserveOn                       │ Thread de observación           │");
System.Console.WriteLine("│ Buffer/Window                   │ Agrupación de eventos           │");
System.Console.WriteLine("│ Throttle/Debounce               │ Control de flujo                │");
System.Console.WriteLine("│ Sample                          │ Muestreo periódico              │");
System.Console.WriteLine("│ Scan                            │ Acumulación progresiva          │");
System.Console.WriteLine("│ Zip/CombineLatest               │ Combinación de streams          │");
System.Console.WriteLine("│ Merge/Concat                    │ Unión de streams                │");
System.Console.WriteLine("│ Distinct                        │ Eliminación de duplicados       │");
System.Console.WriteLine("│ Retry                           │ Reintentos automáticos          │");
System.Console.WriteLine("│ Timeout                         │ Límite de tiempo                │");
System.Console.WriteLine("└─────────────────────────────────┴─────────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 SCHEDULERS en Rx.NET:");
System.Console.WriteLine("  ✅ TaskPoolScheduler.Default - Thread pool (como Schedulers.io())");
System.Console.WriteLine("  ✅ NewThreadScheduler.Default - Nuevo thread (como Schedulers.newThread())");
System.Console.WriteLine("  ✅ ImmediateScheduler.Instance - Thread actual (como Schedulers.immediate())");
System.Console.WriteLine("  ✅ CurrentThreadScheduler.Instance - Cola en thread actual");
System.Console.WriteLine();
System.Console.WriteLine("🎯 PATRONES DE USO:");
System.Console.WriteLine("  📌 Hot Observables: Eventos UI, streams de datos en vivo");
System.Console.WriteLine("  📌 Cold Observables: Consultas HTTP, acceso a BD");
System.Console.WriteLine("  📌 Schedulers: Separar I/O de UI, procesamiento paralelo");
System.Console.WriteLine("  📌 Buffer/Window: Procesamiento por lotes");
System.Console.WriteLine("  📌 Throttle: Búsquedas en tiempo real, auto-guardado");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Reactive Avanzado                          ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

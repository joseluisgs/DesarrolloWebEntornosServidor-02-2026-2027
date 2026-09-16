using System.Reactive.Linq;
using ProductosReactivo.Console.Services;
using ProductosReactivo.Console.Models;

// ==================================================================================
// 🚀 EJEMPLO 07: PROGRAMACIÓN REACTIVA - RxJava → System.Reactive
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ⚡ EJEMPLO REACTIVE - RxJava → System.Reactive (Rx.NET)          ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

var service = new ProductoReactivoService();

// ==================================================================================
// DEMO 1: OBSERVABLE BÁSICO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🌊 DEMO 1: Observable Básico (Cold Observable)");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Suscribiéndose al observable...");
await service.GetProductosObservable()
    .Do(producto => System.Console.WriteLine($"  → {producto}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 2: OPERADORES DE TRANSFORMACIÓN
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔄 DEMO 2: Operadores de Transformación");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Solo nombres de productos:");
await service.MapearANombres()
    .Do(nombre => System.Console.WriteLine($"  → {nombre}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 3: FILTRADO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔍 DEMO 3: Filtrado de Productos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Productos de categoría 'Electrónica':");
await service.FiltrarPorCategoria("Electrónica")
    .Do(producto => System.Console.WriteLine($"  → {producto}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

System.Console.WriteLine("Productos con precio > $300:");
await service.GetProductosCaros(300m)
    .Do(producto => System.Console.WriteLine($"  → {producto}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 4: AGRUPACIÓN
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📦 DEMO 4: Agrupación por Categoría");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

await service.AgruparPorCategoria()
    .SelectMany(async grupo =>
    {
        var items = await grupo.ToList();
        System.Console.WriteLine($"\n📂 Categoría: {grupo.Key}");
        foreach (var producto in items)
        {
            System.Console.WriteLine($"  → {producto}");
        }
        return grupo.Key;
    })
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 5: AGREGACIÓN
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("➕ DEMO 5: Agregación - Suma Total de Precios");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

var precioTotal = await service.CalcularPrecioTotal();
System.Console.WriteLine($"💰 Precio total de todos los productos: ${precioTotal:F2}");

System.Console.WriteLine();

// ==================================================================================
// DEMO 6: HOT OBSERVABLE (Subject)
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔥 DEMO 6: Hot Observable con Subject");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Configurando suscriptores al Hot Observable...");

// Primer suscriptor
service.GetProductosHotObservable()
    .Subscribe(p => System.Console.WriteLine($"  [Suscriptor 1] → {p}"));

// Segundo suscriptor
service.GetProductosHotObservable()
    .Subscribe(p => System.Console.WriteLine($"  [Suscriptor 2] → {p}"));

// Emitir productos
System.Console.WriteLine("\nEmitiendo productos nuevos...");
service.EmitirProducto(new Producto 
{ 
    Id = 9, 
    Nombre = "Tablet Samsung", 
    Precio = 599.99m, 
    Categoria = "Electrónica", 
    Stock = 15 
});

service.EmitirProducto(new Producto 
{ 
    Id = 10, 
    Nombre = "SmartWatch", 
    Precio = 249.99m, 
    Categoria = "Electrónica", 
    Stock = 20 
});

await Task.Delay(500); // Dar tiempo para procesar

System.Console.WriteLine();

// ==================================================================================
// DEMO 7: OBSERVABLE CON INTERVALO
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("⏱️  DEMO 7: Observable con Intervalo de Tiempo");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Emitiendo productos cada 300ms...");
await service.GetProductosConIntervalo(TimeSpan.FromMilliseconds(300))
    .Take(5) // Solo los primeros 5
    .Do(producto => System.Console.WriteLine($"  → {producto}"))
    .LastOrDefaultAsync();

System.Console.WriteLine();

// ==================================================================================
// DEMO 8: COMBINACIÓN DE OPERADORES
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔗 DEMO 8: Combinación de Operadores");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

System.Console.WriteLine("Productos de Electrónica con precio > $200:");
var productosElectronica = await service.GetProductosObservable()
    .Where(p => p.Categoria == "Electrónica")
    .Where(p => p.Precio > 200m)
    .ToList();

foreach (var producto in productosElectronica.OrderBy(p => p.Precio))
{
    System.Console.WriteLine($"  → {producto}");
}

System.Console.WriteLine();

// ==================================================================================
// RESUMEN COMPARATIVO
// ==================================================================================
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN RxJava vs System.Reactive (Rx.NET)                 ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌───────────────────────────────────┬─────────────────────────────────┐");
System.Console.WriteLine("│ JAVA (RxJava)                     │ C# (System.Reactive / Rx.NET)   │");
System.Console.WriteLine("├───────────────────────────────────┼─────────────────────────────────┤");
System.Console.WriteLine("│ Observable<T>                     │ IObservable<T>                  │");
System.Console.WriteLine("│ Observer<T>                       │ IObserver<T>                    │");
System.Console.WriteLine("│ Observable.create()               │ Observable.Create()             │");
System.Console.WriteLine("│ Observable.fromIterable()         │ list.ToObservable()             │");
System.Console.WriteLine("│ Observable.interval()             │ Observable.Interval()           │");
System.Console.WriteLine("│ .map()                            │ .Select()                       │");
System.Console.WriteLine("│ .filter()                         │ .Where()                        │");
System.Console.WriteLine("│ .flatMap()                        │ .SelectMany()                   │");
System.Console.WriteLine("│ .groupBy()                        │ .GroupBy()                      │");
System.Console.WriteLine("│ .reduce()                         │ .Aggregate()                    │");
System.Console.WriteLine("│ .subscribe()                      │ .Subscribe()                    │");
System.Console.WriteLine("│ .doOnNext()                       │ .Do()                           │");
System.Console.WriteLine("│ .onErrorReturn()                  │ .Catch()                        │");
System.Console.WriteLine("│ PublishSubject<T>                 │ Subject<T>                      │");
System.Console.WriteLine("│ BehaviorSubject<T>                │ BehaviorSubject<T>              │");
System.Console.WriteLine("│ ReplaySubject<T>                  │ ReplaySubject<T>                │");
System.Console.WriteLine("└───────────────────────────────────┴─────────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 CONCEPTOS CLAVE de Reactive Extensions:");
System.Console.WriteLine("  ✅ Push-based (vs Pull-based de IEnumerable)");
System.Console.WriteLine("  ✅ Asíncrono y no bloqueante por diseño");
System.Console.WriteLine("  ✅ Composición funcional de operadores");
System.Console.WriteLine("  ✅ Hot vs Cold Observables");
System.Console.WriteLine("  ✅ Backpressure y control de flujo");
System.Console.WriteLine("  ✅ Schedulers para control de concurrencia");
System.Console.WriteLine();
System.Console.WriteLine("🎯 CUÁNDO USAR Reactive Extensions:");
System.Console.WriteLine("  📌 Eventos de UI (clicks, cambios de texto)");
System.Console.WriteLine("  📌 Streams de datos en tiempo real");
System.Console.WriteLine("  📌 WebSockets y SignalR");
System.Console.WriteLine("  📌 Combinación compleja de eventos asíncronos");
System.Console.WriteLine("  📌 Throttling, debouncing, buffering");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Reactive Extensions                        ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

using DesayunoAsync.Console.Models;

namespace DesayunoAsync.Console.Services;

/// <summary>
/// Servicio de cocina asíncrono
/// Equivalente a CompletableFuture en Java
/// 
/// En Java:
/// CompletableFuture<Ingrediente> prepararCafe() {
///     return CompletableFuture.supplyAsync(() -> {...});
/// }
/// 
/// En C#:
/// async Task<Ingrediente> PrepararCafeAsync() {
///     await Task.Delay(tiempo);
///     return cafe;
/// }
/// </summary>
public class CocinaService
{
    public async Task<Ingrediente> PrepararCafeAsync()
    {
        System.Console.WriteLine("☕ Preparando café...");
        await Task.Delay(1000); // Simula preparación
        System.Console.WriteLine("✅ Café listo");
        return new Ingrediente { Nombre = "Café", TiempoPreparacion = 1000 };
    }

    public async Task<Ingrediente> PrepararHuevosAsync()
    {
        System.Console.WriteLine("🍳 Preparando huevos...");
        await Task.Delay(2000);
        System.Console.WriteLine("✅ Huevos listos");
        return new Ingrediente { Nombre = "Huevos", TiempoPreparacion = 2000 };
    }

    public async Task<Ingrediente> PrepararTocinAsync()
    {
        System.Console.WriteLine("🥓 Preparando tocino...");
        await Task.Delay(1500);
        System.Console.WriteLine("✅ Tocino listo");
        return new Ingrediente { Nombre = "Tocino", TiempoPreparacion = 1500 };
    }

    public async Task<Ingrediente> PrepararPanAsync()
    {
        System.Console.WriteLine("🍞 Preparando pan tostado...");
        await Task.Delay(500);
        System.Console.WriteLine("✅ Pan listo");
        return new Ingrediente { Nombre = "Pan Tostado", TiempoPreparacion = 500 };
    }
}

public class DesayunoService
{
    private readonly CocinaService _cocina = new();

    /// <summary>
    /// Preparación SÍNCRONA (mala práctica)
    /// En Java: método normal sin CompletableFuture
    /// </summary>
    public async Task<Desayuno> PrepararDesayunoSincronoAsync()
    {
        System.Console.WriteLine("\n🔴 PREPARACIÓN SÍNCRONA (secuencial, lenta):");
        var inicio = DateTime.Now;

        var cafe = await _cocina.PrepararCafeAsync();
        var huevos = await _cocina.PrepararHuevosAsync();
        var tocino = await _cocina.PrepararTocinAsync();
        var pan = await _cocina.PrepararPanAsync();

        var duracion = (DateTime.Now - inicio).TotalMilliseconds;
        System.Console.WriteLine($"⏱️  Tiempo total: {duracion}ms\n");

        return new Desayuno
        {
            Componentes = new List<string> { cafe.Nombre, huevos.Nombre, tocino.Nombre, pan.Nombre }
        };
    }

    /// <summary>
    /// Preparación ASÍNCRONA PARALELA (mejor práctica)
    /// En Java: CompletableFuture.allOf(...).join()
    /// En C#: Task.WhenAll(...)
    /// </summary>
    public async Task<Desayuno> PrepararDesayunoParaleloAsync()
    {
        System.Console.WriteLine("\n🟢 PREPARACIÓN ASÍNCRONA (paralela, rápida):");
        var inicio = DateTime.Now;

        // Iniciar todas las tareas en paralelo
        var tareasCafe = _cocina.PrepararCafeAsync();
        var tareasHuevos = _cocina.PrepararHuevosAsync();
        var tareasTocino = _cocina.PrepararTocinAsync();
        var tareasPan = _cocina.PrepararPanAsync();

        // Esperar a que todas terminen
        var resultados = await Task.WhenAll(tareasCafe, tareasHuevos, tareasTocino, tareasPan);

        var duracion = (DateTime.Now - inicio).TotalMilliseconds;
        System.Console.WriteLine($"⏱️  Tiempo total: {duracion}ms\n");

        return new Desayuno
        {
            Componentes = resultados.Select(r => r.Nombre).ToList()
        };
    }
}

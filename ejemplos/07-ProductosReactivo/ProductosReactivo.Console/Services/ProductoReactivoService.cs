using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProductosReactivo.Console.Models;

namespace ProductosReactivo.Console.Services;

/// <summary>
/// Servicio que demuestra Reactive Extensions (Rx)
/// Equivalente a RxJava en el ecosistema Java
/// 
/// En Java (RxJava):
/// Observable<Producto> getProductos() { ... }
/// 
/// En C# (System.Reactive):
/// IObservable<Producto> GetProductos() { ... }
/// </summary>
public class ProductoReactivoService
{
    private readonly List<Producto> _productos;
    private readonly Subject<Producto> _productosSubject;

    public ProductoReactivoService()
    {
        _productos = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Laptop Dell XPS", Precio = 1299.99m, Categoria = "Electrónica", Stock = 15 },
            new Producto { Id = 2, Nombre = "iPhone 15 Pro", Precio = 999.99m, Categoria = "Electrónica", Stock = 25 },
            new Producto { Id = 3, Nombre = "Silla Gamer", Precio = 299.99m, Categoria = "Muebles", Stock = 10 },
            new Producto { Id = 4, Nombre = "Monitor LG 27\"", Precio = 349.99m, Categoria = "Electrónica", Stock = 8 },
            new Producto { Id = 5, Nombre = "Teclado Mecánico", Precio = 129.99m, Categoria = "Electrónica", Stock = 20 },
            new Producto { Id = 6, Nombre = "Mouse Logitech", Precio = 49.99m, Categoria = "Electrónica", Stock = 30 },
            new Producto { Id = 7, Nombre = "Escritorio", Precio = 449.99m, Categoria = "Muebles", Stock = 5 },
            new Producto { Id = 8, Nombre = "Auriculares Sony", Precio = 199.99m, Categoria = "Electrónica", Stock = 12 }
        };
        
        _productosSubject = new Subject<Producto>();
    }

    /// <summary>
    /// Observable básico (Cold Observable)
    /// En Java: Observable.fromIterable()
    /// En C#: Observable.Create() o ToObservable()
    /// </summary>
    public IObservable<Producto> GetProductosObservable()
    {
        return Observable.Create<Producto>(async observer =>
        {
            foreach (var producto in _productos)
            {
                // Simular operación asíncrona
                await Task.Delay(100);
                observer.OnNext(producto);
            }
            observer.OnCompleted();
        });
    }

    /// <summary>
    /// Observable desde lista con emisión inmediata
    /// En Java: Observable.fromIterable()
    /// En C#: ToObservable()
    /// </summary>
    public IObservable<Producto> GetProductosDesdeListaObservable()
    {
        return _productos.ToObservable();
    }

    /// <summary>
    /// Subject para hot observables (múltiples suscriptores)
    /// En Java: PublishSubject
    /// En C#: Subject<T>
    /// </summary>
    public IObservable<Producto> GetProductosHotObservable()
    {
        return _productosSubject.AsObservable();
    }

    /// <summary>
    /// Emitir nuevo producto (para hot observable)
    /// </summary>
    public void EmitirProducto(Producto producto)
    {
        _productosSubject.OnNext(producto);
    }

    /// <summary>
    /// Observable con intervalo de tiempo
    /// En Java: Observable.interval()
    /// En C#: Observable.Interval()
    /// </summary>
    public IObservable<Producto> GetProductosConIntervalo(TimeSpan intervalo)
    {
        return Observable.Interval(intervalo)
            .Take(_productos.Count)
            .Select(i => _productos[(int)i]);
    }

    /// <summary>
    /// Filtrar productos por categoría
    /// En Java: observable.filter()
    /// En C#: observable.Where()
    /// </summary>
    public IObservable<Producto> FiltrarPorCategoria(string categoria)
    {
        return GetProductosObservable()
            .Where(p => p.Categoria == categoria);
    }

    /// <summary>
    /// Mapear productos a sus nombres
    /// En Java: observable.map()
    /// En C#: observable.Select()
    /// </summary>
    public IObservable<string> MapearANombres()
    {
        return GetProductosObservable()
            .Select(p => p.Nombre);
    }

    /// <summary>
    /// Obtener productos con precio mayor a cierto valor
    /// En Java: observable.filter().map()
    /// En C#: observable.Where().Select()
    /// </summary>
    public IObservable<string> GetProductosCaros(decimal precioMinimo)
    {
        return GetProductosObservable()
            .Where(p => p.Precio > precioMinimo)
            .Select(p => $"{p.Nombre}: ${p.Precio:F2}");
    }

    /// <summary>
    /// Agrupar productos por categoría
    /// En Java: observable.groupBy()
    /// En C#: observable.GroupBy()
    /// </summary>
    public IObservable<IGroupedObservable<string, Producto>> AgruparPorCategoria()
    {
        return GetProductosDesdeListaObservable()
            .GroupBy(p => p.Categoria);
    }

    /// <summary>
    /// Calcular suma de precios
    /// En Java: observable.reduce()
    /// En C#: observable.Aggregate()
    /// </summary>
    public IObservable<decimal> CalcularPrecioTotal()
    {
        return GetProductosDesdeListaObservable()
            .Select(p => p.Precio)
            .Aggregate((acc, precio) => acc + precio);
    }

    /// <summary>
    /// Manejo de errores
    /// En Java: observable.onErrorReturn()
    /// En C#: observable.Catch()
    /// </summary>
    public IObservable<Producto> GetProductosConErrorHandling()
    {
        return Observable.Create<Producto>(observer =>
        {
            try
            {
                foreach (var producto in _productos)
                {
                    if (producto.Stock < 0) // Simular error
                    {
                        throw new InvalidOperationException("Stock no puede ser negativo");
                    }
                    observer.OnNext(producto);
                }
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
            return System.Reactive.Disposables.Disposable.Empty;
        })
        .Catch<Producto, Exception>(ex =>
        {
            System.Console.WriteLine($"Error capturado: {ex.Message}");
            return Observable.Empty<Producto>();
        });
    }
}

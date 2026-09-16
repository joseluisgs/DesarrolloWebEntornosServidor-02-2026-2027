using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Services;

/// <summary>
/// Servicio avanzado de Reactive Extensions
/// Demuestra conceptos avanzados de RxJava → Rx.NET
/// - Hot vs Cold Observables
/// - Schedulers
/// - Subjects (PublishSubject, BehaviorSubject, ReplaySubject)
/// - Operadores avanzados
/// </summary>
public class TenistaReactivoService
{
    private readonly List<Tenista> _tenistas;
    private readonly Subject<Tenista> _publishSubject;
    private readonly BehaviorSubject<Tenista> _behaviorSubject;
    private readonly ReplaySubject<Tenista> _replaySubject;

    public TenistaReactivoService()
    {
        _tenistas = new List<Tenista>
        {
            new Tenista { Id = 1, Nombre = "Novak Djokovic", Ranking = 1, Pais = "Serbia", GrandSlams = 24 },
            new Tenista { Id = 2, Nombre = "Carlos Alcaraz", Ranking = 2, Pais = "España", GrandSlams = 2 },
            new Tenista { Id = 3, Nombre = "Daniil Medvedev", Ranking = 3, Pais = "Rusia", GrandSlams = 1 },
            new Tenista { Id = 4, Nombre = "Jannik Sinner", Ranking = 4, Pais = "Italia", GrandSlams = 1 },
            new Tenista { Id = 5, Nombre = "Andrey Rublev", Ranking = 5, Pais = "Rusia", GrandSlams = 0 },
            new Tenista { Id = 6, Nombre = "Rafael Nadal", Ranking = 6, Pais = "España", GrandSlams = 22 },
            new Tenista { Id = 7, Nombre = "Stefanos Tsitsipas", Ranking = 7, Pais = "Grecia", GrandSlams = 0 },
            new Tenista { Id = 8, Nombre = "Holger Rune", Ranking = 8, Pais = "Dinamarca", GrandSlams = 0 }
        };

        // Inicializar subjects
        _publishSubject = new Subject<Tenista>();
        _behaviorSubject = new BehaviorSubject<Tenista>(_tenistas[0]); // Con valor inicial
        _replaySubject = new ReplaySubject<Tenista>(3); // Buffer de 3 elementos
    }

    // ==================================================================================
    // COLD OBSERVABLES - Cada suscriptor recibe la secuencia completa desde el inicio
    // ==================================================================================

    /// <summary>
    /// Cold Observable básico - Cada suscriptor recibe todos los eventos
    /// En Java: Observable.create() con emisiones dentro
    /// </summary>
    public IObservable<Tenista> GetColdObservable()
    {
        return Observable.Create<Tenista>(async observer =>
        {
            System.Console.WriteLine("  [Cold Observable] Iniciando emisión para nuevo suscriptor");
            foreach (var tenista in _tenistas)
            {
                await Task.Delay(100);
                observer.OnNext(tenista);
            }
            observer.OnCompleted();
            return System.Reactive.Disposables.Disposable.Empty;
        });
    }

    // ==================================================================================
    // HOT OBSERVABLES - Todos los suscriptores comparten el mismo stream
    // ==================================================================================

    /// <summary>
    /// PublishSubject - Hot Observable sin buffer
    /// En Java: PublishSubject.create()
    /// Los suscriptores solo reciben eventos posteriores a la suscripción
    /// </summary>
    public IObservable<Tenista> GetPublishSubject() => _publishSubject.AsObservable();
    public void EmitirTenista(Tenista tenista) => _publishSubject.OnNext(tenista);

    /// <summary>
    /// BehaviorSubject - Hot Observable con último valor
    /// En Java: BehaviorSubject.create()
    /// Los nuevos suscriptores reciben el último valor emitido
    /// </summary>
    public IObservable<Tenista> GetBehaviorSubject() => _behaviorSubject.AsObservable();
    public void ActualizarTenistaActual(Tenista tenista) => _behaviorSubject.OnNext(tenista);

    /// <summary>
    /// ReplaySubject - Hot Observable con buffer
    /// En Java: ReplaySubject.create(bufferSize)
    /// Los nuevos suscriptores reciben los últimos N valores
    /// </summary>
    public IObservable<Tenista> GetReplaySubject() => _replaySubject.AsObservable();
    public void AgregarAlReplay(Tenista tenista) => _replaySubject.OnNext(tenista);

    // ==================================================================================
    // SCHEDULERS - Control de concurrencia y threading
    // ==================================================================================

    /// <summary>
    /// Observable con Scheduler de I/O
    /// En Java: Observable.subscribeOn(Schedulers.io())
    /// En C#: Observable.SubscribeOn(TaskPoolScheduler.Default)
    /// </summary>
    public IObservable<Tenista> GetObservableConScheduler()
    {
        return Observable.Create<Tenista>(async observer =>
        {
            System.Console.WriteLine($"  [Scheduler] Ejecutando en Thread: {Environment.CurrentManagedThreadId}");
            foreach (var tenista in _tenistas.Take(3))
            {
                await Task.Delay(200);
                observer.OnNext(tenista);
            }
            observer.OnCompleted();
            return System.Reactive.Disposables.Disposable.Empty;
        })
        .SubscribeOn(TaskPoolScheduler.Default); // Ejecutar en thread pool
    }

    /// <summary>
    /// Observable con ObserveOn para cambiar contexto de observación
    /// En Java: Observable.observeOn(Schedulers.computation())
    /// En C#: Observable.ObserveOn(NewThreadScheduler.Default)
    /// </summary>
    public IObservable<Tenista> GetObservableConObserveOn()
    {
        return _tenistas.Take(3).ToObservable()
            .Do(t => System.Console.WriteLine($"  [Emit] Thread: {Environment.CurrentManagedThreadId} - {t.Nombre}"))
            .ObserveOn(NewThreadScheduler.Default)
            .Do(t => System.Console.WriteLine($"  [Observe] Thread: {Environment.CurrentManagedThreadId} - {t.Nombre}"));
    }

    // ==================================================================================
    // OPERADORES AVANZADOS
    // ==================================================================================

    /// <summary>
    /// Buffer - Acumula elementos en lotes
    /// En Java: Observable.buffer(count)
    /// En C#: Observable.Buffer(count)
    /// </summary>
    public IObservable<IList<Tenista>> GetBufferedObservable(int bufferSize)
    {
        return _tenistas.ToObservable()
            .Buffer(bufferSize);
    }

    /// <summary>
    /// Window - Similar a Buffer pero emite observables
    /// En Java: Observable.window(count)
    /// En C#: Observable.Window(count)
    /// </summary>
    public IObservable<IObservable<Tenista>> GetWindowedObservable(int windowSize)
    {
        return _tenistas.ToObservable()
            .Window(windowSize);
    }

    /// <summary>
    /// Throttle - Solo emite si no hay eventos en X tiempo
    /// En Java: Observable.debounce()
    /// En C#: Observable.Throttle()
    /// </summary>
    public IObservable<Tenista> GetThrottledObservable(TimeSpan throttleTime)
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(100))
            .Take(_tenistas.Count)
            .Select(i => _tenistas[(int)i])
            .Throttle(throttleTime);
    }

    /// <summary>
    /// Sample - Emite el último valor en intervalos regulares
    /// En Java: Observable.sample()
    /// En C#: Observable.Sample()
    /// </summary>
    public IObservable<Tenista> GetSampledObservable(TimeSpan sampleInterval)
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(50))
            .Take(_tenistas.Count * 5)
            .Select(i => _tenistas[(int)(i % _tenistas.Count)])
            .Sample(sampleInterval);
    }

    /// <summary>
    /// Scan - Acumula valores (como reduce pero emite intermedios)
    /// En Java: Observable.scan()
    /// En C#: Observable.Scan()
    /// </summary>
    public IObservable<int> GetTotalGrandSlamsAcumulado()
    {
        return _tenistas.ToObservable()
            .Select(t => t.GrandSlams)
            .Scan(0, (acc, gs) => acc + gs);
    }

    /// <summary>
    /// CombineLatest - Combina los últimos valores de múltiples observables
    /// En Java: Observable.combineLatest()
    /// En C#: Observable.CombineLatest()
    /// </summary>
    public IObservable<string> CombinarTenistas()
    {
        var observable1 = _tenistas.Take(3).ToObservable();
        var observable2 = _tenistas.Skip(3).Take(3).ToObservable();

        return observable1.CombineLatest(observable2, (t1, t2) => 
            $"{t1.Nombre} vs {t2.Nombre}");
    }

    /// <summary>
    /// Zip - Combina elementos por posición
    /// En Java: Observable.zip()
    /// En C#: Observable.Zip()
    /// </summary>
    public IObservable<string> ZipTenistas()
    {
        var observable1 = _tenistas.Take(4).ToObservable();
        var observable2 = _tenistas.Skip(4).Take(4).ToObservable();

        return observable1.Zip(observable2, (t1, t2) => 
            $"Partido: {t1.Nombre} vs {t2.Nombre}");
    }

    /// <summary>
    /// Merge - Combina múltiples observables en uno
    /// En Java: Observable.merge()
    /// En C#: Observable.Merge()
    /// </summary>
    public IObservable<Tenista> MergeTenistas()
    {
        var observable1 = _tenistas.Take(3).ToObservable().Delay(TimeSpan.FromMilliseconds(100));
        var observable2 = _tenistas.Skip(3).Take(3).ToObservable();

        return observable1.Merge(observable2);
    }

    /// <summary>
    /// Concat - Concatena observables secuencialmente
    /// En Java: Observable.concat()
    /// En C#: Observable.Concat()
    /// </summary>
    public IObservable<Tenista> ConcatenarTenistas()
    {
        var observable1 = _tenistas.Take(3).ToObservable();
        var observable2 = _tenistas.Skip(3).Take(3).ToObservable();

        return observable1.Concat(observable2);
    }

    /// <summary>
    /// Distinct - Elimina duplicados
    /// En Java: Observable.distinct()
    /// En C#: Observable.Distinct()
    /// </summary>
    public IObservable<string> GetPaisesUnicos()
    {
        return _tenistas.ToObservable()
            .Select(t => t.Pais)
            .Distinct();
    }

    /// <summary>
    /// DistinctUntilChanged - Solo emite si el valor cambió respecto al anterior
    /// En Java: Observable.distinctUntilChanged()
    /// En C#: Observable.DistinctUntilChanged()
    /// </summary>
    public IObservable<string> GetCambiosDePais()
    {
        return _tenistas.Concat(_tenistas).ToObservable()
            .Select(t => t.Pais)
            .DistinctUntilChanged();
    }

    /// <summary>
    /// Retry - Reintenta en caso de error
    /// En Java: Observable.retry()
    /// En C#: Observable.Retry()
    /// </summary>
    public IObservable<Tenista> GetObservableConRetry(int maxRetries)
    {
        var intentos = 0;
        return Observable.Create<Tenista>(observer =>
        {
            intentos++;
            System.Console.WriteLine($"  [Retry] Intento #{intentos}");
            
            if (intentos < 3)
            {
                observer.OnError(new Exception("Simulando error"));
            }
            else
            {
                foreach (var tenista in _tenistas.Take(2))
                {
                    observer.OnNext(tenista);
                }
                observer.OnCompleted();
            }
            return System.Reactive.Disposables.Disposable.Empty;
        })
        .Retry(maxRetries);
    }

    /// <summary>
    /// Timeout - Falla si no hay eventos en X tiempo
    /// En Java: Observable.timeout()
    /// En C#: Observable.Timeout()
    /// </summary>
    public IObservable<Tenista> GetObservableConTimeout(TimeSpan timeout)
    {
        return Observable.Create<Tenista>(async observer =>
        {
            foreach (var tenista in _tenistas.Take(3))
            {
                await Task.Delay(500); // Delay largo para probar timeout
                observer.OnNext(tenista);
            }
            observer.OnCompleted();
            return System.Reactive.Disposables.Disposable.Empty;
        })
        .Timeout(timeout);
    }
}

using Cassandra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductosCassandra.Config;
using Serilog;
using ISession = Cassandra.ISession;

namespace ProductosCassandra.Infrastructures;

/// <summary>
/// Configuracion de la conexion a Cassandra.
/// </summary>
public static class CassandraConfig
{
    /// <summary>
    /// Registra el cluster y la sesion de Cassandra en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Coleccion de servicios.</param>
    /// <param name="configuration">Configuracion de la app.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddCassandra(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(CassandraSettings.SectionName)
            .Get<CassandraSettings>() ?? new CassandraSettings();

        Log.Information("Conectando a Cassandra: {Hosts}:{Port}",
            string.Join(",", settings.Hosts), settings.Port);

        // El cluster y la sesion son caros de crear y son thread-safe:
        // se crean una vez y viven todo el proceso (singleton).
        // El ISession multiplexa las peticiones; no hay un "pool de contexts" como en EF Core.
        var cluster = Cluster.Builder()
            .AddContactPoints(settings.Hosts)
            .WithPort(settings.Port)
            .Build();

        // La primera conexion puede llegar antes de que el nodo despierte
        // (Cassandra tarda ~30-60 s en aceptar conexiones): se reintenta igual
        // que luego con el esquema, en vez de tumbar el arranque de la app.
        ISession session;
        for (var intento = 1; ; intento++)
        {
            try
            {
                session = cluster.Connect();
                break;
            }
            catch (NoHostAvailableException) when (intento < 30)
            {
                Log.Information("Cassandra aun no acepta conexiones (intento {Intento}/30)...", intento);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        services.AddSingleton(settings);
        services.AddSingleton<ISession>(session);

        return services;
    }
}

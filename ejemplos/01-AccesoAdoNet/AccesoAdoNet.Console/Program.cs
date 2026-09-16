using AccesoAdoNet.Console.Database;
using AccesoAdoNet.Console.Models;
using AccesoAdoNet.Console.Repositories;

// ==================================================================================
// 🚀 EJEMPLO 01: ACCESO A DATOS CON ADO.NET (equivalente a JDBC en Java)
// ==================================================================================
// 
// Este ejemplo demuestra el acceso manual a bases de datos usando ADO.NET,
// que es el equivalente de bajo nivel a JDBC en Java.
//
// CONCEPTOS JAVA → C#:
// - JDBC → ADO.NET
// - Connection → NpgsqlConnection
// - PreparedStatement → NpgsqlCommand
// - ResultSet → NpgsqlDataReader
// - DriverManager.getConnection() → new NpgsqlConnection()
//
// ADO.NET proporciona control total sobre SQL y conexiones, similar a JDBC.
// Para un ORM de alto nivel (como JPA), ver ejemplo 02-AccesoEntityFramework
// ==================================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  📊 EJEMPLO ADO.NET - Acceso Manual a Base de Datos (JDBC → C#)    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Configuración de la base de datos
// En Java sería: String url = "jdbc:postgresql://localhost:5432/tenistas_db";
var connectionString = "Host=localhost;Port=5432;Database=tenistas_db;Username=admin;Password=admin123";

// Crear el DatabaseManager (equivalente a obtener Connection en Java)
using var dbManager = new DatabaseManager(connectionString);

try
{
    Console.WriteLine("🔌 Conectando a PostgreSQL...");
    await dbManager.InitializeDatabaseAsync();
    await dbManager.ClearDatabaseAsync();
    
    // Crear repositorio (patrón Repository para abstraer JDBC/ADO.NET)
    var repository = new TenistaRepository(dbManager);

    // ==================================================================================
    // OPERACIÓN 1: CREATE (INSERT)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("📝 OPERACIÓN 1: CREATE - Insertar tenistas");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var tenistas = new List<Tenista>
    {
        new() { Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Altura = 185, Peso = 85, Titulos = 22, FechaNacimiento = new DateTime(1986, 6, 3) },
        new() { Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Altura = 188, Peso = 77, Titulos = 24, FechaNacimiento = new DateTime(1987, 5, 22) },
        new() { Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Altura = 183, Peso = 80, Titulos = 2, FechaNacimiento = new DateTime(2003, 5, 5) },
        new() { Nombre = "Roger Federer", Ranking = 4, Pais = "Suiza", Altura = 185, Peso = 85, Titulos = 20, FechaNacimiento = new DateTime(1981, 8, 8) }
    };

    foreach (var tenista in tenistas)
    {
        await repository.CreateAsync(tenista);
    }

    // ==================================================================================
    // OPERACIÓN 2: READ ALL (SELECT *)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("📖 OPERACIÓN 2: READ ALL - Obtener todos los tenistas");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var todosLosTenistas = await repository.FindAllAsync();
    Console.WriteLine($"📊 Total de tenistas: {todosLosTenistas.Count}");
    foreach (var t in todosLosTenistas)
    {
        Console.WriteLine($"  → {t}");
    }

    // ==================================================================================
    // OPERACIÓN 3: READ BY ID (SELECT WHERE id = ?)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🔍 OPERACIÓN 3: READ BY ID - Buscar tenista por ID");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var tenista1 = await repository.FindByIdAsync(1);
    if (tenista1 != null)
    {
        Console.WriteLine($"✅ Encontrado: {tenista1}");
    }

    // ==================================================================================
    // OPERACIÓN 4: READ BY FILTER (SELECT WHERE pais = ?)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🇪🇸 OPERACIÓN 4: READ BY FILTER - Buscar tenistas españoles");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var tenistasEspañoles = await repository.FindByPaisAsync("España");
    Console.WriteLine($"📊 Tenistas españoles: {tenistasEspañoles.Count}");
    foreach (var t in tenistasEspañoles)
    {
        Console.WriteLine($"  → {t.Nombre} (Ranking: {t.Ranking})");
    }

    // ==================================================================================
    // OPERACIÓN 5: UPDATE
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("✏️  OPERACIÓN 5: UPDATE - Actualizar tenista");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    if (tenista1 != null)
    {
        Console.WriteLine($"Antes: {tenista1}");
        tenista1.Titulos = 23;  // Actualizar títulos
        tenista1.Ranking = 1;
        await repository.UpdateAsync(tenista1);
        
        // Verificar actualización
        var tenistaActualizado = await repository.FindByIdAsync(tenista1.Id);
        Console.WriteLine($"Después: {tenistaActualizado}");
    }

    // ==================================================================================
    // OPERACIÓN 6: COUNT (Aggregate function)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🔢 OPERACIÓN 6: COUNT - Contar registros");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var totalTenistas = await repository.CountAsync();
    Console.WriteLine($"📊 Total de tenistas en la base de datos: {totalTenistas}");

    // ==================================================================================
    // OPERACIÓN 7: DELETE
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🗑️  OPERACIÓN 7: DELETE - Eliminar tenista");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    
    var deleted = await repository.DeleteAsync(4);
    Console.WriteLine(deleted ? "✅ Tenista eliminado correctamente" : "❌ Tenista no encontrado");
    
    // Verificar eliminación
    var tenistasFinal = await repository.FindAllAsync();
    Console.WriteLine($"📊 Tenistas restantes: {tenistasFinal.Count}");

    // ==================================================================================
    // RESUMEN Y COMPARACIÓN JAVA → C#
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║  📚 COMPARACIÓN JAVA (JDBC) vs C# (ADO.NET)                         ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("┌─────────────────────────────┬─────────────────────────────────────┐");
    Console.WriteLine("│ JAVA (JDBC)                 │ C# (ADO.NET)                        │");
    Console.WriteLine("├─────────────────────────────┼─────────────────────────────────────┤");
    Console.WriteLine("│ Connection                  │ NpgsqlConnection                    │");
    Console.WriteLine("│ PreparedStatement           │ NpgsqlCommand                       │");
    Console.WriteLine("│ ResultSet                   │ NpgsqlDataReader                    │");
    Console.WriteLine("│ DriverManager.getConnection │ new NpgsqlConnection()              │");
    Console.WriteLine("│ ps.executeQuery()           │ command.ExecuteReaderAsync()        │");
    Console.WriteLine("│ ps.executeUpdate()          │ command.ExecuteNonQueryAsync()      │");
    Console.WriteLine("│ rs.getString(\"nombre\")      │ reader.GetString(\"nombre\")          │");
    Console.WriteLine("│ rs.next()                   │ await reader.ReadAsync()            │");
    Console.WriteLine("│ try-with-resources          │ using / await using                 │");
    Console.WriteLine("└─────────────────────────────┴─────────────────────────────────────┘");
    Console.WriteLine();
    Console.WriteLine("💡 VENTAJAS de ADO.NET:");
    Console.WriteLine("  ✅ Control total sobre SQL y conexiones");
    Console.WriteLine("  ✅ Alto rendimiento (sin overhead de ORM)");
    Console.WriteLine("  ✅ Ideal para consultas complejas o stored procedures");
    Console.WriteLine("  ✅ Similar a JDBC, fácil de entender viniendo de Java");
    Console.WriteLine();
    Console.WriteLine("⚠️  DESVENTAJAS de ADO.NET:");
    Console.WriteLine("  ⚠️  Mapeo manual de resultados a objetos");
    Console.WriteLine("  ⚠️  Más código boilerplate que un ORM");
    Console.WriteLine("  ⚠️  Mayor riesgo de errores SQL");
    Console.WriteLine();
    Console.WriteLine("👉 Para un ORM completo (como JPA), ver ejemplo 02-AccesoEntityFramework");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"   Asegúrate de que PostgreSQL esté ejecutándose:");
    Console.WriteLine($"   docker run -d -p 5432:5432 -e POSTGRES_DB=tenistas_db -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin123 postgres:15");
}

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✨ Ejemplo completado - ADO.NET vs JDBC                            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

// ==================================================================================
// 👨‍💻 Autor: José Luis González Sánchez
// 📧 Email: joseluis.gonzalez@profesor.com
// 🌐 Web: https://joseluisgs.dev
// 📅 Fecha: Octubre 2025
// 📝 Licencia: Creative Commons BY-NC-SA 4.0
// ==================================================================================

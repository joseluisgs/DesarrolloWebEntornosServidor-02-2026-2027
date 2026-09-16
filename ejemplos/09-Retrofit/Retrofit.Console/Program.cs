using Refit;
using Retrofit.Console.Services;
using Retrofit.Console.Models;

// ==================================================================================
// 🚀 EJEMPLO 09: HTTP CLIENTS DECLARATIVOS - Retrofit → Refit
// ==================================================================================

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  🌐 EJEMPLO REFIT - Retrofit (Java) → Refit (C#)                  ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();

// Crear cliente Refit
var api = RestService.For<IJsonPlaceholderApi>("https://jsonplaceholder.typicode.com");

// ==================================================================================
// DEMO 1: GET - OBTENER RECURSOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📥 DEMO 1: GET - Obtener Recursos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    System.Console.WriteLine("1.1. Obtener todos los usuarios:");
    var users = await api.GetUsers();
    foreach (var user in users.Take(3))
    {
        System.Console.WriteLine($"  → {user}");
    }
    System.Console.WriteLine($"  ... ({users.Count} usuarios en total)");
    System.Console.WriteLine();

    System.Console.WriteLine("1.2. Obtener usuario específico (ID: 1):");
    var user1 = await api.GetUser(1);
    System.Console.WriteLine($"  → {user1}");
    System.Console.WriteLine($"     Ciudad: {user1.Address?.City}");
    System.Console.WriteLine($"     Compañía: {user1.Company?.Name}");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 2: GET CON QUERY PARAMETERS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔍 DEMO 2: GET con Query Parameters");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    System.Console.WriteLine("2.1. Obtener posts del usuario 1:");
    var userPosts = await api.GetUserPosts(1);
    foreach (var post in userPosts.Take(3))
    {
        System.Console.WriteLine($"  → {post}");
    }
    System.Console.WriteLine($"  ... ({userPosts.Count} posts en total)");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 3: GET CON PATH PARAMETERS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🎯 DEMO 3: GET con Path Parameters");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    System.Console.WriteLine("3.1. Obtener post específico (ID: 1):");
    var post = await api.GetPost(1);
    System.Console.WriteLine($"  → {post}");
    System.Console.WriteLine($"     Usuario ID: {post.UserId}");
    System.Console.WriteLine($"     Contenido: {post.Body.Substring(0, Math.Min(50, post.Body.Length))}...");
    System.Console.WriteLine();

    System.Console.WriteLine("3.2. Obtener comentarios del post:");
    var comments = await api.GetComments(1);
    foreach (var comment in comments.Take(3))
    {
        System.Console.WriteLine($"  → {comment}");
    }
    System.Console.WriteLine($"  ... ({comments.Count} comentarios en total)");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 4: POST - CREAR RECURSOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("📤 DEMO 4: POST - Crear Recursos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    var newPost = new Post
    {
        UserId = 1,
        Title = "Aprendiendo Refit en C#",
        Body = "Este es un ejemplo de cómo usar Refit para hacer llamadas HTTP declarativas en C#, similar a Retrofit en Java."
    };

    System.Console.WriteLine("4.1. Creando nuevo post...");
    System.Console.WriteLine($"  Título: {newPost.Title}");
    var createdPost = await api.CreatePost(newPost);
    System.Console.WriteLine($"  ✓ Post creado con ID: {createdPost.Id}");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 5: PUT - ACTUALIZAR RECURSOS COMPLETOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔄 DEMO 5: PUT - Actualizar Recursos Completos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    var updatedPost = new Post
    {
        UserId = 1,
        Id = 1,
        Title = "Post Actualizado con PUT",
        Body = "Este post ha sido completamente actualizado usando el método PUT de Refit."
    };

    System.Console.WriteLine("5.1. Actualizando post completo (ID: 1)...");
    var result = await api.UpdatePost(1, updatedPost);
    System.Console.WriteLine($"  ✓ Post actualizado: {result.Title}");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 6: PATCH - ACTUALIZAR RECURSOS PARCIALMENTE
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("✏️  DEMO 6: PATCH - Actualizar Recursos Parcialmente");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    var partialUpdate = new Dictionary<string, object>
    {
        { "title", "Título Parcialmente Actualizado" }
    };

    System.Console.WriteLine("6.1. Actualizando solo el título del post (ID: 1)...");
    var result = await api.PatchPost(1, partialUpdate);
    System.Console.WriteLine($"  ✓ Título actualizado: {result.Title}");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 7: DELETE - ELIMINAR RECURSOS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🗑️  DEMO 7: DELETE - Eliminar Recursos");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    System.Console.WriteLine("7.1. Eliminando post (ID: 1)...");
    await api.DeletePost(1);
    System.Console.WriteLine("  ✓ Post eliminado exitosamente");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// DEMO 8: BÚSQUEDA CON MÚLTIPLES PARÁMETROS
// ==================================================================================
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine("🔎 DEMO 8: Búsqueda con Múltiples Parámetros");
System.Console.WriteLine("═══════════════════════════════════════════════════════════════");
System.Console.WriteLine();

try
{
    System.Console.WriteLine("8.1. Buscar comentarios del post 1:");
    var searchResults = await api.SearchComments(postId: 1);
    foreach (var comment in searchResults.Take(2))
    {
        System.Console.WriteLine($"  → {comment}");
    }
    System.Console.WriteLine($"  ... ({searchResults.Count} resultados)");
    System.Console.WriteLine();
}
catch (Exception ex)
{
    System.Console.WriteLine($"  ✗ Error: {ex.Message}");
    System.Console.WriteLine();
}

// ==================================================================================
// RESUMEN COMPARATIVO
// ==================================================================================
System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  📚 COMPARACIÓN Retrofit (Java) vs Refit (C#)                      ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
System.Console.WriteLine();
System.Console.WriteLine("┌────────────────────────────────┬────────────────────────────────┐");
System.Console.WriteLine("│ JAVA (Retrofit)                │ C# (Refit)                     │");
System.Console.WriteLine("├────────────────────────────────┼────────────────────────────────┤");
System.Console.WriteLine("│ @GET(\"/users\")                 │ [Get(\"/users\")]                │");
System.Console.WriteLine("│ @POST(\"/posts\")                │ [Post(\"/posts\")]               │");
System.Console.WriteLine("│ @PUT(\"/posts/{id}\")            │ [Put(\"/posts/{id}\")]           │");
System.Console.WriteLine("│ @PATCH(\"/posts/{id}\")          │ [Patch(\"/posts/{id}\")]         │");
System.Console.WriteLine("│ @DELETE(\"/posts/{id}\")         │ [Delete(\"/posts/{id}\")]        │");
System.Console.WriteLine("│ @Path(\"id\")                    │ int id (parámetro del método)  │");
System.Console.WriteLine("│ @Query(\"userId\")               │ [Query] int userId             │");
System.Console.WriteLine("│ @QueryMap Map<K,V>             │ [Query] Dictionary<K,V>        │");
System.Console.WriteLine("│ @Body User user                │ [Body] User user               │");
System.Console.WriteLine("│ @Header(\"Authorization\")       │ [Header(\"Authorization\")]      │");
System.Console.WriteLine("│ @Headers({\"Accept: json\"})     │ [Headers(\"Accept: json\")]      │");
System.Console.WriteLine("│ Call<User>                     │ Task<User>                     │");
System.Console.WriteLine("│ Retrofit.Builder()             │ RestService.For<T>(url)        │");
System.Console.WriteLine("│ .addConverterFactory(Gson)     │ (incluido por defecto)         │");
System.Console.WriteLine("└────────────────────────────────┴────────────────────────────────┘");
System.Console.WriteLine();
System.Console.WriteLine("💡 VENTAJAS de Refit:");
System.Console.WriteLine("  ✅ Sintaxis declarativa (atributos en interfaz)");
System.Console.WriteLine("  ✅ Integración con async/await nativo");
System.Console.WriteLine("  ✅ Type-safe en tiempo de compilación");
System.Console.WriteLine("  ✅ Menos configuración inicial");
System.Console.WriteLine("  ✅ Soporte para HttpClientFactory");
System.Console.WriteLine("  ✅ Serialización JSON automática");
System.Console.WriteLine();
System.Console.WriteLine("🎯 CUÁNDO USAR Refit:");
System.Console.WriteLine("  📌 APIs REST con contratos claros");
System.Console.WriteLine("  📌 Microservicios y comunicación HTTP");
System.Console.WriteLine("  📌 Integración con APIs de terceros");
System.Console.WriteLine("  📌 Cuando quieres reducir boilerplate HTTP");
System.Console.WriteLine("  📌 Testing con mocks de interfaces");
System.Console.WriteLine();

System.Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
System.Console.WriteLine("║  ✨ Ejemplo completado - Refit HTTP Client                          ║");
System.Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

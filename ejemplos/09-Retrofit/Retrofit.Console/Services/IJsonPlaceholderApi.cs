using Refit;
using Retrofit.Console.Models;

namespace Retrofit.Console.Services;

/// <summary>
/// API declarativa usando Refit
/// Equivalente a Retrofit en Java/Kotlin
/// 
/// En Java (Retrofit):
/// @GET("/users")
/// Call<List<User>> getUsers();
/// 
/// En C# (Refit):
/// [Get("/users")]
/// Task<List<User>> GetUsers();
/// </summary>
[Headers("User-Agent: Refit-CSharp-Example")]
public interface IJsonPlaceholderApi
{
    /// <summary>
    /// Obtener todos los usuarios
    /// En Java: @GET("users") Call<List<User>> getUsers()
    /// </summary>
    [Get("/users")]
    Task<List<User>> GetUsers();

    /// <summary>
    /// Obtener usuario por ID
    /// En Java: @GET("users/{id}") Call<User> getUser(@Path("id") int id)
    /// </summary>
    [Get("/users/{id}")]
    Task<User> GetUser(int id);

    /// <summary>
    /// Obtener posts
    /// En Java: @GET("posts") Call<List<Post>> getPosts()
    /// </summary>
    [Get("/posts")]
    Task<List<Post>> GetPosts();

    /// <summary>
    /// Obtener post por ID
    /// En Java: @GET("posts/{id}") Call<Post> getPost(@Path("id") int id)
    /// </summary>
    [Get("/posts/{id}")]
    Task<Post> GetPost(int id);

    /// <summary>
    /// Obtener posts de un usuario
    /// En Java: @GET("posts") Call<List<Post>> getUserPosts(@Query("userId") int userId)
    /// </summary>
    [Get("/posts")]
    Task<List<Post>> GetUserPosts([Query] int userId);

    /// <summary>
    /// Obtener comentarios de un post
    /// En Java: @GET("posts/{postId}/comments") Call<List<Comment>> getComments(@Path("postId") int postId)
    /// </summary>
    [Get("/posts/{postId}/comments")]
    Task<List<Comment>> GetComments(int postId);

    /// <summary>
    /// Crear nuevo post
    /// En Java: @POST("posts") Call<Post> createPost(@Body Post post)
    /// </summary>
    [Post("/posts")]
    Task<Post> CreatePost([Body] Post post);

    /// <summary>
    /// Actualizar post completo
    /// En Java: @PUT("posts/{id}") Call<Post> updatePost(@Path("id") int id, @Body Post post)
    /// </summary>
    [Put("/posts/{id}")]
    Task<Post> UpdatePost(int id, [Body] Post post);

    /// <summary>
    /// Actualizar post parcialmente
    /// En Java: @PATCH("posts/{id}") Call<Post> patchPost(@Path("id") int id, @Body Map<String, Object> fields)
    /// </summary>
    [Patch("/posts/{id}")]
    Task<Post> PatchPost(int id, [Body] Dictionary<string, object> fields);

    /// <summary>
    /// Eliminar post
    /// En Java: @DELETE("posts/{id}") Call<Void> deletePost(@Path("id") int id)
    /// </summary>
    [Delete("/posts/{id}")]
    Task DeletePost(int id);

    /// <summary>
    /// Búsqueda con múltiples parámetros
    /// En Java: @GET("comments") Call<List<Comment>> searchComments(@QueryMap Map<String, String> params)
    /// </summary>
    [Get("/comments")]
    Task<List<Comment>> SearchComments([Query] int? postId = null, [Query] string? email = null);

    /// <summary>
    /// Headers personalizados
    /// En Java: @GET("users") @Headers("Authorization: Bearer {token}") Call<List<User>> getUsersAuth(@Header("Authorization") String auth)
    /// </summary>
    [Get("/users")]
    [Headers("Accept: application/json")]
    Task<List<User>> GetUsersWithHeaders([Header("Authorization")] string authorization);
}

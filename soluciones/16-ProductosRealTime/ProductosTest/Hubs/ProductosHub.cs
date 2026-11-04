using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace ProductosEFCore.Hubs;

/// <summary>
/// Hub principal de productos en tiempo real.
/// Protegido con [Authorize]: solo usuarios autenticados pueden conectarse.
/// </summary>
[Authorize]
public class ProductosHub : Hub
{
    private static readonly ConcurrentDictionary<string, int> _messageCounts = new();
    private const int MaxMessagesPerConnection = 100;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = Context.User?.FindFirstValue(ClaimTypes.Email);
        var isAdmin = Context.User?.IsInRole("ADMIN") ?? false;

        Log.Information("Usuario conectado: {Email} (ID: {UserId})", email, userId);

        // Grupo privado: solo este usuario recibe sus notificaciones
        if (userId is not null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            Log.Debug("Usuario {UserId} agregado a grupo user-{UserId}", userId, userId);
        }

        // Grupo de admins: todas las notificaciones de administracion
        if (isAdmin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
            Log.Debug("Usuario {UserId} agregado a grupo admins", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        Log.Information("Usuario desconectado: {UserId}", userId);

        // Eliminar contador de mensajes al desconectar
        if (userId is not null)
        {
            _messageCounts.TryRemove(userId, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Enviar mensaje a todos los conectados.
    /// </summary>
    public async Task SendMessage(string mensaje)
    {
        if (!TryIncrementMessageCount(out var count))
        {
            await Clients.Caller.SendAsync("Error", "Limite de mensajes alcanzado (max 100)");
            return;
        }

        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = Context.User!.FindFirstValue(ClaimTypes.Email);

        await Clients.All.SendAsync("ReceiveMessage", email, mensaje, DateTime.UtcNow);
        Log.Debug("Mensaje enviado por {Email}: {Mensaje} (count: {Count})", email, mensaje, count);
    }

    /// <summary>
    /// Unirse a un grupo de categoria de producto.
    /// </summary>
    public async Task JoinCategoriaGroup(long categoriaId)
    {
        var groupName = $"categoria-{categoriaId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        Log.Debug("Conexion {ConnectionId} unida a grupo {Group}", Context.ConnectionId, groupName);

        await Clients.Caller.SendAsync("JoinedGroup", groupName);
    }

    /// <summary>
    /// Salir de un grupo de categoria.
    /// </summary>
    public async Task LeaveCategoriaGroup(long categoriaId)
    {
        var groupName = $"categoria-{categoriaId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        Log.Debug("Conexion {ConnectionId} eliminada del grupo {Group}", Context.ConnectionId, groupName);

        await Clients.Caller.SendAsync("LeftGroup", groupName);
    }

    /// <summary>
    /// Ping para mantener la conexion activa.
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
    }

    private bool TryIncrementMessageCount(out int count)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        count = _messageCounts.AddOrUpdate(userId, 1, (_, c) => c + 1);
        if (count <= MaxMessagesPerConnection) return true;

        // Si excede, decrementar y retornar false
        _messageCounts.AddOrUpdate(userId, MaxMessagesPerConnection, (_, c) => c - 1);
        count = MaxMessagesPerConnection;
        return false;
    }
}

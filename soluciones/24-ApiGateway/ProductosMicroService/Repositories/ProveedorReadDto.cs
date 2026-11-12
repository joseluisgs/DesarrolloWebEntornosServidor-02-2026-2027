using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// DTO de respuesta del Proveedor remoto.
/// </summary>
public record ProveedorReadDto(string Id, string Nombre, string Email, string Telefono);

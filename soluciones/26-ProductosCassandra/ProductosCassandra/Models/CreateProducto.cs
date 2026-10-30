namespace ProductosCassandra.Models;

/// <summary>
/// Modelo de creación de producto en memoria, sin la categoría resuelta.
/// El id lo aporta la aplicación: en Cassandra no hay autoincremento.
/// </summary>
public record CreateProducto(long Id, string Nombre, double Precio);

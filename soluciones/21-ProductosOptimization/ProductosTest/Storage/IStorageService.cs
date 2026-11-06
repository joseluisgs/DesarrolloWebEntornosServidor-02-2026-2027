using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Storage;

/// <summary>
/// Interfaz para operaciones de almacenamiento de archivos.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Guarda un archivo en el sistema de almacenamiento.
    /// </summary>
    /// <param name="file">Archivo a guardar.</param>
    /// <param name="folder">Carpeta destino (productos, usuarios, categorias).</param>
    /// <returns>Ruta relativa del archivo guardado.</returns>
    Task<Result<string, DomainError>> SaveFileAsync(IFormFile file, string folder);

    /// <summary>
    /// Elimina un archivo del sistema de almacenamiento.
    /// </summary>
    /// <param name="filename">Ruta relativa del archivo a eliminar.</param>
    /// <returns>True si se elimino, false si no existia.</returns>
    Task<Result<bool, DomainError>> DeleteFileAsync(string filename);

    /// <summary>
    /// Verifica si un archivo existe.
    /// </summary>
    /// <param name="filename">Ruta relativa o nombre del archivo.</param>
    /// <returns>True si existe, false en caso contrario.</returns>
    bool FileExists(string filename);

    /// <summary>
    /// Obtiene la ruta fisica completa de un archivo.
    /// </summary>
    /// <param name="filename">Nombre del archivo o ruta relativa.</param>
    /// <returns>Ruta completa en el sistema de archivos.</returns>
    string GetFullPath(string filename);

    /// <summary>
    /// Genera una ruta relativa formateada para almacenar en BD.
    /// </summary>
    /// <param name="filename">Nombre del archivo.</param>
    /// <param name="folder">Carpeta dentro del storage.</param>
    /// <returns>Ruta relativa formateada: /uploads/{carpeta}/{nombre}</returns>
    string GetRelativePath(string filename, string folder = "productos");
}

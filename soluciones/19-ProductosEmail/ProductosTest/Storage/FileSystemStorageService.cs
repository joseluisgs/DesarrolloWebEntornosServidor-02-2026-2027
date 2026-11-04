using CSharpFunctionalExtensions;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Storage;

/// <summary>
/// Implementacion de IStorageService que almacena ficheros en el sistema de archivos local.
/// Los ficheros se guardan en wwwroot/{Storage:UploadPath}/{folder} para acceso web directo.
/// </summary>
public class FileSystemStorageService : IStorageService
{
    private readonly string _rootPath;
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;
    private readonly string[] _allowedContentTypes;
    private readonly ILogger<FileSystemStorageService> _logger;

    public FileSystemStorageService(IConfiguration configuration, ILogger<FileSystemStorageService> logger, IWebHostEnvironment env)
    {
        _logger = logger;

        _uploadPath = configuration["Storage:UploadPath"] ?? "uploads";
        _maxFileSize = configuration.GetValue<long>("Storage:MaxFileSize", 5 * 1024 * 1024);
        _allowedExtensions = configuration.GetSection("Storage:AllowedExtensions").Get<string[]>()
            ?? [".jpg", ".jpeg", ".png", ".gif"];
        _allowedContentTypes = configuration.GetSection("Storage:AllowedContentTypes").Get<string[]>()
            ?? ["image/jpeg", "image/png", "image/gif"];

        var webRoot = env.WebRootPath ?? env.ContentRootPath;
        _rootPath = System.IO.Path.Combine(webRoot, _uploadPath);

        if (!Directory.Exists(_rootPath))
            Directory.CreateDirectory(_rootPath);

        _logger.LogInformation("Storage service inicializado en: {Path}", _rootPath);
    }

    private static string GenerateUniqueFilename(string originalFilename)
    {
        var extension = System.IO.Path.GetExtension(originalFilename).ToLowerInvariant();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var sanitizedName = System.IO.Path.GetFileNameWithoutExtension(originalFilename)
            .Replace(" ", "_").Replace("-", "_");
        return $"{timestamp}_{uniqueId}_{sanitizedName}{extension}";
    }

    private UnitResult<DomainError> ValidateFile(IFormFile file)
    {
        if (file is null or { Length: 0 })
            return UnitResult.Failure<DomainError>(StorageError.ArchivoVacio());

        if (file.Length > _maxFileSize)
            return UnitResult.Failure<DomainError>(StorageError.ArchivoMuyGrande());

        var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return UnitResult.Failure<DomainError>(StorageError.ExtensionNoPermitida());

        var contentType = file.ContentType?.ToLowerInvariant();
        if (contentType is null || !_allowedContentTypes.Any(ct => contentType.Contains(ct.Split('/')[1])))
            return UnitResult.Failure<DomainError>(StorageError.TipoContenidoNoPermitido());

        var filename = System.IO.Path.GetFileName(file.FileName);
        if (filename.Contains("..") || filename.Contains('/') || filename.Contains('\\'))
            return UnitResult.Failure<DomainError>(StorageError.NombreArchivoInvalido());

        return UnitResult.Success<DomainError>();
    }

    public Task<Result<string, DomainError>> SaveFileAsync(IFormFile file, string folder)
    {
        var validation = ValidateFile(file);
        if (validation.IsFailure)
            return Task.FromResult(Result.Failure<string, DomainError>(validation.Error));

        try
        {
            var filename = GenerateUniqueFilename(file.FileName);
            var folderPath = System.IO.Path.Combine(_rootPath, folder);
            Directory.CreateDirectory(folderPath);

            var filePath = System.IO.Path.Combine(folderPath, filename);
            var relativePath = GetRelativePath(filename, folder);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            _logger.LogInformation("Archivo guardado: {Path}", relativePath);
            return Task.FromResult(Result.Success<string, DomainError>(relativePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error guardando archivo");
            return Task.FromResult(Result.Failure<string, DomainError>(StorageError.ErrorGuardando()));
        }
    }

    public Task<Result<bool, DomainError>> DeleteFileAsync(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return Task.FromResult(Result.Success<bool, DomainError>(true));

        try
        {
            var fullPath = GetFullPath(filename);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Archivo eliminado: {Filename}", filename);
            }
            return Task.FromResult(Result.Success<bool, DomainError>(true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando archivo {Filename}", filename);
            return Task.FromResult(Result.Failure<bool, DomainError>(StorageError.ErrorEliminando()));
        }
    }

    public bool FileExists(string filename)
    {
        if (string.IsNullOrEmpty(filename)) return false;
        return File.Exists(GetFullPath(filename));
    }

    public string GetFullPath(string filename)
    {
        if (System.IO.Path.IsPathRooted(filename)) return filename;
        var prefix = $"/{_uploadPath}/";
        if (filename.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            filename = filename[prefix.Length..];
        return System.IO.Path.Combine(_rootPath, filename);
    }

    public string GetRelativePath(string filename, string folder = "productos")
    {
        return $"/{_uploadPath}/{folder}/{filename}";
    }
}

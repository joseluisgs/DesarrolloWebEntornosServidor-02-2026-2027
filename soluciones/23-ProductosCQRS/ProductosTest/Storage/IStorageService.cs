using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using ProductosCQRS.Errors;

namespace ProductosCQRS.Storage;

public interface IStorageService
{
    Task<Result<string, DomainError>> SaveFileAsync(IFormFile file, string folder);
    Task<Result<bool, DomainError>> DeleteFileAsync(string filename);
    bool FileExists(string filename);
    string GetFullPath(string filename);
    string GetRelativePath(string filename, string folder = "productos");
}

using CommonServices.Error;
using CSharpFunctionalExtensions;

namespace CommonServices.Services.Storage;

public interface IStorageService
{
    Task<Result<string, FunkoError>> SaveFileAsync(IFormFile file, string folder);

    Task<Result<bool, FunkoError>> DeleteFileAsync(string filename);

    bool FileExists(string filename);

    string GetFullPath(string filename);
    
    string GetRelativePath(string filename, string folder = "productos");
}
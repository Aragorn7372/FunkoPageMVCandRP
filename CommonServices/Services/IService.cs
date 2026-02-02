using CSharpFunctionalExtensions;

namespace CommonServices.Services;

public interface IService<T,ID,E>
{
    Task<List<T>> GetAllAsync();
    Task<Result<T,E>> GetByIdAsync(ID id);
}
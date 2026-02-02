using CommonServices.Dto;
using CommonServices.Error;
using CommonServices.Mapper;
using CommonServices.model;
using CommonServices.Repository.Category;
using CSharpFunctionalExtensions;

namespace CommonServices.Services.Categorias;

public class ServiceCategoriaImpl(ILogger<ServiceCategoriaImpl> logger,ICategoriaRepository repository) : IServiceCategoria
{
    private const string CacheKey = "Categoria_";
    public async Task<List<CategoriaResponseDto>> GetAllAsync()
    {
        logger.LogInformation("Getting categorias");
        return await Task.FromResult(repository.GetAllAsync().Result.Select(it => it.ToDto()).ToList());
    }

    public async Task<Result<CategoriaResponseDto, CategoriaError>> GetByIdAsync(string id)
    {
        return await repository.GetByIdAsync(id) is { } categoria
            ? Result.Success<CategoriaResponseDto, CategoriaError>(categoria.ToDto())
                .Tap(_ => logger.LogInformation("getting categoria {id}", id))
            : Result.Failure<CategoriaResponseDto, CategoriaError>(
                new CategoriaNotFoundError(($"no se ha encontrado categoria con nombre: {id}", id).ToString()))
                .TapError(_ => logger.LogWarning("categoria not found with name: {id}", id));
    }

   
}
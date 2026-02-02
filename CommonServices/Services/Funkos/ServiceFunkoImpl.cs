using CommonServices.Dto;
using CommonServices.Error;
using CommonServices.Mapper;
using CommonServices.model;
using CommonServices.Repository.Category;
using CommonServices.Repository.Funkos;
using CommonServices.Services.Storage;
using CSharpFunctionalExtensions;

namespace CommonServices.Services.Funkos;

public class ServiceFunkoImpl(
    IFunkoRepository repository,
    ICategoriaRepository categoryRepository,
    ILogger<ServiceFunkoImpl> logger,
    IStorageService storage
    )
    : IServiceFunko
{
    public async Task<Result<PageResponse<FunkoResponseDto>, FunkoError>> GetAllAsync(FilterDto filter)
    {
        logger.LogDebug(
            "Obteniendo listado de Funkos con filtros - Nombre: {Nombre}, Categoria: {Categoria}, MaxPrecio: {MaxPrecio}",
            filter.Nombre, filter.Categoria, filter.MaxPrecio);

        var (funkos, totalCount) = await repository.GetAllAsync(filter);
        var response = funkos.Select(it => it.ToDto()).ToList();

        var page = new PageResponse<FunkoResponseDto>
        {
            Items = response,
            TotalCount = totalCount,
            Page = filter.Page,
            Size = filter.Size
        };

        logger.LogInformation("Listado de Funkos obtenido, total encontrado: {Total}, página: {Page}", totalCount,
            filter.Page);
        return Result.Success<PageResponse<FunkoResponseDto>, FunkoError>(page);
    }
    private const string CacheKey = "Funko_";
  

    public async Task<List<FunkoResponseDto>> GetAllAsync()
    {
        logger.LogInformation("obtener funkos");
        return await Task.FromResult(repository.GetAllAsync().Result.Select(it => it.ToDto()).ToList());
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> GetByIdAsync(long id)
    {
          logger.LogInformation("obtener funko con id: " + id);
          return await repository.GetByIdAsync(id) is { } repoModel
                ? Result.Success<FunkoResponseDto, FunkoError>(repoModel.ToDto())
                    .Tap(_=> logger.LogInformation("funko obtenido y guardado en la cache con con id: " + repoModel.Id)) 
                : Result.Failure<FunkoResponseDto,FunkoError>(new FunkoNotFoundError("funko no encontrado con id: " + id))
                    .TapError(_=> logger.LogWarning("funko no encontrado con id: " + id));
    }


    public async Task<Result<FunkoResponseDto, FunkoError>> CreateAsync(FunkoRequestDto request, IFormFile? file)
    {
        var validationResult = await Valida(request);
        var image = await SaveImage(file);
        if (image.IsSuccess && image.Value != string.Empty)
        {
            request.Image=image.Value;
        }
        return validationResult.IsSuccess 
            ? image.IsSuccess
                ? await repository.AddAsync(request.ToModel(validationResult.Value)) is { } model
                    ? Result.Success<FunkoResponseDto, FunkoError>(
                        model.ToDto()
                    ).Tap(_=> logger.LogInformation("funko guardado en la base de datos con id:" + model.Id))
                    : Result.Failure<FunkoResponseDto, FunkoError>(
                        new FunkoError("no se pudo guardar el funko")
                    ).TapError(_=>logger.LogError("funko no ha sido guardado en la base de datos"))
                : Result.Failure<FunkoResponseDto, FunkoError>(image.Error)
            : Result.Failure<FunkoResponseDto, FunkoError>(validationResult.Error);
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> DeleteAsync(long id)
    {
        
        return await repository.DeleteAsync(id) is { } model
            ? Result.Success<FunkoResponseDto, FunkoError>(model.ToDto()).Tap(_=>  logger.LogInformation("funko deleto con id:" + id))
            : Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError("no se encontro funko con id " + id))
                .TapError(_=> logger.LogWarning("funko no ha sido encontro funko con id: " + id));
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> UpdateAsync(long id, FunkoRequestDto request,IFormFile? file)
    {   
        var validationResult = await Valida(request);
        var image = await SaveImage(file);
        if (image.IsSuccess && image.Value != string.Empty)
        {
            request.Image=image.Value;
        }
        return validationResult.IsSuccess
            ?image.IsSuccess
                ? await repository.UpdateAsync(id, request.ToModel(validationResult.Value)) is { } updateModel 
                    ? Result.Success<FunkoResponseDto, FunkoError>(updateModel.ToDto())
                        .Tap(_=> logger.LogInformation("funko valido y correctamente actualizado"))
                    : Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError("no se pudo guardar el funko con id:" + id))
                        .TapError(_=> logger.LogWarning("funko no encontrado con id:" + id))
                : Result.Failure<FunkoResponseDto,FunkoError>(image.Error)
                    .TapError(_=> logger.LogWarning("funko image no ha sido guardada"))
            : Result.Failure<FunkoResponseDto,FunkoError>(validationResult.Error)
                .TapError(_=> logger.LogWarning("funko invalido"));
    }

    private async Task<Result<Categoria,FunkoError>> Valida(FunkoRequestDto request)
    {
        return await categoryRepository.GetByIdAsync(request.Categoria) is { } categoria
            ? Result.Success<Categoria,FunkoError>(categoria)
                .Tap(_=> logger.LogInformation("funko valido"))
            : Result.Failure<Categoria,FunkoError>(new FunkoValidationError("funko no valido categoria no existe")
            ).TapError(_=> logger.LogWarning("funko no ha sido valido"));
    }
    private async Task<Result<string, FunkoError>> SaveImage(IFormFile? file)
    {
        try
        {
            return file is not null
                ? await storage.SaveFileAsync(file,"images")
                : Result.Success<string,FunkoError>(string.Empty);
        }
        catch (Exception e)
        {
            return Result.Failure<string, FunkoError>(new FunkoStorageError(e.Message));
        }
    }
}
using CommonServices.Dto;
using CommonServices.Error;
using CSharpFunctionalExtensions;

namespace CommonServices.Services.Funkos;

public interface IServiceFunko : IService<FunkoResponseDto, long,FunkoError>
{
    Task<Result<PageResponse<FunkoResponseDto>, FunkoError>> GetAllAsync(FilterDto filter);
    Task<Result<FunkoResponseDto,FunkoError>> CreateAsync(FunkoRequestDto request, IFormFile? file);
    Task<Result<FunkoResponseDto,FunkoError>> UpdateAsync(long id,FunkoRequestDto request, IFormFile? file);
    Task<Result<FunkoResponseDto,FunkoError>> DeleteAsync(long id);
    
}
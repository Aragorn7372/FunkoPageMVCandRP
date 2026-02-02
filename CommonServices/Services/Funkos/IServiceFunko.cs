using CommonServices.Dto;

namespace CommonServices.Services;

public interface IServiceFunko : IService<FunkoResponseDto, long,FunkoError>
{
    Task<Result<FunkoResponseDto,FunkoError>> CreateAsync(FunkoRequestDto request);
    Task<Result<FunkoResponseDto,FunkoError>> UpdateAsync(long id,FunkoRequestDto request);
    Task<Result<FunkoResponseDto,FunkoError>> DeleteAsync(long id);
    
}
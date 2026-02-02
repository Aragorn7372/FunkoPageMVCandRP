using CommonServices.Dto;
using CommonServices.Error;

namespace CommonServices.Services.Categorias;

public interface IServiceCategoria : IService<CategoriaResponseDto, string, CategoriaError>;

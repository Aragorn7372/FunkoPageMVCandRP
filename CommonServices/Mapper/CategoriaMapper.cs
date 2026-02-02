using CommonServices.Dto;
using CommonServices.model;

namespace CommonServices.Mapper;

public static class CategoriaMapper
{
    public static CategoriaResponseDto ToDto(this Categoria categoria)
    {
        return new CategoriaResponseDto(categoria.Id, categoria.Nombre);
    }
}
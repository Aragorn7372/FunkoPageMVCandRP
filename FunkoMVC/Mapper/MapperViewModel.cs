using CommonServices.Dto;
using FunkoMVC.Model;

namespace FunkoMVC.Mapper;

public static class MapperViewModel
{
    public static FunkoModelViews toVM(this FunkoResponseDto response,IFormFile file)
    {
        return new FunkoModelViews(
            response.Id,
            response.Nombre,
            response.Precio,
            response.Categoria,
            file);
    }
    public static  FunkoRequestDto toDto(this FunkoModelViews request)
    {
        return new FunkoRequestDto
        {
            Categoria = request.Categoria,
            Nombre = request.Nombre,
            Price = request.Precio
        };
    }
}
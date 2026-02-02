using CommonServices.Dto;
using CommonServices.model;

namespace CommonServices.Mapper;

public static class FunkosMapper
{
    public static Funko ToModel(this FunkoRequestDto dto, Categoria categoria )
    {
        return new Funko
        {
            Name = dto.Nombre,
            Category = categoria,
            Price = dto.Price,
            Imagen = dto.Image ?? Funko.IMAGE_DEFAULT
        };


    }

    public static FunkoResponseDto ToDto(this Funko funko)
    {
        return new FunkoResponseDto(
            funko.Id,
            funko.Name, 
            funko.Price, 
            funko.Category!.Nombre,
            funko.Imagen
        );
    }
}
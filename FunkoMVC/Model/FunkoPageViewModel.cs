using CommonServices.Dto;

namespace FunkoMVC.Model;

public class FunkoPageViewModel
{
    
    public string? Nombre { get; set; }
    
    public int PageNumber { get; set; } = 1;
    
    public int TotalPages { get; set; }
    public IEnumerable<FunkoResponseDto> Funkos { get; set; } = new List<FunkoResponseDto>();
    
    // últimos 3 Funkos visitados 
    public List<FunkoResponseDto> VistosRecientemente { get; set; } = new List<FunkoResponseDto>();
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
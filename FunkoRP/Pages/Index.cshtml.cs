using CommonServices.Dto;
using CommonServices.Services.Funkos;
using FunkoRP.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunkoRP.Pages;

public class IndexModel(IServiceFunko service, ILogger<IndexModel> logger) : PageModel
{
    public IEnumerable<FunkoResponseDto> FunkoResponses { get; set; } = [];
    public List<FunkoResponseDto> VistosRecientemente { get; private set; } = [];
    
    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }

    // Añadimos el índice de la página actual (por defecto 0 según tu FilterDto)
    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 0;

    // Añadimos el total de páginas
    public int TotalPages { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        logger.LogInformation("obteniendo funkos");
        
        // Pasamos el PageIndex al filtro. Asumimos un tamaño de página de 10.
        var filter = new FilterDto(Nombre, null, null, PageIndex, 10);
        
        var result = await service.GetAllAsync(filter);
        logger.LogInformation("funkos success: "+result.IsSuccess);
        
        if (result.IsSuccess) 
        {
            FunkoResponses = result.Value.Items;
            
            // Asignamos el total de páginas. 
            // NOTA: Asegúrate de que 'result.Value' tenga esta propiedad. 
            // Si tu DTO usa otro nombre (ej. TotalElements), ajusta esta línea.
            TotalPages = result.Value.TotalPages; 
        }
        else 
        {
            FunkoResponses = [];
            TotalPages = 0;
        }
        
        logger.LogInformation("funkos obtenidos " + FunkoResponses.ToList().Count);
        VistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDto>>("VistosRecientemente") ?? new();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        // Verificar que el usuario es Admin
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó eliminar Funko con id: {Id}", id);
            TempData["ErrorMessage"] = "No tienes permisos para eliminar Funkos";
            return RedirectToPage("/AccessDenied");
        }
        logger.LogInformation("Eliminando funko");
        var result = await service.DeleteAsync(id);

        if (!result.IsSuccess || result.Value == null)
        {
            return NotFound();
        }
        TempData["Eliminado"] = $"{result.Value.Nombre} fue eliminado con éxito.";
        return RedirectToPage();
    }
}
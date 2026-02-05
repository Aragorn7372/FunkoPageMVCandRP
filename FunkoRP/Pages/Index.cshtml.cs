using CommonServices.Dto;
using CommonServices.Services.Funkos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunkoRP.Pages;

public class IndexModel(IServiceFunko service, ILogger<IndexModel> logger) : PageModel
{
    public IEnumerable<FunkoResponseDto> FunkoResponses { get; set; } = [];
    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        logger.LogInformation("obteniendo funkos");
        var filter = new FilterDto(Nombre, null, null);
        var result = await service.GetAllAsync(filter);
        logger.LogInformation("funkos success: "+result.IsSuccess);
        if (result.IsSuccess) FunkoResponses = result.Value.Items;
        else FunkoResponses = [];
        logger.LogInformation("funkos obtenidos " + FunkoResponses.ToList().Count);
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
      
            return RedirectToPage("/Error");
            // return NotFound();
        }
        return RedirectToPage();
    }
}

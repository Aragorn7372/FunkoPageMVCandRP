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
        if (FunkoResponses != null)
            logger.LogInformation("funkos obtenidos " + FunkoResponses.ToList().Count);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
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

/*
 * using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages;

public class IndexModel(IFunkoService service) : PageModel
{
    // Declaramos la propiedad que almacenará la lista de Funkos para mostrarla en la vista
    public IEnumerable<FunkoResponseDTO> Funkos { get; set; } = [];

    // Vinculamos esta propiedad con el input del buscador de la vista
    // Usamos SupportsGet = true para que capture el valor desde la URL (ej: ?Nombre=Batman)
    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }

    // Este método se ejecuta automáticamente cuando cargamos la página
    public async Task OnGetAsync()
    {
        // Creamos el objeto de filtrado usando el valor que hemos recibido en la propiedad Nombre
        // Establecemos valores por defecto para paginación (página 1, tamaño 10) y ordenación
        var filter = new FilterDTO(Nombre, null, null, 1, 10, "id", "asc");

        // Llamamos al servicio pasándole el filtro por nombre que acabamos de crear
        var result = await service.GetAllAsync(filter);

        // Comprobamos si el resultado del servicio fue exitoso
        if (result.IsSuccess)
        {
            // Si fue bien, extraemos la lista de items del objeto PageResponse y la guardamos
            Funkos = result.Value.Items;
        }
        else
        {
            // Si hubo algún error o no hay datos, inicializamos la lista como vacía para evitar nulos
            Funkos = [];
        }
    }
}
 */

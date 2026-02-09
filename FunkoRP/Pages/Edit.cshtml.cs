using CommonServices.Dto;
using CommonServices.Services.Categorias;
using CommonServices.Services.Funkos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunkoRP.Pages;

public class Edit(IServiceFunko service,IServiceCategoria serviceCategoria, ILogger<Edit> logger) : PageModel
{


    [BindProperty]
    public FunkoRequestDto Funko { get; set; } = new();

    [BindProperty]
    public IFormFile? File { get; set; }

    public string? ImagenActual { get; set; }

    public bool EsEdicion => Id.HasValue;

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }
    public IEnumerable<CategoriaResponseDto> Categorias { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó crear o actualizar Funko");
            TempData["ErrorMessage"] = "No tienes permisos para eliminar crear o actualizar";
            return RedirectToPage("/AccessDenied");
        }
        logger.LogInformation("obteniendo categorias");
        Categorias = await serviceCategoria.GetAllAsync();
        logger.LogInformation("es crear o editar");
        if (!Id.HasValue)
            return Page(); 
        logger.LogInformation("es editar, obteniendo funko");
        var result = await service.GetByIdAsync(Id.Value);

        if (!result.IsSuccess || result.Value == null)
            return NotFound();
        logger.LogInformation("obtenido sin problema, bindeando");
        Funko = new FunkoRequestDto
        {
            Nombre = result.Value.Nombre,
            Price = result.Value.Precio,
            Categoria = result.Value.Categoria
        };

        ImagenActual = result.Value.Imagen;
        
        return Page();
    }

    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó eliminar Funko");
            TempData["ErrorMessage"] = "No tienes permisos para eliminar Funkos";
            return RedirectToPage("/AccessDenied");
        }
        logger.LogInformation(ModelState.ToString());
        if (!ModelState.IsValid)
            return Page();

        if (EsEdicion)
        {
            var result=await service.UpdateAsync(Id!.Value, Funko, File);
            if (result.IsSuccess)
            {
                TempData["Actualizado"] = $"{Funko.Nombre} actualizado con éxito, ya es parte de la colección.";
            }
            else
            {
                ModelState.AddModelError("ImageFile", $"Error guardando el Funko: {result.Error.Error}");
                return Page();
            }
            
        }
        else
        {
            var result=await service.CreateAsync(Funko, File);
            if (result.IsSuccess)
            {
                TempData["Creado"] = $"{Funko.Nombre} creado con éxito, ya es parte de la colección.";
            }
            else
            {
                ModelState.AddModelError("ImageFile", $"Error creando el Funko: {result.Error.Error}");
                return Page();
            }
        }
        
        return RedirectToPage("Index");
    }
}
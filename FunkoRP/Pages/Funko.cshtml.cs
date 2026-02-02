using CommonServices.Dto;
using CommonServices.Services.Categorias;
using CommonServices.Services.Funkos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunkoRP.Pages;


public class Funko(IServiceFunko service) : PageModel
{
    public FunkoResponseDto FunkoResponses { get; set; }

   
    public async Task<IActionResult> OnGet(
        long id)
    {
        var result = await service.GetByIdAsync(id);

        if (!result.IsSuccess || result.Value == null)
        {
      
            return RedirectToPage("/Error");
            // return NotFound();
        }

        FunkoResponses = result.Value;
    
        return Page();
    }
}
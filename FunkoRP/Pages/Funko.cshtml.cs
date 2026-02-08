using CommonServices.Dto;
using CommonServices.Services.Categorias;
using CommonServices.Services.Funkos;
using FunkoRP.Session;
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
      
            return NotFound();
            
            // return NotFound();
        }

        FunkoResponses = result.Value;
        AddFunkoToSession(FunkoResponses);
        return Page();
    }
    private void AddFunkoToSession(FunkoResponseDto funko)
    {
     
        //Recuperamos la lista actual de la sesión (o creamos una vacía si es null)
        var vistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDto>>("VistosRecientemente") ?? new();

        //Evitamos duplicados: Si el funko ya estaba en la lista, lo quitamos de su posición anterior
        vistosRecientemente.RemoveAll(f => f.Id == FunkoResponses.Id);

        //Insertamos al principio de la lista (el más reciente)
        vistosRecientemente.Insert(0, FunkoResponses);

        // 4. Limitamos a 3 elementos (Cola FIFO)
        if (vistosRecientemente.Count > 3)
        {
            vistosRecientemente.RemoveAt(3); // Borramos el 4º elemento (el más antiguo)
        }

        // 5. Guardamos la lista actualizada en la sesión
        HttpContext.Session.SetJson("VistosRecientemente", vistosRecientemente);
    }

}
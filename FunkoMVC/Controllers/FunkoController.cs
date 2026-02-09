using System.Diagnostics;
using CommonServices.Dto;
using CommonServices.Model;
using CommonServices.Services.Funkos;
using FunkoMVC.Model;
using FunkoMVC.Session;
using Microsoft.AspNetCore.Mvc;

namespace FunkoMVC.Controllers;

public class FunkosController (IServiceFunko service,ILogger<FunkosController> logger) : Controller
{
    
[HttpGet]
public async Task<IActionResult> Index(string? nombre, int pageNumber = 0)
{
    var filter = new FilterDto(nombre, null, null, pageNumber);
    var result = await service.GetAllAsync(filter);
    var viewModel = new FunkoPageViewModel();
    if (result.IsSuccess)
    {
        viewModel.Funkos = result.Value.Items; 
        viewModel.Nombre = nombre; 
        viewModel.PageNumber = pageNumber; 
        viewModel.TotalPages = (int)Math.Ceiling((double)result.Value.TotalCount / 10);
        viewModel.VistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDto>>("VistosRecientemente") 
                                         ?? new List<FunkoResponseDto>();
    }
    return View(viewModel);
}
    
    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("User"))
        {
            logger.LogWarning("Usuario sin permisos intentó crear Funko");
            TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
            return RedirectToAction("AccessDenied", "Error"); 
        }
        var result = await service.GetByIdAsync(id);

        if (result.IsSuccess)
        {
            var funko = result.Value;
            AddFunkoToSession(funko);

            var viewModel = new FunkoModelViews{
                Id = funko.Id,
                Nombre = funko.Nombre,
                Categoria = funko.Categoria,
                Precio = funko.Precio,
                Imagen = funko.Imagen
            };
            return View(viewModel);
        }
    
        return NotFound();
    }
    private void AddFunkoToSession(FunkoResponseDto funko)
    {
        var vistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDto>>("VistosRecientemente") 
                                  ?? new List<FunkoResponseDto>();
        vistosRecientemente.RemoveAll(f => f.Id == funko.Id);

        vistosRecientemente.Insert(0, funko);
        if (vistosRecientemente.Count > 3)
        {
            vistosRecientemente.RemoveAt(3); // Borramos el 4º elemento
        }
        HttpContext.Session.SetJson("VistosRecientemente", vistosRecientemente);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        // Verificar que el usuario es Admin
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó crear Funko");
            TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
            return RedirectToAction("AccessDenied", "Error"); 
        }
        return View(); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Create(FunkoRequestDto funko, IFormFile? imagen)
    {
        // Verificar que el usuario es Admin
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó crear Funko");
            TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
            return RedirectToAction("AccessDenied", "Error"); 
        }
        if (!ModelState.IsValid)
        {
            return View(funko); 
        }
        var result = await service.CreateAsync(funko,imagen);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Error);
            return View(funko);
        }
        TempData["Creado"] = $"{result.Value.Nombre} fue creado con éxito.";
        return RedirectToAction(nameof(Index));
    }
[HttpGet]
public async Task<IActionResult> Update(long id)
{
    // Verificar que el usuario es Admin
    if (!User.IsInRole("Admin"))
    {
        logger.LogWarning("Usuario sin permisos intentó actualizar funko Funko {}",id);
        TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
        return RedirectToAction("AccessDenied", "Error"); 
    }
    var result = await service.GetByIdAsync(id);
    if (result.IsFailure)
    {
        return NotFound();
    }
    var funko = result.Value;
    var viewModel = new FunkoPostViewModel()
    {
        Id = funko.Id,
        Form = new FunkoRequestDto
        {
            Nombre = funko.Nombre,
            Categoria = funko.Categoria,
            Price = funko.Precio,
            Image = funko.Imagen 
        }
    };
    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken] // Protección CSRF
public async Task<IActionResult> Update(long id, FunkoPostViewModel viewModel)
{
    // Verificar que el usuario es Admin
    if (!User.IsInRole("Admin"))
    {
        logger.LogWarning("Usuario sin permisos intentó actualizar Funko {}",id);
        TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
        return RedirectToAction("AccessDenied", "Error"); 
    }
    if (!ModelState.IsValid)
    {
        return View(viewModel);
    }
    var result = await service.UpdateAsync(id, viewModel.Form,viewModel.ImageFile);
    if (result.IsFailure)
    {
        ModelState.AddModelError(string.Empty, result.Error.Error);
        return View(viewModel);
    }
    TempData["Actualizado"] = $"{viewModel.Form.Nombre} fue actualizado con éxito.";
    return RedirectToAction(nameof(Index));
}

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Delete(long id)
    {
        // Verificar que el usuario es Admin
        if (!User.IsInRole("Admin"))
        {
            logger.LogWarning("Usuario sin permisos intentó crear Funko");
            TempData["ErrorMessage"] = "No tienes permisos para crear Funkos";
            return RedirectToPage("AccessDenied", "Auth");
        }
        var result = await service.DeleteAsync(id);
        if (result.IsSuccess)
        {
            TempData["Eliminado"] = $"{result.Value.Nombre} fue eliminado con éxito.";
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
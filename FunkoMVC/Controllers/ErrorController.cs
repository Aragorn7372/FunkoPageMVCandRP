using Microsoft.AspNetCore.Mvc;

namespace FunkoMVC.Controllers;

public class ErrorController : Controller
{
    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewData["Title"] = "Acceso Denegado";
        return View(); 
    }
}
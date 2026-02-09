using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunkoMVC.Pages;

public class NotFound : PageModel
{
        [BindProperty(SupportsGet = true)]
        public string? Message { get; set; }

        public string Mensaje =>
            string.IsNullOrEmpty(Message)
                ? "La página solicitada no existe"
                : Message;

}
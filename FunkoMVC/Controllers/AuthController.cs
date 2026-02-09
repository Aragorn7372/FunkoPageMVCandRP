using System.ComponentModel.DataAnnotations;
using CommonServices.model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FunkoMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(SignInManager<User> signInManager, ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es obligatorio")]
            [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Limpia cookies antiguas de TempData si existen
            if (TempData.ContainsKey("InicioSesionFallido"))
                TempData.Remove("InicioSesionFallido");

            return View(new InputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(InputModel input, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return View(input);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    input.Email,
                    input.Password,
                    input.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} ha iniciado sesión correctamente.", input.Email);
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("La cuenta del usuario {Email} está bloqueada.", input.Email);
                    ModelState.AddModelError(string.Empty, "La cuenta está bloqueada.");
                    return View(input);
                }

                // Credenciales incorrectas
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                _logger.LogWarning("Intento de inicio de sesión fallido para {Email}.", input.Email);
                return View(input);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión para {Email}.", input.Email);
                ModelState.AddModelError(string.Empty, "Error interno, vuelve a intentarlo.");
                return View(input);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

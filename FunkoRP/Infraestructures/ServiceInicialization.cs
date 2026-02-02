using CommonServices.Services.Categorias;
using CommonServices.Services.Funkos;
using Serilog;

namespace FunkoRP.Infraestructures;

/// <summary>
/// Extensiones de configuración de servicios de negocio.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra todos los servicios de negocio en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Log.Information("⚙️ Registrando servicios...");
        return services
            .AddScoped<IServiceFunko, ServiceFunkoImpl>()
            .AddScoped<IServiceCategoria, ServiceCategoriaImpl>();
    }
}
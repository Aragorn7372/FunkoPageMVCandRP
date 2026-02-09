using CommonServices.Database;
using Microsoft.EntityFrameworkCore;

namespace FunkoMVC.Infraestructures;

public static class DatabaseConfig
{
    
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<FunkoDbContext>(options =>
            options.UseInMemoryDatabase("FunkoDatabase"));

        return services;
    }
}


using System.Linq.Expressions;
using CommonServices.Database;
using CommonServices.Dto;
using CommonServices.model;
using Microsoft.EntityFrameworkCore;

namespace CommonServices.Repository.Funkos;

public class FunkoRepository(FunkoDbContext context,ILogger<FunkoRepository> log) : IFunkoRepository
{
   
    public async Task<(IEnumerable<Funko> Items, int TotalCount)> GetAllAsync(FilterDto filter)
    {
        log.LogDebug("Consultando Funkos con filtros - Nombre: {Nombre}, Categoria: {Categoria}, MaxPrecio: {MaxPrecio}, Page: {Page}",
            filter.Nombre, filter.Categoria, filter.MaxPrecio, filter.Page);
        
        var query = context.Funkos.Include(f => f.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Nombre))
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{filter.Nombre}%"));

        if (!string.IsNullOrWhiteSpace(filter.Categoria))
            query = query.Where(p => EF.Functions.Like(p.Category!.Nombre, $"%{filter.Categoria}%"));

        if (filter.MaxPrecio.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrecio.Value);


        var totalCount = await query.CountAsync();
        query = ApplySorting(query, filter.SortBy, filter.Direction);

        var items = await query
            .Skip(filter.Page * filter.Size)
            .Take(filter.Size)
            .ToListAsync();

        log.LogDebug("Consulta de Funkos completada, encontrados: {Total}", totalCount);
        return (items, totalCount);
    }

    public async Task<List<Funko>> GetAllAsync()
    {
        log.LogInformation("Getting all Funkos");
        return await  context.Funkos
            .Include(f => f.Category)
            .ToListAsync();
    }

    public async Task<Funko?> GetByIdAsync(long id)
    {
        log.LogInformation("Getting Funko with id: " + id);
        return await context.Funkos
            .Include(f => f.Category)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
    
    public async Task<Funko?> UpdateAsync(long id, Funko newFunko)
    {
        log.LogInformation("Updating Funko with id: " + id);
        newFunko.Id = id;
        var found=await context.Funkos.FindAsync(id);
        if (found != null)
        {
            found.Name = newFunko.Name;
            found.Category = newFunko.Category;
            found.Price= newFunko.Price;
            found.UpdatedAt= DateTime.UtcNow;
            if (newFunko.Imagen != Funko.IMAGE_DEFAULT)
            {
                found.Imagen = newFunko.Imagen;
            }
            var updated =  context.Funkos.Update(found);
            await context.SaveChangesAsync();
            await context.Funkos.Entry(found).Reference(f => f.Category).LoadAsync();
            return updated.Entity;
        }
        return null;
    }

    public async Task<Funko> AddAsync(Funko newFunko)
    { 
        log.LogInformation("Adding Funko");
        var saved=await context.Funkos.AddAsync(newFunko);
        await context.SaveChangesAsync();
        await context.Funkos.Entry(newFunko).Reference(f => f.Category).LoadAsync();
        return saved.Entity;
    }

    public async Task<Funko?> DeleteAsync(long id)
    {
        log.LogInformation("Deleting Funko with id: " + id);
        var deleted=await context. Funkos
            .Include(f => f.Category)
            .FirstOrDefaultAsync(f => f.Id.Equals(id)) is { } funko
            ? context.Funkos.Remove(funko).Entity
            : null;
        await context.SaveChangesAsync();
        return deleted;
    }
    private static IQueryable<Funko> ApplySorting(IQueryable<Funko> query, string sortBy, string direction)
    {
        var isDescending = direction.Equals("desc", StringComparison.OrdinalIgnoreCase);
        Expression<Func<Funko, object>> keySelector = sortBy.ToLower() switch
        {
            "nombre" => p => p.Name,
            "precio" => p => p.Price,
            "createdat" => p => p.CreatedAt,
            "categoria" => p => p.Category!.Nombre,
            _ => p => p.Id
        };
        return isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }
   
}
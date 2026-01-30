using CommonServices.Database;
using CommonServices.model;
using Microsoft.EntityFrameworkCore;

namespace CommonServices.Repository.Category;

public class CategoryRepository(FunkoDbContext context,ILogger<CategoryRepository> log) : ICategoriaRepository
{

    public async Task<List<Categoria>> GetAllAsync()
    {
        log.LogInformation("Getting all categorias");
        return await context.Categorias.ToListAsync();
    }
    public async Task<Categoria?> GetByIdAsync(string id)
    {
        log.LogInformation("gettin categorie with id {id}", id);
        return await context.Categorias.Where(c=> c.Nombre == id).FirstOrDefaultAsync();
    }
}


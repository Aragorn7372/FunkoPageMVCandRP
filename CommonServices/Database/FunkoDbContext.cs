

using CommonServices.model;
using Microsoft.EntityFrameworkCore;

namespace CommonServices.Database;
/// <summary>
/// clase de configuracion de la base de datos donde se indican las tablas y columnas
/// </summary>
public class FunkoDbContext : DbContext
{
    /// <summary>
    /// genera tablas y columnas
    /// </summary>
    /// <param name="modelBuilder">generador de net para tablas y columnas</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedData(modelBuilder); 
  
    }
    
    
    public DbSet<Funko> Funkos { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;

    
  /// <summary>
  /// contructor con parametros
  /// </summary>
  /// <param name="options">opcionas para configurar la clase</param>
    public FunkoDbContext(DbContextOptions<FunkoDbContext> options)
        : base(options)
    { }
   
        
/// <summary>
/// inicializador de datos de ejemplo de la base de datos
/// </summary>
/// <param name="modelBuilder">controlador de la base de datos</param>
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Define category IDs
        var categoriaAnimeId = Guid.NewGuid();
        var categoriaPeliculasId = Guid.NewGuid();
        var categoriaVideojuegosId = Guid.NewGuid();

        // Seed categories first
        var categoria1 = new Categoria
        {
            Id = categoriaAnimeId,
            Nombre = "Anime"
        };

        var categoria2 = new Categoria
        {
            Id = categoriaPeliculasId,
            Nombre = "Películas"
        };

        var categoria3 = new Categoria
        {
            Id = categoriaVideojuegosId,
            Nombre = "Videojuegos"
        };

        modelBuilder.Entity<Categoria>().HasData(categoria1, categoria2, categoria3);

        // Seed funkos using CategoryId (foreign key), not Category navigation property
        var funko1 = new Funko
        {
            Id = 1,
            Name = "Goku Super Saiyan",
            CategoryId = categoriaAnimeId,  
            Imagen = "uploads/default.png",
            Price = 19.99,
            CreatedAt = DateTime.UtcNow,  
            UpdatedAt = DateTime.UtcNow 
        };

        var funko2 = new Funko
        {
            Id = 2,
            Name = "Darth Vader",
            CategoryId = categoriaPeliculasId, 
            Imagen = "uploads/default.png",
            Price = 24.50,
            CreatedAt = DateTime.UtcNow,  
            UpdatedAt = DateTime.UtcNow 
        };

        var funko3 = new Funko
        {
            Id = 3,
            Name = "Mario Bros",
            CategoryId = categoriaVideojuegosId,  
            Imagen = "uploads/"+Funko.IMAGE_DEFAULT,
            Price = 17.75,
            CreatedAt = DateTime.UtcNow,  
            UpdatedAt = DateTime.UtcNow 
        };

        modelBuilder.Entity<Funko>().HasData(funko1, funko2, funko3);
    }
    
}
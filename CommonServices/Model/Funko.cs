using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonServices.model;

public record Funko
{
    public const string IMAGE_DEFAULT = "default.png";
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption. Identity)]
    public long Id { get; set; }
    
    [Column]
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    // Foreign key property
    [Column]
    [Required]
    public Guid CategoryId { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(CategoryId))]
    public Categoria? Category { get; set; }
    
    [Column]
    [Required]
    public string Imagen { get; set; } = IMAGE_DEFAULT;
    
    [Column]
    [Required]
    [Range(0, int.MaxValue)]
    public double Price { get; set; }
    
    [Column]
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column]
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
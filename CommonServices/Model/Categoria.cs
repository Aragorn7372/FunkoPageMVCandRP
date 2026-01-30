using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonServices.model;


public record Categoria
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Column]
    [Required]
    public string Nombre { get; set; }= string.Empty;
}
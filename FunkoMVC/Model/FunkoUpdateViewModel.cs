using System.ComponentModel.DataAnnotations;
using CommonServices.Dto;

namespace FunkoMVC.Model;

public class FunkoPostViewModel
{
    [Required]
    public long Id { get; set; }
    public FunkoRequestDto Form { get; set; } = new();
    public IFormFile? ImageFile { get; set; }
}
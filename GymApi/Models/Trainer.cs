
using System.ComponentModel.DataAnnotations;

public class Trainer
    {
    public Guid ID { get; set; }
    [Required, StringLength(100)]
    public string FullName { get; set; }= string.Empty;
    [Required, StringLength(10)]
    public string Specialty {  get; set; }= string.Empty;
    [Required]
    public bool Certified { get; set; }=false;

    }

public record CreateTrainerDto
{
    [Required, StringLength(100)]
    public string FullName { get; init; } = string.Empty;
    [Required, StringLength(10)]
    public string Specialty { get; init; } = string.Empty;
}

public record UpdateTrainerDto
{
    [Required, StringLength(100)]
    public string FullName { get; init; } = string.Empty;
    [Required, StringLength(10)]
    public string Specialty { get; init; } = string.Empty;
}

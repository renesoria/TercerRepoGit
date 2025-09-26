
using System.ComponentModel.DataAnnotations;

public class Trainer
    {
    public Guid ID { get; set; }
    [Required, StringLength(100)]
    public string FullName { get; set; }= string.Empty;
    [Required, StringLength(10)]
    public string Speciality {  get; set; }= string.Empty;
    [Required]
    public bool Certified { get; set; }

    }


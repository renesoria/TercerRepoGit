
using System;
using System.ComponentModel.DataAnnotations;
public class User
{
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    required public string Email { get; set; }

    [Required, StringLength(50)]
    required public string Password { get; set; }

    [Range(0, 100)]
    public int Age { get; set; }
}
// DTOs (entrada/salida para la API)
public record CreateUserDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    required public string Email { get; set; }

    [Required, StringLength(50)]
    required public string Password { get; set; }

    [Range(0, 100)]
    public int Age { get; set; }
}
public record UpdateUserDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    required public string Email { get; set; }

    [Required, StringLength(50)]
    required public string Password { get; set; }

    [Range(0, 100)]
    public int Age { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime SubscriptionDate { get; set; } = DateTime.UtcNow;

    [Range(1, 36, ErrorMessage = "Duration must be between 1 and 36 months.")]
    public int Duration { get; set; } // months

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
}

// DTOs
public record CreateSubscriptionDto
{
    [Required] public DateTime SubscriptionDate { get; init; } = DateTime.UtcNow;
    [Range(1, 36)] public int Duration { get; init; }
    [Required, StringLength(100)] public string Name { get; init; } = string.Empty;
}

public record UpdateSubscriptionDto
{
    [Required] public DateTime SubscriptionDate { get; init; }
    [Range(1, 36)] public int Duration { get; init; }
    [Required, StringLength(100)] public string Name { get; init; } = string.Empty;
}

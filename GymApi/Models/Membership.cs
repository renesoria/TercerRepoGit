using System.ComponentModel.DataAnnotations;

namespace FirstExam.Models
{
    public class Memberships
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string Plan { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = default!;
    }

    public class CreateMembershipsDto
    {
        [Required] public Guid MemberId { get; set; }
        [Required, RegularExpression("^(basic|pro|premium)$")] public string Plan { get; set; } = "basic";
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [RegularExpression("^(active|expired|canceled)$")] public string? Status { get; set; } = "active";
    }

    public class UpdateMembershipsDto
    {
        [Required] public Guid MemberId { get; set; }
        [Required, RegularExpression("^(basic|pro|premium)$")] public string Plan { get; set; } = "basic";
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [Required, RegularExpression("^(active|expired|canceled)$")] public string Status { get; set; } = "active";
    }
}

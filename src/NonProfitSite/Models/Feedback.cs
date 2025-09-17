using System;
using System.ComponentModel.DataAnnotations;

namespace NonProfitSite.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [Required]
        public string Message { get; set; } = null!;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false;
        public bool IsReplied { get; set; } = false;
    }
}

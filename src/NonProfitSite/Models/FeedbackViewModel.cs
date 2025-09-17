using System.ComponentModel.DataAnnotations;

namespace NonProfitSite.Models
{
    public class FeedbackViewModel
    {
        [MaxLength(200)]
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = null!;
    }
}

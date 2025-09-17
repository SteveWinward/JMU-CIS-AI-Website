using System.ComponentModel.DataAnnotations;

namespace NonProfitSite.Models
{
    public class AdminUser
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = null!;
        [MaxLength(200)]
        public string? Name { get; set; }
        [Required]
        public string PasswordHash { get; set; } = null!;
    }
}

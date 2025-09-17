using System;
using System.ComponentModel.DataAnnotations;

namespace NonProfitSite.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}

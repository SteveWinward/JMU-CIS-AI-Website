using System;
using System.ComponentModel.DataAnnotations;

namespace NonProfitSite.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}

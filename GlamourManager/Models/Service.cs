using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public required string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(5, 480)]
        public int DurationMinutes { get; set; }

        public virtual List<Stylist> Stylists { get; set; } = new();
        public virtual List<Appointment> Appointments { get; set; } = new();
    }
}
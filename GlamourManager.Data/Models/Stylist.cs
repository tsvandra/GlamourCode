using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Data.Models
{
    public class Stylist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public virtual List<Appointment> Appointments { get; set; } = new();
        public virtual List<Service> Services { get; set; } = new();
    }
}
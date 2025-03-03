using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Models
{
    public class Stylist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Specialization { get; set; }

        public virtual List<Appointment> Appointments { get; set; } = new();
        public virtual List<Service> Services { get; set; } = new();
    }
}
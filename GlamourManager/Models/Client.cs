using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        public virtual List<Appointment> Appointments { get; set; } = new();
    }
}
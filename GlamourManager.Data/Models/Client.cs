using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public virtual List<Appointment> Appointments { get; set; } = new();
    }
}
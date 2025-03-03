using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlamourManager.Data.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime DateTime { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int StylistId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        public int StatusId { get; set; } = AppointmentStatus.Pending;

        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;
        
        [ForeignKey("StylistId")]
        public Stylist Stylist { get; set; } = null!;
        
        [ForeignKey("ServiceId")]
        public Service Service { get; set; } = null!;
        
        [ForeignKey("StatusId")]
        public AppointmentStatus Status { get; set; } = null!;
    }
}
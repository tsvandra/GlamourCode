using System.ComponentModel.DataAnnotations;

namespace GlamourManager.Data.Models
{
    public class AppointmentStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public virtual List<Appointment> Appointments { get; set; } = new();

        // Status constants
        public static int Pending => 1;
        public static int Accepted => 2;
        public static int Refused => 3;
        public static int Cancelled => 4;
        public static int Done => 5;
    }
}
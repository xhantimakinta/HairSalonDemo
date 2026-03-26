using System.ComponentModel.DataAnnotations;

namespace HairSalonDemo.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Staff Member")]
        public int StaffId { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; } = new TimeSpan(9, 0, 0);

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Booked";

        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }
        public Service? Service { get; set; }
    }
}

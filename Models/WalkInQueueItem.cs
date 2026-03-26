using System.ComponentModel.DataAnnotations;

namespace HairSalonDemo.Models
{
    public class WalkInQueueItem
    {
        public int WalkInQueueItemId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Staff Member")]
        public int? StaffId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Waiting";

        [Display(Name = "Added At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Staff? Staff { get; set; }
    }
}

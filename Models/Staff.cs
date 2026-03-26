using System.ComponentModel.DataAnnotations;

namespace HairSalonDemo.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Commission Rate (%)")]
        [Range(0, 100)]
        public decimal CommissionRate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalonDemo.Models
{
    public class Service
    {
        public int ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Service Name")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price")]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Display(Name = "Duration (Minutes)")]
        [Range(1, 600)]
        public int DurationMinutes { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Category")]
        public int ServiceCategoryId { get; set; }

        public ServiceCategory? ServiceCategory { get; set; }
    }
}
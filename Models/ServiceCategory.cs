using System.ComponentModel.DataAnnotations;

namespace HairSalonDemo.Models
{
    public class ServiceCategory
    {
        public int ServiceCategoryId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Description { get; set; }

        public ICollection<Service>? Services { get; set; }
    }
}

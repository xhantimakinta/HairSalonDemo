using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HairSalonDemo.Models
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; } = string.Empty; // Service or Product

        [Required]
        [Display(Name = "Item Id")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Line Total")]
        public decimal LineTotal { get; set; }

        public Sale? Sale { get; set; }
    }
}
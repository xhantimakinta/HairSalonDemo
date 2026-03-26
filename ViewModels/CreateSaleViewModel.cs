using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HairSalonDemo.ViewModels
{
    public class CreateSaleViewModel
    {
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [Required]
        [Display(Name = "Staff Member")]
        public int StaffId { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }

        [Display(Name = "Product Quantity")]
        [Range(1, 100)]
        public int ProductQuantity { get; set; } = 1;

        [Required]
        [StringLength(20)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Discount")]
        [Range(0, 100000)]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal TotalAmount { get; set; }

        public List<SelectListItem>? Customers { get; set; }
        public List<SelectListItem>? StaffMembers { get; set; }
        public List<SelectListItem>? Services { get; set; }
        public List<SelectListItem>? Products { get; set; }
    }
}

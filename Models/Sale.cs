using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.ServerSentEvents;

namespace HairSalonDemo.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        [Required]
        [Display(Name = "Staff Member")]
        public int StaffId { get; set; }

        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        [Range(0, 100000)]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Discount")]
        [Range(0, 100000)]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        [Range(0, 100000)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public Customer? Customer { get; set; }
        public Staff? Staff { get; set; }

        public ICollection<SaleItem>? SaleItems { get; set; }
    }
}
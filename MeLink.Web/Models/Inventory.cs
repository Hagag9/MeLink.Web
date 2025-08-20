
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MeLink.Web.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; }

        [Required]
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
       
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }
    }
}

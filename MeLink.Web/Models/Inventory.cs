
using System.ComponentModel.DataAnnotations;


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

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

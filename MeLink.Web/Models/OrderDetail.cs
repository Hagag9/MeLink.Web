
using System.ComponentModel.DataAnnotations;

namespace MeLink.Web.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

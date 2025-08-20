using System.ComponentModel.DataAnnotations;


namespace MeLink.Web.Models
{
    public enum OrderStatus { Pending = 1, Approved = 2, Rejected = 3, Shipped = 4, Delivered = 5, Cancelled = 6 }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string FromUserId { get; set; } = default!;
        public ApplicationUser? FromUser { get; set; }

        [Required]
        public string ToUserId { get; set; } = default!;
        public ApplicationUser? ToUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // اختياري: لتصنيف النوع (عميل→صيدلية، صيدلية→شركة... لو حبيت)
        public string? OrderType { get; set; }
        public ICollection<OrderDetail> Items { get; set; } = new List<OrderDetail>();
    }
}

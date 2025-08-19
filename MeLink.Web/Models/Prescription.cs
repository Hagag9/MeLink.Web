
using System.ComponentModel.DataAnnotations;

namespace MeLink.Web.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        [Required, MaxLength(300)]
        public string ImagePath { get; set; } = default!; // هنخزن المسار

        [Required]
        public string UploadedByUserId { get; set; } = default!;
        public ApplicationUser? UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

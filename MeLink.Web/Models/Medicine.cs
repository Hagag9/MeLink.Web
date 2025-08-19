using System.ComponentModel.DataAnnotations;

namespace MeLink.Web.Models
{
    public class Medicine
    {

        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string GenericName { get; set; } = default!;

        [MaxLength(200)]
        public string? BrandName { get; set; }

        [MaxLength(100)]
        public string? DosageForm { get; set; }  // Tablet, Syrup...

        [MaxLength(100)]
        public string? Strength { get; set; }    // 500mg, 5mg/5ml

        [MaxLength(200)]
        public string? ManufacturerName { get; set; }

        [MaxLength(200)]
        public string? ActiveIngredient { get; set; }
    }
}

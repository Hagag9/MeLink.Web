namespace MeLink.Web.Models
{
    public class Pharmacy : ApplicationUser
    {
        public string? LicenseNumber { get; set; }
        public string? WorkingHours { get; set; }

        public bool IsDeliveryAvailable { get; set; } = false;
    }
}

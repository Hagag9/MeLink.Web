namespace MeLink.Web.Models
{
    public class Manufacturer : ApplicationUser
    {
        public string? FactoryRegistrationNo { get; set; }
        public string? ProductionCapacity { get; set; }
        public string? ContactPersonName { get; set; }
        public int? MaxPaymentPeriodInDays { get; set; }
        
    }
}

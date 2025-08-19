namespace MeLink.Web.Models
{
    public class DistributionCompany : ApplicationUser
    {
        public string? CompanyRegistrationNo { get; set; }
        public string? ContactPersonName { get; set; }

        public int? MaxPaymentPeriodInDays { get; set; }
    }
}

using System.Collections.Generic;

namespace MeLink.Web.ViewModels
{
    // Represents the indebtedness towards a single company
    public class IndebtednessSummaryViewModel
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // Represents the overall view model for the Indebtedness page
    public class IndebtednessViewModel
    {
        public List<IndebtednessSummaryViewModel> IndebtednessSummaries { get; set; }
        public decimal TotalIndebtedness { get; set; }
        public decimal TotalPaid { get; set; } // For future use
        public int CompaniesCount { get; set; }

        public IndebtednessViewModel()
        {
            IndebtednessSummaries = new List<IndebtednessSummaryViewModel>();
        }
    }
}

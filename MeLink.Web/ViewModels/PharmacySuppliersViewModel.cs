namespace MeLink.Web.ViewModels
{
    // في مجلد ViewModels
    public class UserSuppliersViewModel
    {
        public string PageTitle { get; set; } = "Suppliers";
        public List<SupplierViewModel> Suppliers { get; set; } = new List<SupplierViewModel>();
    }

    // هذا الموديل يمكن إعادة استخدامه كما هو
    public class SupplierViewModel
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string UserType { get; set; } = default!;
    }

    // موديل جديد لعرض أنواع الموردين
    public class SupplierTypeViewModel
    {
        public string Type { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public string Color { get; set; } = default!;
    }


    public class NearbyPharmaciesViewModel
    {
        public List<PharmacyViewModel> Pharmacies { get; set; } = new List<PharmacyViewModel>();
    }

    public class PharmacyViewModel
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public double? DistanceInKm { get; set; } // New property to store the calculated distance
    }
}

namespace MeLink.Web.ViewModels
{
    public class PharmacySuppliersViewModel
    {
        // قائمة الموردين المرتبطين بالصيدلية
        public List<SupplierViewModel> Suppliers { get; set; } = new List<SupplierViewModel>();

    }

    public class SupplierViewModel
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? Address { get; set; }
        public string? City { get; set; }
        // خاصية لتحديد نوع المورد (شركة توزيع، مخزن، مصنع)
        public string UserType { get; set; } = default!;
    }
}

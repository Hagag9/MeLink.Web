
using System.ComponentModel.DataAnnotations;

namespace MeLink.Web.ViewModels
{
    // للبحث في الأدوية
    public class OrderSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public List<MedicineInventoryViewModel> AvailableMedicines { get; set; } = new();


        // خاصية جديدة لتحديد الـ SupplierId
        public string? SupplierId { get; set; }
    }

    // عرض الدواء مع بيانات الصيدلية
    public class MedicineInventoryViewModel
    {
        public int MedicineId { get; set; }
        public string GenericName { get; set; } = default!;
        public string? BrandName { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }

        public string PharmacyId { get; set; } = default!;
        public string PharmacyName { get; set; } = default!;
        public string? PharmacyAddress { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int InventoryId { get; set; }
        public double DistanceInKm { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

    // إنشاء طلب جديد
    public class CreateOrderViewModel
    {
        // تم تغيير اسم الخاصيتين لتكون أكثر عمومية
        public string FromUserId { get; set; } = default!;
        public string FromUserName { get; set; } = default!;

        // باقي الخصائص كما هي
        public bool IsPatient { get; set; }
        public string CurrentUserId { get; set; } = default!;
        public string? PharmacyId { get; set; }
        public string? PharmacyName { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
        [Display(Name = "Upload Prescription (Optional)")]
        public IFormFile? PrescriptionFile { get; set; }
        public string? Notes { get; set; }
    }

    // عنصر في الطلب
    public class OrderItemViewModel
    {
        public int InventoryId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = default!;
        public string? BrandName { get; set; }
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int Quantity { get; set; } = 1;

        public decimal Total => UnitPrice * Quantity;
    }

    // عرض الفاتورة
    public class InvoiceViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PatientName { get; set; } = default!;
        public string PharmacyName { get; set; } = default!;
        public string? PharmacyAddress { get; set; }
        public List<InvoiceItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = default!;
        public string DeliveryTime { get; set; }
    }

    public class InvoiceItemViewModel
    {
        public string MedicineName { get; set; } = default!;
        public string? BrandName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }


    // ملخص الطلبات
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PharmacyName { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int TotalItems { get; set; }
    }

    // للبحث السريع
    public class MedicineSearchResult
    {
        public int InventoryId { get; set; }
        public string MedicineName { get; set; } = default!;
        public string? BrandName { get; set; }
        public string SourceName { get; set; } = default!;
        public string SourceId { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }
    // Request Models
    public class AddToCartRequest
    {
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
    }
    // في ViewModels/IncomingOrdersViewModel.cs
    public class IncomingOrdersViewModel
    {
        public List<OrderSummaryViewModel> IncomingOrders { get; set; } = new List<OrderSummaryViewModel>();
    }


    public class PharmacyActionViewModel
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
    }

    public class CreatePrescriptionOrderViewModel
    {
        [Required]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        [Required(ErrorMessage = "Please upload a prescription file.")]
        [Display(Name = "Prescription File")]
        public IFormFile PrescriptionFile { get; set; }
        public string? Notes { get; set; }
        public bool IsPatient { get; set; }
    }
}
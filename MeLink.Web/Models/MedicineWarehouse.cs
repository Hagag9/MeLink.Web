namespace MeLink.Web.Models
{
    public class MedicineWarehouse : ApplicationUser
    {
        public string? WarehouseCode { get; set; }
        public string? Capacity { get; set; }
        public int? MaxPaymentPeriodInDays { get; set; }
    }
}

namespace MeLink.Web.Models
{
    public class UserRelation
    {
        public int Id { get; set; }

        public string FromUserId { get; set; }  // User A
        public string ToUserId { get; set; }    // User B

        public RelationType RelationType { get; set; }

        public ApplicationUser FromUser { get; set; }
        public ApplicationUser ToUser { get; set; }
    }

    public enum RelationType
    {
        PatientPharmacy = 1,        // مريض → صيدلية  
        PharmacyCompany = 2,        // صيدلية → شركة موزعة
        PharmacyWarehouse = 3,      // صيدلية → مخزن أدوية
        PharmacyManufacturer = 4,   // صيدلية → مصنع
        CompanyWarehouse = 5,       // شركة → مخزن أدوية
        CompanyManufacturer = 6,    // شركة → مصنع
        WarehouseManufacturer = 7   // مخزن → مصنع
    }
}

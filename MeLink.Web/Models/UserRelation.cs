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
        PharmacyCompany = 1,        // علاقة بين صيدلية وشركة موزعة
        PharmacyWarehouse = 2,      // علاقة بين صيدلية ومخزن أدوية
        PharmacyManufacturer = 3,   // علاقة بين صيدلية وشركة مصنّعة
        CompanyWarehouse = 4,       // علاقة بين شركة موزعة ومخزن أدوية
        CompanyManufacturer = 5,    // علاقة بين شركة موزعة وشركة مصنّعة
        WarehouseManufacturer = 6   // علاقة بين مخزن أدوية وشركة مصنّعة
    }


}

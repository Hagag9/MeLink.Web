// في ملف Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using MeLink.Web.Models;

namespace MeLink.Web.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // إنشاء الأدوية أولاً
            await SeedMedicines(context);

            // إنشاء المستخدمين
            await SeedUsers(userManager, context);

            // إنشاء الـ Inventory
            await SeedInventory(context);

            // إنشاء العلاقات
            await SeedUserRelations(context);
        }

        private static async Task SeedMedicines(ApplicationDbContext context)
        {
            if (context.Medicines.Any()) return;

            var medicines = new List<Medicine>
            {
                new Medicine
                {
                    GenericName = "Paracetamol",
                    BrandName = "Panadol",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    ManufacturerName = "GlaxoSmithKline",
                    ActiveIngredient = "Paracetamol"
                },
                new Medicine
                {
                    GenericName = "Ibuprofen",
                    BrandName = "Brufen",
                    DosageForm = "Tablet",
                    Strength = "400mg",
                    ManufacturerName = "Abbott",
                    ActiveIngredient = "Ibuprofen"
                },
                new Medicine
                {
                    GenericName = "Amoxicillin",
                    BrandName = "Augmentin",
                    DosageForm = "Capsule",
                    Strength = "500mg",
                    ManufacturerName = "GlaxoSmithKline",
                    ActiveIngredient = "Amoxicillin"
                },
                new Medicine
                {
                    GenericName = "Omeprazole",
                    BrandName = "Losec",
                    DosageForm = "Capsule",
                    Strength = "20mg",
                    ManufacturerName = "AstraZeneca",
                    ActiveIngredient = "Omeprazole"
                },
                new Medicine
                {
                    GenericName = "Aspirin",
                    BrandName = "Aspegic",
                    DosageForm = "Tablet",
                    Strength = "75mg",
                    ManufacturerName = "Sanofi",
                    ActiveIngredient = "Acetylsalicylic Acid"
                },
                new Medicine
                {
                    GenericName = "Metformin",
                    BrandName = "Glucophage",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    ManufacturerName = "Merck",
                    ActiveIngredient = "Metformin HCl"
                },
                new Medicine
                {
                    GenericName = "Atorvastatin",
                    BrandName = "Lipitor",
                    DosageForm = "Tablet",
                    Strength = "20mg",
                    ManufacturerName = "Pfizer",
                    ActiveIngredient = "Atorvastatin Calcium"
                },
                new Medicine
                {
                    GenericName = "Cetirizine",
                    BrandName = "Zyrtec",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    ManufacturerName = "Johnson & Johnson",
                    ActiveIngredient = "Cetirizine HCl"
                }
            };

            context.Medicines.AddRange(medicines);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // مرضى
            await CreatePatientAsync(userManager, "patient1@test.com", "أحمد", "محمد", "القاهرة", 30.0444, 31.2357);
            await CreatePatientAsync(userManager, "patient2@test.com", "فاطمة", "علي", "الجيزة", 30.0131, 31.2089);
            await CreatePatientAsync(userManager, "patient3@test.com", "محمد", "حسن", "الإسكندرية", 31.2001, 29.9187);

            // صيدليات
            await CreatePharmacyAsync(userManager, "pharmacy1@test.com", "صيدلية النهضة", "القاهرة", 30.0626, 31.2497, "PH001", "9:00 AM - 10:00 PM");
            await CreatePharmacyAsync(userManager, "pharmacy2@test.com", "صيدلية الشفاء", "القاهرة", 30.0444, 31.2357, "PH002", "8:00 AM - 11:00 PM");
            await CreatePharmacyAsync(userManager, "pharmacy3@test.com", "صيدلية الأمل", "الجيزة", 30.0131, 31.2089, "PH003", "9:00 AM - 9:00 PM");
            await CreatePharmacyAsync(userManager, "pharmacy4@test.com", "صيدلية البحر", "الإسكندرية", 31.2001, 29.9187, "PH004", "8:30 AM - 10:30 PM");

            // شركات توزيع
            await CreateDistributionCompanyAsync(userManager, "company1@test.com", "شركة الدلتا للأدوية", "القاهرة", 30.0444, 31.2357, "DC001", "أحمد محمود");
            await CreateDistributionCompanyAsync(userManager, "company2@test.com", "شركة النيل فارما", "الجيزة", 30.0131, 31.2089, "DC002", "سارة إبراهيم");

            // مخازن
            await CreateWarehouseAsync(userManager, "warehouse1@test.com", "مخزن الشرق الأوسط", "القاهرة", 30.0444, 31.2357, "WH001", "5000 متر مكعب");
            await CreateWarehouseAsync(userManager, "warehouse2@test.com", "مخزن المستقبل", "الإسكندرية", 31.2001, 29.9187, "WH002", "8000 متر مكعب");

            // مصانع
            await CreateManufacturerAsync(userManager, "manufacturer1@test.com", "مصنع فارما إيجيبت", "القاهرة", 30.0444, 31.2357, "MF001", "10000 وحدة/يوم", "د. خالد سليم");
        }

        private static async Task CreatePatientAsync(UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string city, double lat, double lng)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var patient = new Patient
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    DisplayName = $"{firstName} {lastName}",
                    City = city,
                    Address = $"عنوان تجريبي، {city}",
                    Latitude = lat,
                    Longitude = lng,
                    Gender = firstName == "فاطمة" || firstName == "سارة" ? Gender.Female : Gender.Male,
                    DateOfBirth = DateTime.Now.AddYears(-new Random().Next(25, 65))
                };

                await userManager.CreateAsync(patient, "Test123!");
            }
        }

        private static async Task CreatePharmacyAsync(UserManager<ApplicationUser> userManager, string email, string name, string city, double lat, double lng, string license, string hours)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var pharmacy = new Pharmacy
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = name,
                    City = city,
                    Address = $"عنوان {name}، {city}",
                    Latitude = lat,
                    Longitude = lng,
                    LicenseNumber = license,
                    WorkingHours = hours
                };

                await userManager.CreateAsync(pharmacy, "Test123!");
            }
        }

        private static async Task CreateDistributionCompanyAsync(UserManager<ApplicationUser> userManager, string email, string name, string city, double lat, double lng, string regNo, string contactPerson)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var company = new DistributionCompany
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = name,
                    City = city,
                    Address = $"عنوان {name}، {city}",
                    Latitude = lat,
                    Longitude = lng,
                    CompanyRegistrationNo = regNo,
                    ContactPersonName = contactPerson,
                    MaxPaymentPeriodInDays = 30
                };

                await userManager.CreateAsync(company, "Test123!");
            }
        }

        private static async Task CreateWarehouseAsync(UserManager<ApplicationUser> userManager, string email, string name, string city, double lat, double lng, string code, string capacity)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var warehouse = new MedicineWarehouse
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = name,
                    City = city,
                    Address = $"عنوان {name}، {city}",
                    Latitude = lat,
                    Longitude = lng,
                    WarehouseCode = code,
                    Capacity = capacity,
                    MaxPaymentPeriodInDays = 15
                };

                await userManager.CreateAsync(warehouse, "Test123!");
            }
        }

        private static async Task CreateManufacturerAsync(UserManager<ApplicationUser> userManager, string email, string name, string city, double lat, double lng, string regNo, string capacity, string contactPerson)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var manufacturer = new Manufacturer
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = name,
                    City = city,
                    Address = $"عنوان {name}، {city}",
                    Latitude = lat,
                    Longitude = lng,
                    FactoryRegistrationNo = regNo,
                    ProductionCapacity = capacity,
                    ContactPersonName = contactPerson,
                    MaxPaymentPeriodInDays = 45
                };

                await userManager.CreateAsync(manufacturer, "Test123!");
            }
        }

        private static async Task SeedInventory(ApplicationDbContext context)
        {
            if (context.Inventories.Any()) return;

            var medicines = context.Medicines.ToList();
            var pharmacies = context.Users.OfType<Pharmacy>().ToList();
            var random = new Random();

            var inventories = new List<Inventory>();

            foreach (var pharmacy in pharmacies)
            {
                // كل صيدلية تبيع 5-7 أدوية مختلفة
                var selectedMedicines = medicines.OrderBy(x => random.Next()).Take(random.Next(5, 8));

                foreach (var medicine in selectedMedicines)
                {
                    inventories.Add(new Inventory
                    {
                        UserId = pharmacy.Id,
                        MedicineId = medicine.Id,
                        Price = random.Next(10, 200), // أسعار من 10 إلى 200 جنيه
                        StockQuantity = random.Next(10, 100),
                        IsAvailable = true
                    });
                }
            }

            context.Inventories.AddRange(inventories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUserRelations(ApplicationDbContext context)
        {
            if (context.UserRelations.Any()) return;

            var patients = context.Users.OfType<Patient>().ToList();
            var pharmacies = context.Users.OfType<Pharmacy>().ToList();
            var companies = context.Users.OfType<DistributionCompany>().ToList();
            var warehouses = context.Users.OfType<MedicineWarehouse>().ToList();
            var manufacturers = context.Users.OfType<Manufacturer>().ToList();

            var relations = new List<UserRelation>();

            // علاقات المرضى بالصيدليات
            foreach (var patient in patients)
            {
                foreach (var pharmacy in pharmacies)
                {
                    relations.Add(new UserRelation
                    {
                        FromUserId = patient.Id,
                        ToUserId = pharmacy.Id,
                        RelationType = RelationType.PatientPharmacy
                    });
                }
            }

            // علاقات الصيدليات بالشركات والمخازن
            foreach (var pharmacy in pharmacies)
            {
                foreach (var company in companies)
                {
                    relations.Add(new UserRelation
                    {
                        FromUserId = pharmacy.Id,
                        ToUserId = company.Id,
                        RelationType = RelationType.PharmacyCompany
                    });
                }

                foreach (var warehouse in warehouses)
                {
                    relations.Add(new UserRelation
                    {
                        FromUserId = pharmacy.Id,
                        ToUserId = warehouse.Id,
                        RelationType = RelationType.PharmacyWarehouse
                    });
                }
            }

            // علاقات الشركات بالمصانع
            foreach (var company in companies)
            {
                foreach (var manufacturer in manufacturers)
                {
                    relations.Add(new UserRelation
                    {
                        FromUserId = company.Id,
                        ToUserId = manufacturer.Id,
                        RelationType = RelationType.CompanyManufacturer
                    });
                }
            }

            // علاقات المخازن بالمصانع
            foreach (var warehouse in warehouses)
            {
                foreach (var manufacturer in manufacturers)
                {
                    relations.Add(new UserRelation
                    {
                        FromUserId = warehouse.Id,
                        ToUserId = manufacturer.Id,
                        RelationType = RelationType.WarehouseManufacturer
                    });
                }
            }

            context.UserRelations.AddRange(relations);
            await context.SaveChangesAsync();
        }
    }
}


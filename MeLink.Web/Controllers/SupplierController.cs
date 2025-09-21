using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeLink.Web.Data;
using MeLink.Web.Models;
using MeLink.Web.ViewModels;

namespace MeLink.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplierController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();

            var types = new List<SupplierTypeViewModel>();

            if (currentUser is Pharmacy)
            {
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = "Manufacturers", Description = "Order from factories.", Icon = "fas fa-industry", Color = "warning" });
                types.Add(new SupplierTypeViewModel { Type = "Warehouse", Title = "Medicine Warehouses", Description = "Order from local warehouses.", Icon = "fas fa-warehouse", Color = "info" });
                types.Add(new SupplierTypeViewModel { Type = "DistributionCompany", Title = "Distribution Companies", Description = "Order from wholesalers.", Icon = "fas fa-truck", Color = "success" });
            }
            else if (currentUser is MedicineWarehouse)
            {
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = "Manufacturers", Description = "Order from factories.", Icon = "fas fa-industry", Color = "warning" });
                types.Add(new SupplierTypeViewModel { Type = "DistributionCompany", Title = "Distribution Companies", Description = "Order from wholesalers.", Icon = "fas fa-truck", Color = "success" });
            }
            // **تمت إضافة هذا الشرط لـ DistributionCompany**
            else if (currentUser is DistributionCompany)
            {
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = "Manufacturers", Description = "Order from factories.", Icon = "fas fa-industry", Color = "warning" });
            }
            // يمكنك إضافة المزيد من الشروط هنا لأنواع مستخدمين أخرى

            return View(types);
        }

        public async Task<IActionResult> ListByType(string type)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();

            var supplierIds = new List<string>();
            var pageTitle = "";

            // **تمت إضافة هذا الشرط لـ DistributionCompany**
            if (currentUser is DistributionCompany)
            {
                pageTitle = "Connected Manufacturers";
                var relationType = RelationType.CompanyManufacturer;

                supplierIds = await _context.UserRelations
                    .Where(r => r.FromUserId == currentUser.Id && r.RelationType == relationType)
                    .Select(r => r.ToUserId)
                    .ToListAsync();
            }
            // **تم تعديل هذا الشرط ليصبح else if**
            else if (currentUser is Pharmacy)
            {
                pageTitle = type switch
                {
                    "Manufacturer" => "Connected Manufacturers",
                    "Warehouse" => "Connected Warehouses",
                    "DistributionCompany" => "Connected Distribution Companies",
                    _ => "Connected Suppliers"
                };

                var relationType = type switch
                {
                    "Manufacturer" => RelationType.PharmacyManufacturer,
                    "Warehouse" => RelationType.PharmacyWarehouse,
                    "DistributionCompany" => RelationType.PharmacyCompany,
                    _ => RelationType.PatientPharmacy
                };

                supplierIds = await _context.UserRelations
                    .Where(r => r.FromUserId == currentUser.Id && r.RelationType == relationType)
                    .Select(r => r.ToUserId)
                    .ToListAsync();
            }
            else if (currentUser is MedicineWarehouse)
            {
                pageTitle = type switch
                {
                    "Manufacturer" => "Connected Manufacturers",
                    "DistributionCompany" => "Connected Distribution Companies",
                    _ => "Connected Suppliers"
                };

                var relationType = type switch
                {
                    "Manufacturer" => RelationType.WarehouseManufacturer,
                    "DistributionCompany" => RelationType.CompanyWarehouse,
                    _ => RelationType.PatientPharmacy
                };

                supplierIds = await _context.UserRelations
                    .Where(r => r.FromUserId == currentUser.Id && r.RelationType == relationType)
                    .Select(r => r.ToUserId)
                    .ToListAsync();
            }
            // يمكنك إضافة المزيد من الشروط هنا لأنواع مستخدمين أخرى

            var suppliers = await _context.Users
                .Where(u => supplierIds.Contains(u.Id))
                .Select(u => new SupplierViewModel
                {
                    UserId = u.Id,
                    DisplayName = u.DisplayName!,
                    Address = u.Address,
                    City = u.City,
                    UserType = u.GetType().Name,
                    Installment =u.Installment
                })
                .ToListAsync();

            var viewModel = new UserSuppliersViewModel
            {
                PageTitle = pageTitle,
                Suppliers = suppliers
            };

            return View(viewModel);
        }
    }
}
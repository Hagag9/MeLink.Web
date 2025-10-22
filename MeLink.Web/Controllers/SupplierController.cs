using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeLink.Web.Data;
using MeLink.Web.Models;
using MeLink.Web.ViewModels;
using Microsoft.Extensions.Localization; 

namespace MeLink.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<SupplierController> _localizer; 

      
        public SupplierController(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  IStringLocalizer<SupplierController> localizer)
        {
            _context = context;
            _userManager = userManager;
            _localizer = localizer; 
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();

            var types = new List<SupplierTypeViewModel>();

            if (currentUser is Pharmacy)
            {
                // استخدام الـ Localizer لجلب النصوص
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = _localizer["ManufacturersTitle"], Description = _localizer["ManufacturersDescription"], Icon = "fas fa-industry", Color = "warning" });
                types.Add(new SupplierTypeViewModel { Type = "Warehouse", Title = _localizer["WarehousesTitle"], Description = _localizer["WarehousesDescription"], Icon = "fas fa-warehouse", Color = "info" });
                types.Add(new SupplierTypeViewModel { Type = "DistributionCompany", Title = _localizer["DistributionTitle"], Description = _localizer["DistributionDescription"], Icon = "fas fa-truck", Color = "success" });
            }
            else if (currentUser is MedicineWarehouse)
            {
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = _localizer["ManufacturersTitle"], Description = _localizer["ManufacturersDescription"], Icon = "fas fa-industry", Color = "warning" });
                types.Add(new SupplierTypeViewModel { Type = "DistributionCompany", Title = _localizer["DistributionTitle"], Description = _localizer["DistributionDescription"], Icon = "fas fa-truck", Color = "success" });
                types.Add(new SupplierTypeViewModel { Type = "Warehouse", Title = _localizer["OtherWarehousesTitle"], Description = _localizer["OtherWarehousesDescription"], Icon = "fas fa-warehouse", Color = "info" });
            }
            else if (currentUser is DistributionCompany)
            {
                types.Add(new SupplierTypeViewModel { Type = "Manufacturer", Title = _localizer["ManufacturersTitle"], Description = _localizer["ManufacturersDescription"], Icon = "fas fa-industry", Color = "warning" });
            }

            return View(types);
        }

        public async Task<IActionResult> ListByType(string type)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Forbid();

            IQueryable<ApplicationUser> suppliersQuery = _context.Users.AsQueryable();
            string pageTitle; // <-- تم إزالة القيمة الافتراضية

            switch (type)
            {
                case "Manufacturer":
                    suppliersQuery = _context.Users.OfType<Manufacturer>();
                    pageTitle = _localizer["PageTitle_Manufacturers"]; // <-- استخدام الـ Localizer
                    break;
                case "Warehouse":
                    suppliersQuery = _context.Users.OfType<MedicineWarehouse>().Where(w => w.Id != currentUser.Id);
                    pageTitle = _localizer["PageTitle_OtherWarehouses"]; // <-- استخدام الـ Localizer
                    break;
                case "DistributionCompany":
                    suppliersQuery = _context.Users.OfType<DistributionCompany>();
                    pageTitle = _localizer["PageTitle_DistributionCompanies"]; // <-- استخدام الـ Localizer
                    break;
                default:
                    return BadRequest(_localizer["InvalidSupplierType"]); // <-- استخدام الـ Localizer
            }

            var suppliers = await suppliersQuery
                .Select(u => new SupplierViewModel
                {
                    UserId = u.Id,
                    DisplayName = u.DisplayName!,
                    Address = u.Address,
                    City = u.City,
                    UserType = u.GetType().Name,
                    Installment = u.Installment
                })
                .ToListAsync();

            if (currentUser is DistributionCompany && type != "Manufacturer")
                suppliers.Clear();
            if (currentUser is MedicineWarehouse && !new[] { "Manufacturer", "DistributionCompany", "Warehouse" }.Contains(type))
                suppliers.Clear();

            var viewModel = new UserSuppliersViewModel
            {
                PageTitle = pageTitle,
                Suppliers = suppliers
            };

            return View(viewModel);
        }
    }
}
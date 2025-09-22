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
                types.Add(new SupplierTypeViewModel { Type = "Warehouse", Title = "Other Warehouses", Description = "Order from other warehouses.", Icon = "fas fa-warehouse", Color = "info" });
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

            IQueryable<ApplicationUser> suppliersQuery = _context.Users.AsQueryable();
            var pageTitle = $"{type}s";

            switch (type)
            {
                case "Manufacturer":
                    suppliersQuery = _context.Users.OfType<Manufacturer>();
                    break;
                case "Warehouse":
                    suppliersQuery = _context.Users.OfType<MedicineWarehouse>().Where(w => w.Id != currentUser.Id);
                    pageTitle = "Other Warehouses";
                    break;
                case "DistributionCompany":
                    suppliersQuery = _context.Users.OfType<DistributionCompany>();
                    pageTitle = "Distribution Companies";
                    break;
                default:
                    return BadRequest("Invalid supplier type.");
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

            // This logic ensures that only valid supplier types are shown based on the current user's role.
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
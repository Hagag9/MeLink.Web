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
    public class PharmacyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PharmacyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Suppliers()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // التحقق من أن المستخدم الحالي هو صيدلي
            if (currentUser is not Pharmacy)
            {
                return Forbid(); // منع الوصول
            }

            // جلب العلاقات الخاصة بالمستخدم الحالي كصيدلي
            var relationships = await _context.UserRelations
                .Include(r => r.ToUser)
                .Where(r => r.FromUserId == currentUser.Id &&
                            (r.RelationType == RelationType.PharmacyCompany ||
                             r.RelationType == RelationType.PharmacyWarehouse ||
                             r.RelationType == RelationType.PharmacyManufacturer))
                .ToListAsync();

            // تعبئة الـ ViewModel
            var viewModel = new PharmacySuppliersViewModel
            {
                Suppliers = relationships.Select(r => new SupplierViewModel
                {
                    UserId = r.ToUser.Id,
                    DisplayName = r.ToUser.DisplayName!,
                    Address = r.ToUser.Address,
                    City = r.ToUser.City,
                    // تحديد نوع المستخدم بناءً على الـ Discriminator
                    UserType = r.ToUser switch
                    {
                        DistributionCompany => "Distriputuon Company",
                        MedicineWarehouse => "Warehouse",
                        Manufacturer => "Manufacturer",
                        _ => "Unknown"
                    }
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
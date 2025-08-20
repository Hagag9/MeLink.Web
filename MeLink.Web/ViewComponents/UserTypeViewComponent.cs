using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MeLink.Web.Models;
using System.Threading.Tasks;

namespace MeLink.Web.ViewComponents
{
    public class UserTypeViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserTypeViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // الحصول على كائن المستخدم الكامل من قاعدة البيانات
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // إرسال كائن المستخدم إلى الـ View الخاص بالـ View Component
            return View(currentUser);
        }
    }
}
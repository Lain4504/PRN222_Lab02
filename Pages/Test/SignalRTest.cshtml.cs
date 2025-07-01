using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HuynhNgocTien_SE18B01_A02.Pages.Test
{
    public class SignalRTestModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Check if user is authenticated
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            // Only allow admin to access test page
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            return Page();
        }
    }
}

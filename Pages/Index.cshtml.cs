using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HuynhNgocTien_SE18B01_A02.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("AccountId").HasValue)
            {
                return RedirectToPage("/Home/Index");
            }

            // If not logged in, redirect to Login page
            return RedirectToPage("/Account/Login");
        }
    }
}

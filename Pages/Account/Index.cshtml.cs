using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public IndexModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        public IEnumerable<SystemAccount> Accounts { get; set; } = new List<SystemAccount>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            Accounts = await _accountService.GetAllAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(short id)
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            await _accountService.DeleteAsync(id);
            return RedirectToPage();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Account
{
    public class CreateModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public CreateModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccountViewModel AccountData { get; set; } = new();

        public IActionResult OnGet()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if email already exists
            var existingAccount = await _accountService.GetByEmailAsync(AccountData.AccountEmail);
            if (existingAccount != null)
            {
                ModelState.AddModelError("AccountData.AccountEmail", "Email already exists.");
                return Page();
            }

            var account = new SystemAccount
            {
                AccountName = AccountData.AccountName,
                AccountEmail = AccountData.AccountEmail,
                AccountRole = AccountData.AccountRole,
                AccountPassword = AccountData.AccountPassword
            };

            await _accountService.CreateAsync(account);
            return RedirectToPage("./Index");
        }
    }
}

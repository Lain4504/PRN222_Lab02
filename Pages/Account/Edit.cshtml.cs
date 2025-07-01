using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public EditModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccountViewModel AccountData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(short id)
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            AccountData = new SystemAccountViewModel
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName ?? "",
                AccountEmail = account.AccountEmail ?? "",
                AccountRole = account.AccountRole ?? 0
            };

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

            var account = await _accountService.GetByIdAsync(AccountData.AccountId);
            if (account == null)
            {
                return NotFound();
            }

            account.AccountName = AccountData.AccountName;
            account.AccountEmail = AccountData.AccountEmail;
            account.AccountRole = AccountData.AccountRole;
            
            if (!string.IsNullOrEmpty(AccountData.AccountPassword))
            {
                account.AccountPassword = AccountData.AccountPassword;
            }

            await _accountService.UpdateAsync(account);
            return RedirectToPage("./Index");
        }
    }
}

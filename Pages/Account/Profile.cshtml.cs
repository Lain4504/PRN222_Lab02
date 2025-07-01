using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly ISignalRNotificationService _signalRService;

        public ProfileModel(ISystemAccountService accountService, ISignalRNotificationService signalRService)
        {
            _accountService = accountService;
            _signalRService = signalRService;
        }

        [BindProperty]
        public SystemAccountViewModel AccountData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var account = await _accountService.GetByIdAsync((short)accountId.Value);
            if (account == null)
            {
                return RedirectToPage("/Account/Login");
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
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = await _accountService.GetByIdAsync((short)accountId.Value);
            if (account == null)
            {
                return RedirectToPage("/Account/Login");
            }

            account.AccountName = AccountData.AccountName;
            account.AccountEmail = AccountData.AccountEmail;
            
            if (!string.IsNullOrEmpty(AccountData.AccountPassword))
            {
                account.AccountPassword = AccountData.AccountPassword;
            }

            await _accountService.UpdateAsync(account);
            
            // Update session
            HttpContext.Session.SetString("AccountName", account.AccountName ?? "");
            
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var accountName = HttpContext.Session.GetString("AccountName");

            // Send SignalR notification for user logout
            if (accountId.HasValue)
            {
                try
                {
                    await _signalRService.NotifyUserLogoutAsync((short)accountId.Value, accountName ?? "Unknown");
                }
                catch (Exception ex)
                {
                    // Log error but don't fail logout
                    Console.WriteLine($"SignalR logout notification failed: {ex.Message}");
                }
            }

            HttpContext.Session.Clear();
            return RedirectToPage("/Account/Login");
        }
    }
}

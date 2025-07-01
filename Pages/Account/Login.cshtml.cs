using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;

namespace HuynhNgocTien_SE18B01_A02.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly ISignalRNotificationService _signalRService;

        public LoginModel(ISystemAccountService accountService, ISignalRNotificationService signalRService)
        {
            _accountService = accountService;
            _signalRService = signalRService;
        }

        [BindProperty]
        public LoginViewModel LoginData { get; set; } = new();

        public void OnGet()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetInt32("AccountId").HasValue)
            {
                Response.Redirect("/Home");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = await _accountService.GetByEmailAsync(LoginData.Email);
            
            if (account == null || account.AccountPassword != LoginData.Password)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            // Set session
            HttpContext.Session.SetInt32("AccountId", account.AccountId);
            HttpContext.Session.SetString("AccountName", account.AccountName ?? "");
            HttpContext.Session.SetInt32("AccountRole", account.AccountRole ?? 0);

            // Send SignalR notification for user login
            try
            {
                await _signalRService.NotifyUserLoginAsync(account.AccountId, account.AccountName ?? "Unknown");
            }
            catch (Exception ex)
            {
                // Log error but don't fail login
                Console.WriteLine($"SignalR login notification failed: {ex.Message}");
            }

            return RedirectToPage("/Home/Index");
        }
    }
}

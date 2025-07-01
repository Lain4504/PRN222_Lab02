using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class HistoryModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public HistoryModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public IEnumerable<Models.NewsArticle> Articles { get; set; } = new List<Models.NewsArticle>();
        public int UserRole { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            UserRole = HttpContext.Session.GetInt32("AccountRole") ?? 0;
            UserId = accountId.Value;

            // Only Staff can view their own history
            if (UserRole != 1)
            {
                return RedirectToPage("/Home/Index");
            }

            var allArticles = await _newsService.GetAllAsync();
            Articles = allArticles
                .Where(a => a.CreatedById == UserId)
                .OrderByDescending(a => a.ModifiedDate ?? a.CreatedDate);

            return Page();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public IndexModel(INewsArticleService newsService)
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

            var allArticles = await _newsService.GetAllAsync();

            // Filter articles based on user role
            if (UserRole == 1) // Staff - can see all articles
            {
                Articles = allArticles;
            }
            else if (UserRole == 2) // Lecturer - only published articles
            {
                Articles = allArticles.Where(a => a.NewsStatus == true);
            }
            else if (UserRole == 3) // Admin - only published articles (no CRUD access)
            {
                Articles = allArticles.Where(a => a.NewsStatus == true);
            }
            else
            {
                return RedirectToPage("/Account/Login");
            }

            Articles = Articles.OrderByDescending(a => a.CreatedDate);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            var userId = HttpContext.Session.GetInt32("AccountId");

            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var article = await _newsService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check permissions - only staff can delete all articles
            if (userRole == 1)
            {
                await _newsService.DeleteAsync(id);
                TempData["Success"] = "Article deleted successfully!";
            }
            else
            {
                TempData["Error"] = "You don't have permission to delete this article.";
            }

            return RedirectToPage();
        }
    }
}

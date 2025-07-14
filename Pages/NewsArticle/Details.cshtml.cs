using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public DetailsModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public Models.NewsArticle Article { get; set; } = new();
        public int UserRole { get; set; }
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            UserRole = HttpContext.Session.GetInt32("AccountRole") ?? 0;
            UserId = accountId.Value;

            var article = await _newsService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check permissions
            if (UserRole == 2 || UserRole == 3) // Lecturer and Admin - only published articles
            {
                if (article.NewsStatus != true)
                {
                    return RedirectToPage("/NewsArticle/Index");
                }
            }
            // Staff (role 1) can view all articles

            Article = article;
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
                return RedirectToPage("/NewsArticle/Index");
            }
            else
            {
                TempData["Error"] = "You don't have permission to delete this article.";
                return RedirectToPage();
            }
        }
    }
}

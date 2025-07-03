using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.ViewModels;

namespace HuynhNgocTien_SE18B01_A02.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public IndexModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IEnumerable<Models.NewsArticle> RecentArticles { get; set; } = new List<Models.NewsArticle>();
        public IEnumerable<Models.Category> Categories { get; set; } = new List<Models.Category>();
        public int TotalArticles { get; set; }
        public int TotalCategories { get; set; }
        public int UserRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            UserRole = HttpContext.Session.GetInt32("AccountRole") ?? 0;
            ViewData["UserRole"] = UserRole;

            // Get recent articles (last 5)
            var allArticles = await _newsService.GetAllAsync();
            RecentArticles = allArticles
                .Where(a => a.NewsStatus == true)
                .OrderByDescending(a => a.CreatedDate)
                .Take(5);

            // Get categories
            Categories = await _categoryService.GetAllAsync();

            // Create category statistics for ViewBag
            var categoryStats = Categories.Select(c => new CategoryStatsViewModel
            {
                Category = c,
                ArticleCount = allArticles.Count(a => a.CategoryId == c.CategoryId && a.NewsStatus == true)
            }).Where(cs => cs.Category.IsActive == true).OrderByDescending(cs => cs.ArticleCount).ToList();

            ViewData["CategoryStats"] = categoryStats;

            // Get statistics
            TotalArticles = allArticles.Count();
            TotalCategories = Categories.Count();

            return Page();
        }

        public async Task<IActionResult> OnGetCategoryStatsAsync()
        {
            var allArticles = await _newsService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var categoryStats = categories.Select(c => new CategoryStatsViewModel
            {
                Category = c,
                ArticleCount = allArticles.Count(a => a.CategoryId == c.CategoryId && a.NewsStatus == true)
            }).Where(cs => cs.Category.IsActive == true).OrderByDescending(cs => cs.ArticleCount).ToList();

            return new JsonResult(categoryStats);
        }
    }
}

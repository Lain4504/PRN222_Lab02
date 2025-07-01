using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class ReportModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _accountService;

        public ReportModel(INewsArticleService newsService, ICategoryService categoryService, ISystemAccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public int TotalArticles { get; set; }
        public int PublishedArticles { get; set; }
        public int DraftArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public IEnumerable<Models.NewsArticle> RecentArticles { get; set; } = new List<Models.NewsArticle>();
        public IEnumerable<Models.NewsArticle> Articles { get; set; } = new List<Models.NewsArticle>();
        public IEnumerable<CategoryStats> CategoryStats { get; set; } = new List<CategoryStats>();
        public IEnumerable<AuthorStats> AuthorStats { get; set; } = new List<AuthorStats>();
        public Dictionary<string, int> ArticlesByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ArticlesByAuthor { get; set; } = new Dictionary<string, int>();

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            // Set default date range if not provided
            if (!StartDate.HasValue)
                StartDate = DateTime.Now.AddMonths(-1);
            if (!EndDate.HasValue)
                EndDate = DateTime.Now;

            await LoadReportData();
            return Page();
        }

        private async Task LoadReportData()
        {
            var allArticles = await _newsService.GetAllAsync();
            var filteredArticles = allArticles.Where(a => 
                a.CreatedDate >= StartDate && a.CreatedDate <= EndDate).ToList();

            // Basic statistics
            TotalArticles = filteredArticles.Count;
            PublishedArticles = filteredArticles.Count(a => a.NewsStatus == true);
            DraftArticles = filteredArticles.Count(a => a.NewsStatus != true);

            // Recent articles
            RecentArticles = filteredArticles
                .OrderByDescending(a => a.CreatedDate)
                .Take(10);

            // All articles for the table
            Articles = filteredArticles.OrderByDescending(a => a.CreatedDate);

            // Category statistics
            var categories = await _categoryService.GetAllAsync();
            TotalCategories = categories.Count();

            CategoryStats = categories.Select(c => new CategoryStats
            {
                CategoryName = c.CategoryName,
                ArticleCount = filteredArticles.Count(a => a.CategoryId == c.CategoryId)
            }).Where(cs => cs.ArticleCount > 0).OrderByDescending(cs => cs.ArticleCount);

            // Articles by category for chart
            ArticlesByCategory = CategoryStats.ToDictionary(cs => cs.CategoryName, cs => cs.ArticleCount);

            // Author statistics
            var accounts = await _accountService.GetAllAsync();
            TotalAuthors = accounts.Count(a => a.AccountRole == 1); // Staff only

            AuthorStats = accounts.Where(a => a.AccountRole == 1).Select(a => new AuthorStats
            {
                AuthorName = a.AccountName ?? "Unknown",
                ArticleCount = filteredArticles.Count(art => art.CreatedById == a.AccountId)
            }).Where(aus => aus.ArticleCount > 0).OrderByDescending(aus => aus.ArticleCount);

            // Articles by author for chart
            ArticlesByAuthor = AuthorStats.ToDictionary(aus => aus.AuthorName, aus => aus.ArticleCount);
        }
    }

    public class CategoryStats
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }

    public class AuthorStats
    {
        public string AuthorName { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }
}

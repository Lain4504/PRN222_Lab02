using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class PublicViewModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public PublicViewModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IEnumerable<Models.NewsArticle> Articles { get; set; } = new List<Models.NewsArticle>();
        public SelectList Categories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
        
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public short? CategoryId { get; set; }

        public async Task OnGetAsync()
        {
            // Load categories for dropdown
            var categories = await _categoryService.GetAllAsync();
            Categories = new SelectList(categories.Where(c => c.IsActive == true), "CategoryId", "CategoryName");

            // Get all published articles
            var allArticles = await _newsService.GetAllAsync();
            Articles = allArticles.Where(a => a.NewsStatus == true);

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchString))
            {
                Articles = Articles.Where(a => 
                    a.NewsTitle.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    (a.NewsContent != null && a.NewsContent.Contains(SearchString, StringComparison.OrdinalIgnoreCase)) ||
                    (a.Headline != null && a.Headline.Contains(SearchString, StringComparison.OrdinalIgnoreCase)));
            }

            // Apply category filter
            if (CategoryId.HasValue)
            {
                Articles = Articles.Where(a => a.CategoryId == CategoryId.Value);
            }

            // Order by creation date (newest first)
            Articles = Articles.OrderByDescending(a => a.CreatedDate);
        }
    }
}

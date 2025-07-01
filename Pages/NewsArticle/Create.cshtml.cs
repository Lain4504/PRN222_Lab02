using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public CreateModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public NewsArticleViewModel ArticleData { get; set; } = new();

        public SelectList Categories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
        public IEnumerable<Models.Tag> AvailableTags { get; set; } = new List<Models.Tag>();

        // Properties that the view expects directly on the model
        [BindProperty]
        public bool? NewsStatus { get; set; } = true;

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is staff
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 1)
            {
                return RedirectToPage("/Home/Index");
            }

            await LoadCategories();
            await LoadTags();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is staff
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            var userId = HttpContext.Session.GetInt32("AccountId");
            
            if (userRole != 1 || !userId.HasValue)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                await LoadTags();
                return Page();
            }

            // Generate new ID
            var allArticles = await _newsService.GetAllAsync();
            var maxId = allArticles.Any() ? 
                allArticles.Max(a => int.Parse(a.NewsArticleId.Substring(2))) : 0;
            var newId = $"NA{maxId + 1:D3}";

            var article = new Models.NewsArticle
            {
                NewsArticleId = newId,
                NewsTitle = ArticleData.NewsTitle,
                Headline = ArticleData.Headline,
                NewsContent = ArticleData.NewsContent,
                NewsSource = ArticleData.NewsSource,
                CategoryId = ArticleData.CategoryId,
                NewsStatus = ArticleData.NewsStatus,
                CreatedById = (short)userId.Value,
                CreatedDate = DateTime.Now
            };

            // Parse tags if provided
            var tagIds = new List<int>();
            if (!string.IsNullOrEmpty(ArticleData.Tags))
            {
                var tagNames = ArticleData.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToList();
                // For now, we'll skip tag processing - you can implement this later
            }

            await _newsService.CreateAsync(article, tagIds);
            TempData["Success"] = "Article created successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadCategories()
        {
            var categories = await _categoryService.GetActiveAsync();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }

        private async Task LoadTags()
        {
            try
            {
                AvailableTags = await _newsService.GetAllTagsAsync();
            }
            catch
            {
                AvailableTags = new List<Models.Tag>();
            }
        }
    }
}

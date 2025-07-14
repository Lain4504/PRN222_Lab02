using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public EditModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public NewsArticleViewModel ArticleData { get; set; } = new();

        public SelectList Categories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
        public SelectList AvailableCategories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
        public IEnumerable<Models.Tag> AvailableTags { get; set; } = new List<Models.Tag>();

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
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

            // Check permissions - only staff can edit all articles
            if (userRole != 1)
            {
                return RedirectToPage("./Index");
            }
            // Admin (role 3) can edit all articles

            // Parse NewsArticleId safely
            short articleIdNumber = 0;
            if (!string.IsNullOrEmpty(article.NewsArticleId) &&
                article.NewsArticleId.Length > 2 &&
                article.NewsArticleId.StartsWith("NA"))
            {
                short.TryParse(article.NewsArticleId.Substring(2), out articleIdNumber);
            }

            ArticleData = new NewsArticleViewModel
            {
                NewsArticleId = articleIdNumber,
                NewsTitle = article.NewsTitle ?? "",
                Headline = article.Headline ?? "",
                NewsContent = article.NewsContent ?? "",
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId ?? 0,
                NewsStatus = article.NewsStatus ?? false,
                CreatedDate = article.CreatedDate,
                ModifiedDate = article.ModifiedDate,
                CreatedById = article.CreatedById
            };

            CreatedDate = article.CreatedDate;
            ModifiedDate = article.ModifiedDate;

            // Load selected tags for this article
            SelectedTagIds = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();

            await LoadCategories();
            await LoadTags();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            var userId = HttpContext.Session.GetInt32("AccountId");

            // Debug: Log received data
            Console.WriteLine($"Edit OnPostAsync - SelectedTagIds count: {SelectedTagIds?.Count ?? 0}");
            Console.WriteLine($"Edit OnPostAsync - SelectedTagIds: [{string.Join(", ", SelectedTagIds ?? new List<int>())}]");

            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                await LoadTags();

                // Initialize SelectedTagIds if null
                if (SelectedTagIds == null)
                {
                    SelectedTagIds = new List<int>();
                }

                return Page();
            }

            var article = await _newsService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check permissions - only staff can edit all articles
            if (userRole != 1)
            {
                return RedirectToPage("./Index");
            }
            // Admin (role 3) can edit all articles

            // Update from ArticleData ViewModel
            article.NewsTitle = ArticleData.NewsTitle;
            article.Headline = ArticleData.Headline;
            article.NewsContent = ArticleData.NewsContent;
            article.NewsSource = ArticleData.NewsSource;
            article.CategoryId = ArticleData.CategoryId;
            article.NewsStatus = ArticleData.NewsStatus;
            article.ModifiedDate = DateTime.Now;
            article.UpdatedById = (short)userId.Value;

            // Use selected tag IDs from the form
            var tagIds = SelectedTagIds ?? new List<int>();

            // Debug: Log selected tag IDs
            Console.WriteLine($"Selected Tag IDs: {string.Join(", ", tagIds)}");

            await _newsService.UpdateAsync(article, tagIds);
            TempData["Success"] = "Article updated successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveAsync();
                if (categories != null)
                {
                    Categories = new SelectList(categories, "CategoryId", "CategoryName");
                    AvailableCategories = new SelectList(categories, "CategoryId", "CategoryName");
                }
                else
                {
                    Categories = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
                    AvailableCategories = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
                }
            }
            catch
            {
                Categories = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
                AvailableCategories = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");
            }
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

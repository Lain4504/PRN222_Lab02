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
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        // Properties that the view expects directly on the model
        [BindProperty]
        public string NewsArticleId { get; set; } = string.Empty;
        [BindProperty]
        public string NewsTitle { get; set; } = string.Empty;
        [BindProperty]
        public string? Headline { get; set; }
        [BindProperty]
        public string NewsContent { get; set; } = string.Empty;
        [BindProperty]
        public string? NewsSource { get; set; }
        [BindProperty]
        public short CategoryId { get; set; }
        [BindProperty]
        public short? ParentCategoryId { get; set; }
        [BindProperty]
        public bool IsActive { get; set; } = true;
        [BindProperty]
        public bool? NewsStatus { get; set; }
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

            // Check permissions
            if (userRole == 1 && article.CreatedById != userId) // Staff can only edit their own
            {
                return RedirectToPage("./Index");
            }
            else if (userRole != 1 && userRole != 3) // Only Staff and Admin can edit
            {
                return RedirectToPage("./Index");
            }

            ArticleData = new NewsArticleViewModel
            {
                NewsArticleId = short.Parse(article.NewsArticleId.Substring(2)),
                NewsTitle = article.NewsTitle ?? "",
                Headline = article.Headline ?? "",
                NewsContent = article.NewsContent ?? "",
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId ?? 0,
                NewsStatus = article.NewsStatus,
                CreatedDate = article.CreatedDate,
                ModifiedDate = article.ModifiedDate,
                CreatedById = article.CreatedById
            };

            // Populate direct properties for the view
            NewsArticleId = article.NewsArticleId;
            NewsTitle = article.NewsTitle ?? "";
            Headline = article.Headline;
            NewsContent = article.NewsContent ?? "";
            NewsSource = article.NewsSource;
            CategoryId = article.CategoryId ?? 0;
            NewsStatus = article.NewsStatus;
            CreatedDate = article.CreatedDate;
            ModifiedDate = article.ModifiedDate;

            await LoadCategories();
            await LoadTags();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            var userId = HttpContext.Session.GetInt32("AccountId");
            
            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                await LoadTags();
                return Page();
            }

            var article = await _newsService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check permissions
            if (userRole == 1 && article.CreatedById != userId)
            {
                return RedirectToPage("./Index");
            }
            else if (userRole != 1 && userRole != 3)
            {
                return RedirectToPage("./Index");
            }

            // Update from direct properties
            article.NewsTitle = NewsTitle;
            article.Headline = Headline;
            article.NewsContent = NewsContent;
            article.NewsSource = NewsSource;
            article.CategoryId = CategoryId;
            article.NewsStatus = NewsStatus;
            article.ModifiedDate = DateTime.Now;
            article.UpdatedById = (short)userId.Value;

            var tagIds = new List<int>();
            if (!string.IsNullOrEmpty(ArticleData.Tags))
            {
                // Parse tags - implement later if needed
            }

            await _newsService.UpdateAsync(article, tagIds);
            TempData["Success"] = "Article updated successfully!";
            return RedirectToPage("./Details", new { id = id });
        }

        private async Task LoadCategories()
        {
            var categories = await _categoryService.GetActiveAsync();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
            AvailableCategories = new SelectList(categories, "CategoryId", "CategoryName");
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

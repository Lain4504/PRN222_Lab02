using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public CategoryViewModel CategoryData { get; set; } = new();

        public SelectList ParentCategories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            await LoadParentCategories();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid)
            {
                await LoadParentCategories();
                return Page();
            }

            var category = new Models.Category
            {
                CategoryName = CategoryData.CategoryName,
                CategoryDesciption = CategoryData.CategoryDesciption,
                ParentCategoryId = CategoryData.ParentCategoryId,
                IsActive = CategoryData.IsActive
            };

            await _categoryService.CreateAsync(category);
            TempData["Success"] = "Category created successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadParentCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            ParentCategories = new SelectList(categories, "CategoryId", "CategoryName");
        }
    }
}

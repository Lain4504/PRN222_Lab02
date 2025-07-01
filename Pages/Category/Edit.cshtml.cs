using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.ViewModels;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Category
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public CategoryViewModel CategoryData { get; set; } = new();

        public SelectList ParentCategories { get; set; } = new SelectList(new List<Models.Category>(), "CategoryId", "CategoryName");

        public async Task<IActionResult> OnGetAsync(short id)
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            if (userRole != 3)
            {
                return RedirectToPage("/Home/Index");
            }

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            CategoryData = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive ?? true
            };

            await LoadParentCategories(id);
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
                await LoadParentCategories(CategoryData.CategoryId);
                return Page();
            }

            try
            {
                var category = await _categoryService.GetByIdAsync(CategoryData.CategoryId);
                if (category == null)
                {
                    return NotFound();
                }

                category.CategoryName = CategoryData.CategoryName;
                category.CategoryDesciption = CategoryData.CategoryDesciption;
                category.ParentCategoryId = CategoryData.ParentCategoryId;
                category.IsActive = CategoryData.IsActive;

                await _categoryService.UpdateAsync(category);
                TempData["Success"] = "Category updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating category: {ex.Message}";
                await LoadParentCategories(CategoryData.CategoryId);
                return Page();
            }
        }

        private async Task LoadParentCategories(short excludeId)
        {
            var categories = await _categoryService.GetAllAsync();
            // Exclude current category to prevent circular reference
            var availableCategories = categories.Where(c => c.CategoryId != excludeId);
            ParentCategories = new SelectList(availableCategories, "CategoryId", "CategoryName");
        }
    }
}

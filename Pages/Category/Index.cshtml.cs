using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<Models.Category> Categories { get; set; } = new List<Models.Category>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user has permission (Admin or Staff)
            var userRole = HttpContext.Session.GetInt32("AccountRole");

            if (userRole != 3 && userRole != 1)
            {
                return RedirectToPage("/Home/Index");
            }

            Categories = await _categoryService.GetAllAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(short id)
        {
            Console.WriteLine($"Delete Category called with ID: {id}");

            // Check if user is admin
            var userRole = HttpContext.Session.GetInt32("AccountRole");
            Console.WriteLine($"User role: {userRole}");

            if (userRole != 3)
            {
                Console.WriteLine("Access denied - not admin");
                return RedirectToPage("/Home/Index");
            }

            try
            {
                Console.WriteLine($"Attempting to delete category with ID: {id}");
                await _categoryService.DeleteAsync(id);
                Console.WriteLine("Category deleted successfully");
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Business logic error: {ex.Message}");
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting the category.";
            }

            return RedirectToPage();
        }
    }
}

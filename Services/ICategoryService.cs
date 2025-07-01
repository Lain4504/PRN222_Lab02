
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Services;

public interface ICategoryService
{
    Task<Category?> GetByIdAsync(short id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(short id);
    Task<IEnumerable<Category>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(short id);
    Task<bool> HasNewsArticlesAsync(short id);
} 
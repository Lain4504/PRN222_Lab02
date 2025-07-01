using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.Repositories;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Hubs;
using HuynhNgocTien_SE18B01_A02.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace HuynhNgocTien_SE18B01_A02.Services.Implement;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly INewsArticleRepository _newsRepository;
    private readonly IHubContext<CategoryHub> _hubContext;

    public CategoryService(ICategoryRepository categoryRepository, INewsArticleRepository newsRepository, IHubContext<CategoryHub> hubContext)
    {
        _categoryRepository = categoryRepository;
        _newsRepository = newsRepository;
        _hubContext = hubContext;
    }

    public async Task<Category?> GetByIdAsync(short id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveAsync()
    {
        var allCategories = await _categoryRepository.GetAllAsync();
        return allCategories.Where(c => c.IsActive == true);
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(short parentId)
    {
        return await _categoryRepository.GetSubCategoriesAsync(parentId);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        // Validate parent category if specified
        if (category.ParentCategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(category.ParentCategoryId.Value))
            {
                throw new InvalidOperationException("Parent category does not exist");
            }
        }

        var createdCategory = await _categoryRepository.AddAsync(category);

        // Send SignalR notification
        try
        {
            var categoryDto = createdCategory.ToDto();
            await _hubContext.Clients.Group("Categories").SendAsync("CategoryCreated", categoryDto);
            await _hubContext.Clients.All.SendAsync("CategoryListUpdated");
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return createdCategory;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(category.CategoryId);
        if (existingCategory == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Validate parent category if specified
        if (category.ParentCategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(category.ParentCategoryId.Value))
            {
                throw new InvalidOperationException("Parent category does not exist");
            }

            // Prevent circular reference
            if (category.ParentCategoryId == category.CategoryId)
            {
                throw new InvalidOperationException("Category cannot be its own parent");
            }
        }

        var updatedCategory = await _categoryRepository.UpdateAsync(category);

        // Send SignalR notification
        try
        {
            var categoryDto = updatedCategory.ToDto();
            await _hubContext.Clients.Group("Categories").SendAsync("CategoryUpdated", categoryDto);
            await _hubContext.Clients.All.SendAsync("CategoryListUpdated");
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return updatedCategory;
    }

    public async Task DeleteAsync(short id)
    {
        if (!await _categoryRepository.ExistsAsync(id))
        {
            throw new InvalidOperationException("Category not found");
        }

        // Check if category has any news articles
        if (await HasNewsArticlesAsync(id))
        {
            throw new InvalidOperationException("Cannot delete category that has news articles");
        }

        // Check if category has any subcategories
        if (await _categoryRepository.HasSubCategoriesAsync(id))
        {
            throw new InvalidOperationException("Cannot delete category that has subcategories");
        }

        await _categoryRepository.DeleteAsync(id);

        // Send SignalR notification
        try
        {
            await _hubContext.Clients.Group("Categories").SendAsync("CategoryDeleted", id);
            await _hubContext.Clients.All.SendAsync("CategoryListUpdated");
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
    {
        var allCategories = await _categoryRepository.GetAllAsync();
        return allCategories.Where(c =>
            c.CategoryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            c.CategoryDesciption.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> ExistsAsync(short id)
    {
        return await _categoryRepository.ExistsAsync(id);
    }

    public async Task<bool> HasNewsArticlesAsync(short id)
    {
        var newsInCategory = await _newsRepository.GetByCategoryAsync(id);
        return newsInCategory.Any();
    }
} 
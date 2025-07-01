using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Repositories;

public interface INewsArticleRepository
{
    Task<NewsArticle?> GetByIdAsync(string id);
    Task<IEnumerable<NewsArticle>> GetAllAsync();
    Task<IEnumerable<NewsArticle>> GetByCategoryAsync(short categoryId);
    Task<IEnumerable<NewsArticle>> GetByAuthorAsync(short authorId);
    Task<IEnumerable<NewsArticle>> GetByTagAsync(int tagId);
    Task<NewsArticle> AddAsync(NewsArticle article);
    Task<NewsArticle> UpdateAsync(NewsArticle article);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Tag>> GetAllTagsAsync();
} 
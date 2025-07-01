using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
} 
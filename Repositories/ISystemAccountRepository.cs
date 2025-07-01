using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Repositories;

public interface ISystemAccountRepository
{
    Task<SystemAccount?> GetByIdAsync(short id);
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task<IEnumerable<SystemAccount>> GetAllAsync();
    Task<SystemAccount> AddAsync(SystemAccount account);
    Task<SystemAccount> UpdateAsync(SystemAccount account);
    Task DeleteAsync(short id);
    Task<bool> ExistsAsync(short id);
    Task<bool> ExistsByEmailAsync(string email);
} 
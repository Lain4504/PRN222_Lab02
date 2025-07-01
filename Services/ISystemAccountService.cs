using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Services;

public interface ISystemAccountService
{
    Task<SystemAccount?> GetByIdAsync(short id);
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task<IEnumerable<SystemAccount>> GetAllAsync();
    Task<SystemAccount> CreateAsync(SystemAccount account);
    Task<SystemAccount> UpdateAsync(SystemAccount account);
    Task DeleteAsync(short id);
    Task<bool> ValidateLoginAsync(string email, string password);
    Task<IEnumerable<SystemAccount>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(short id);
    Task<bool> ExistsByEmailAsync(string email);
} 
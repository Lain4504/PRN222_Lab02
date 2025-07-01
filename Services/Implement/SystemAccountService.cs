using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.Repositories;
using HuynhNgocTien_SE18B01_A02.Hubs;
using HuynhNgocTien_SE18B01_A02.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace HuynhNgocTien_SE18B01_A02.Services.Implement;

public class SystemAccountService : ISystemAccountService
{
    private readonly ISystemAccountRepository _accountRepository;
    private readonly IHubContext<SystemAccountHub> _hubContext;

    public SystemAccountService(ISystemAccountRepository accountRepository, IHubContext<SystemAccountHub> hubContext)
    {
        _accountRepository = accountRepository;
        _hubContext = hubContext;
    }

    public async Task<SystemAccount?> GetByIdAsync(short id)
    {
        return await _accountRepository.GetByIdAsync(id);
    }

    public async Task<SystemAccount?> GetByEmailAsync(string email)
    {
        return await _accountRepository.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<SystemAccount>> GetAllAsync()
    {
        return await _accountRepository.GetAllAsync();
    }

    public async Task<SystemAccount> CreateAsync(SystemAccount account)
    {
        if (await _accountRepository.ExistsByEmailAsync(account.AccountEmail))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var createdAccount = await _accountRepository.AddAsync(account);

        // Send SignalR notification
        try
        {
            var accountDto = createdAccount.ToDto();
            await _hubContext.Clients.Group("SystemAccounts").SendAsync("AccountCreated", accountDto);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return createdAccount;
    }

    public async Task<SystemAccount> UpdateAsync(SystemAccount account)
    {
        var existingAccount = await _accountRepository.GetByIdAsync(account.AccountId);
        if (existingAccount == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        // Check if email is being changed and if it already exists
        if (existingAccount.AccountEmail != account.AccountEmail &&
            await _accountRepository.ExistsByEmailAsync(account.AccountEmail))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var updatedAccount = await _accountRepository.UpdateAsync(account);

        // Send SignalR notification
        try
        {
            var accountDto = updatedAccount.ToDto();
            await _hubContext.Clients.Group("SystemAccounts").SendAsync("AccountUpdated", accountDto);
            await _hubContext.Clients.Group($"User_{accountDto.AccountId}").SendAsync("ProfileUpdated", accountDto);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return updatedAccount;
    }

    public async Task DeleteAsync(short id)
    {
        if (!await _accountRepository.ExistsAsync(id))
        {
            throw new InvalidOperationException("Account not found");
        }

        await _accountRepository.DeleteAsync(id);

        // Send SignalR notification
        try
        {
            await _hubContext.Clients.Group("SystemAccounts").SendAsync("AccountDeleted", id);
            await _hubContext.Clients.Group($"User_{id}").SendAsync("ForceLogout", "Your account has been deleted.");
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }
    }

    public async Task<bool> ValidateLoginAsync(string email, string password)
    {
        var account = await _accountRepository.GetByEmailAsync(email);
        return account != null && account.AccountPassword == password;
    }

    public async Task<IEnumerable<SystemAccount>> SearchAsync(string searchTerm)
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Where(a =>
            a.AccountName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
            a.AccountEmail?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
    }

    public async Task<bool> ExistsAsync(short id)
    {
        return await _accountRepository.ExistsAsync(id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _accountRepository.ExistsByEmailAsync(email);
    }
} 
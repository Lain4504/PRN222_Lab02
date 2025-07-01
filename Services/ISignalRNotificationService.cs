using HuynhNgocTien_SE18B01_A02.DTOs;

namespace HuynhNgocTien_SE18B01_A02.Services
{
    public interface ISignalRNotificationService
    {
        Task NotifyNewsArticleCreatedAsync(NewsArticleDto article);
        Task NotifyNewsArticleUpdatedAsync(NewsArticleDto article);
        Task NotifyNewsArticleDeletedAsync(string articleId, short? categoryId);
        Task NotifyNewsArticleStatusChangedAsync(string articleId, bool status);
        
        Task NotifyCategoryCreatedAsync(CategoryDto category);
        Task NotifyCategoryUpdatedAsync(CategoryDto category);
        Task NotifyCategoryDeletedAsync(short categoryId);
        Task NotifyCategoryStatusChangedAsync(short categoryId, bool isActive);
        
        Task NotifyAccountCreatedAsync(SystemAccountDto account);
        Task NotifyAccountUpdatedAsync(SystemAccountDto account);
        Task NotifyAccountDeletedAsync(short accountId);
        Task NotifyUserLoginAsync(short accountId, string accountName);
        Task NotifyUserLogoutAsync(short accountId, string accountName);
    }
}

using Microsoft.AspNetCore.SignalR;
using HuynhNgocTien_SE18B01_A02.DTOs;
using HuynhNgocTien_SE18B01_A02.Hubs;

namespace HuynhNgocTien_SE18B01_A02.Services.Implement
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<NewsArticleHub> _newsArticleHubContext;
        private readonly IHubContext<CategoryHub> _categoryHubContext;
        private readonly IHubContext<SystemAccountHub> _systemAccountHubContext;

        public SignalRNotificationService(
            IHubContext<NewsArticleHub> newsArticleHubContext,
            IHubContext<CategoryHub> categoryHubContext,
            IHubContext<SystemAccountHub> systemAccountHubContext)
        {
            _newsArticleHubContext = newsArticleHubContext;
            _categoryHubContext = categoryHubContext;
            _systemAccountHubContext = systemAccountHubContext;
        }

        public async Task NotifyNewsArticleCreatedAsync(NewsArticleDto article)
        {
            try
            {
                await _newsArticleHubContext.Clients.Group("NewsArticles").SendAsync("ArticleCreated", article);
                if (article.CategoryId.HasValue)
                {
                    await _newsArticleHubContext.Clients.Group($"Category_{article.CategoryId}").SendAsync("ArticleCreated", article);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyNewsArticleUpdatedAsync(NewsArticleDto article)
        {
            try
            {
                await _newsArticleHubContext.Clients.Group("NewsArticles").SendAsync("ArticleUpdated", article);
                if (article.CategoryId.HasValue)
                {
                    await _newsArticleHubContext.Clients.Group($"Category_{article.CategoryId}").SendAsync("ArticleUpdated", article);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyNewsArticleDeletedAsync(string articleId, short? categoryId)
        {
            try
            {
                await _newsArticleHubContext.Clients.Group("NewsArticles").SendAsync("ArticleDeleted", articleId);
                if (categoryId.HasValue)
                {
                    await _newsArticleHubContext.Clients.Group($"Category_{categoryId}").SendAsync("ArticleDeleted", articleId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyNewsArticleStatusChangedAsync(string articleId, bool status)
        {
            try
            {
                await _newsArticleHubContext.Clients.Group("NewsArticles").SendAsync("ArticleStatusChanged", articleId, status);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyCategoryCreatedAsync(CategoryDto category)
        {
            try
            {
                await _categoryHubContext.Clients.Group("Categories").SendAsync("CategoryCreated", category);
                await _categoryHubContext.Clients.All.SendAsync("CategoryListUpdated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyCategoryUpdatedAsync(CategoryDto category)
        {
            try
            {
                await _categoryHubContext.Clients.Group("Categories").SendAsync("CategoryUpdated", category);
                await _categoryHubContext.Clients.All.SendAsync("CategoryListUpdated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyCategoryDeletedAsync(short categoryId)
        {
            try
            {
                await _categoryHubContext.Clients.Group("Categories").SendAsync("CategoryDeleted", categoryId);
                await _categoryHubContext.Clients.All.SendAsync("CategoryListUpdated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyCategoryStatusChangedAsync(short categoryId, bool isActive)
        {
            try
            {
                await _categoryHubContext.Clients.Group("Categories").SendAsync("CategoryStatusChanged", categoryId, isActive);
                await _categoryHubContext.Clients.All.SendAsync("CategoryListUpdated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyAccountCreatedAsync(SystemAccountDto account)
        {
            try
            {
                await _systemAccountHubContext.Clients.Group("SystemAccounts").SendAsync("AccountCreated", account);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyAccountUpdatedAsync(SystemAccountDto account)
        {
            try
            {
                await _systemAccountHubContext.Clients.Group("SystemAccounts").SendAsync("AccountUpdated", account);
                await _systemAccountHubContext.Clients.Group($"User_{account.AccountId}").SendAsync("ProfileUpdated", account);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyAccountDeletedAsync(short accountId)
        {
            try
            {
                await _systemAccountHubContext.Clients.Group("SystemAccounts").SendAsync("AccountDeleted", accountId);
                await _systemAccountHubContext.Clients.Group($"User_{accountId}").SendAsync("ForceLogout", "Your account has been deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyUserLoginAsync(short accountId, string accountName)
        {
            try
            {
                await _systemAccountHubContext.Clients.Group("SystemAccounts").SendAsync("UserLoggedIn", accountId, accountName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }

        public async Task NotifyUserLogoutAsync(short accountId, string accountName)
        {
            try
            {
                await _systemAccountHubContext.Clients.Group("SystemAccounts").SendAsync("UserLoggedOut", accountId, accountName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR notification failed: {ex.Message}");
            }
        }
    }
}

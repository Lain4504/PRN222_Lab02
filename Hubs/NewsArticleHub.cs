using Microsoft.AspNetCore.SignalR;
using HuynhNgocTien_SE18B01_A02.DTOs;

namespace HuynhNgocTien_SE18B01_A02.Hubs
{
    public class NewsArticleHub : BaseHub
    {
        public async Task JoinNewsArticleGroup()
        {
            if (IsUserAuthenticated())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "NewsArticles");
            }
        }

        public async Task LeaveNewsArticleGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "NewsArticles");
        }

        public async Task JoinCategoryGroup(string categoryId)
        {
            if (IsUserAuthenticated())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
            }
        }

        public async Task LeaveCategoryGroup(string categoryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
        }

        // Methods to be called from server-side
        public async Task NotifyArticleCreated(NewsArticleDto article)
        {
            await Clients.Group("NewsArticles").SendAsync("ArticleCreated", article);
            if (article.CategoryId.HasValue)
            {
                await Clients.Group($"Category_{article.CategoryId}").SendAsync("ArticleCreated", article);
            }
        }

        public async Task NotifyArticleUpdated(NewsArticleDto article)
        {
            await Clients.Group("NewsArticles").SendAsync("ArticleUpdated", article);
            if (article.CategoryId.HasValue)
            {
                await Clients.Group($"Category_{article.CategoryId}").SendAsync("ArticleUpdated", article);
            }
        }

        public async Task NotifyArticleDeleted(string articleId, short? categoryId)
        {
            await Clients.Group("NewsArticles").SendAsync("ArticleDeleted", articleId);
            if (categoryId.HasValue)
            {
                await Clients.Group($"Category_{categoryId}").SendAsync("ArticleDeleted", articleId);
            }
        }

        public async Task NotifyArticleStatusChanged(string articleId, bool status)
        {
            await Clients.Group("NewsArticles").SendAsync("ArticleStatusChanged", articleId, status);
        }
    }
}

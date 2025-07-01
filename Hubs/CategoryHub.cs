using Microsoft.AspNetCore.SignalR;
using HuynhNgocTien_SE18B01_A02.DTOs;

namespace HuynhNgocTien_SE18B01_A02.Hubs
{
    public class CategoryHub : BaseHub
    {
        public async Task JoinCategoryGroup()
        {
            if (IsUserAuthenticated() && IsAdmin())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Categories");
            }
        }

        public async Task LeaveCategoryGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Categories");
        }

        // Methods to be called from server-side
        public async Task NotifyCategoryCreated(CategoryDto category)
        {
            await Clients.Group("Categories").SendAsync("CategoryCreated", category);
            // Also notify all users since categories affect navigation
            await Clients.All.SendAsync("CategoryListUpdated");
        }

        public async Task NotifyCategoryUpdated(CategoryDto category)
        {
            await Clients.Group("Categories").SendAsync("CategoryUpdated", category);
            // Also notify all users since categories affect navigation
            await Clients.All.SendAsync("CategoryListUpdated");
        }

        public async Task NotifyCategoryDeleted(short categoryId)
        {
            await Clients.Group("Categories").SendAsync("CategoryDeleted", categoryId);
            // Also notify all users since categories affect navigation
            await Clients.All.SendAsync("CategoryListUpdated");
        }

        public async Task NotifyCategoryStatusChanged(short categoryId, bool isActive)
        {
            await Clients.Group("Categories").SendAsync("CategoryStatusChanged", categoryId, isActive);
            // Also notify all users since categories affect navigation
            await Clients.All.SendAsync("CategoryListUpdated");
        }
    }
}

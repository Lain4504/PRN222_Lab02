using Microsoft.AspNetCore.SignalR;
using HuynhNgocTien_SE18B01_A02.DTOs;

namespace HuynhNgocTien_SE18B01_A02.Hubs
{
    public class SystemAccountHub : BaseHub
    {
        public async Task JoinAccountGroup()
        {
            if (IsUserAuthenticated() && IsAdmin())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "SystemAccounts");
            }
        }

        public async Task LeaveAccountGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SystemAccounts");
        }

        // Methods to be called from server-side
        public async Task NotifyAccountCreated(SystemAccountDto account)
        {
            await Clients.Group("SystemAccounts").SendAsync("AccountCreated", account);
        }

        public async Task NotifyAccountUpdated(SystemAccountDto account)
        {
            await Clients.Group("SystemAccounts").SendAsync("AccountUpdated", account);
            
            // If the updated account is currently online, notify them
            await Clients.Group($"User_{account.AccountId}").SendAsync("ProfileUpdated", account);
        }

        public async Task NotifyAccountDeleted(short accountId)
        {
            await Clients.Group("SystemAccounts").SendAsync("AccountDeleted", accountId);
            
            // Force logout the deleted user if they're online
            await Clients.Group($"User_{accountId}").SendAsync("ForceLogout", "Your account has been deleted.");
        }

        public async Task NotifyUserLogin(short accountId, string accountName)
        {
            await Clients.Group("SystemAccounts").SendAsync("UserLoggedIn", accountId, accountName);
        }

        public async Task NotifyUserLogout(short accountId, string accountName)
        {
            await Clients.Group("SystemAccounts").SendAsync("UserLoggedOut", accountId, accountName);
        }
    }
}

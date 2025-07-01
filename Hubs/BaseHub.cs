using Microsoft.AspNetCore.SignalR;

namespace HuynhNgocTien_SE18B01_A02.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected bool IsUserAuthenticated()
        {
            var userId = Context.GetHttpContext()?.Session.GetInt32("AccountId");
            return userId.HasValue && userId.Value > 0;
        }

        protected int? GetCurrentUserId()
        {
            return Context.GetHttpContext()?.Session.GetInt32("AccountId");
        }

        protected int? GetCurrentUserRole()
        {
            return Context.GetHttpContext()?.Session.GetInt32("AccountRole");
        }

        protected bool IsAdmin()
        {
            var role = GetCurrentUserRole();
            return role.HasValue && role.Value == 3;
        }

        protected bool IsStaff()
        {
            var role = GetCurrentUserRole();
            return role.HasValue && (role.Value == 2 || role.Value == 3);
        }

        public override async Task OnConnectedAsync()
        {
            if (IsUserAuthenticated())
            {
                var userId = GetCurrentUserId();
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                var role = GetCurrentUserRole();
                if (role.HasValue)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
                }
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (IsUserAuthenticated())
            {
                var userId = GetCurrentUserId();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                var role = GetCurrentUserRole();
                if (role.HasValue)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Role_{role}");
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}

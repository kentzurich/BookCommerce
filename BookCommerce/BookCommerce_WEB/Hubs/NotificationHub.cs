using BookCommerce_Utility;
using Microsoft.AspNetCore.SignalR;

namespace BookCommerce_WEB.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task TriggerNotify()
        {
            await Clients.All.SendAsync("TriggerNotification");
        }
    }
}

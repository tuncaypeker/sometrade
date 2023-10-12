using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SomeTrade.Socket
{
    public class DeploymentHub : Hub
    {
        protected IHubContext<DeploymentHub> _context;

        public DeploymentHub(IHubContext<DeploymentHub> context)
        {
            _context = context;
        }

        public async Task SendMessage(string message, string color = "black")
        {
            await _context.Clients.All.SendAsync("newMessage", message, color);
        }
    }
}

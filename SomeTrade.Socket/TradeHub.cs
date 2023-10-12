using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SomeTrade.Socket
{
    public class TradeHub : Hub
    {
        protected IHubContext<TradeHub> _context;

        public TradeHub(IHubContext<TradeHub> context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">
        /// 1: price-changed  symbol:price
        /// 2: position open  symbol:
        /// 3: position close symbol:
        /// 4: normal message
        /// </param>
        /// <param name="symbol"></param>
        /// <param name="price"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task SendMessage(int type, string message, string color = "white")
        {
            await _context.Clients.All.SendAsync("newMessage",type, message, color);
        }
    }
}

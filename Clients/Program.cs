using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;

namespace Clients
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync());
        }

        private static async Task MainAsync()
        {
            var tasks = new List<Task>
            {
                TcpConsumer.ConsumeAllAsync(),
                //UdpConsumer.ConsumeAllAsync(),
            };
            await Task.WhenAll(tasks);
        }
    }
}

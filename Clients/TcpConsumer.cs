using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace Clients
{
    internal class TcpConsumer
    {
        public static async Task ConsumeAllAsync()
        {
            var clientTasks = Enumerable
                .Repeat((byte)0, Constants.ClientCount)
                .Select(_=>new TcpClient())
                .Select(client => ConsumeOneAsync(client, Constants.IpAddress, Constants.TcpPort))
                .ToArray();
            await Task.WhenAll(clientTasks);
        }

        private static async Task ConsumeOneAsync(TcpClient client, IPAddress ip, int port)
        {
            await client.ConnectAsync(ip, port);
            using (var stream = client.GetStream())
            {
                await TcpMessenger.ReadAndWriteAsync(stream, Constants.ServerToClient, Constants.ClientToServer);
            }
        }
    }
}
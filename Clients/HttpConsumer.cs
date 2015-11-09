using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Clients
{
    internal class HttpConsumer
    {
        public static async Task ConsumeAllAsync()
        {
            var clientTasks = Enumerable
                .Repeat((byte)0, Constants.ClientCount)
                .Select(_ => new HttpClient())
                .Select(client => ConsumeOneAsync(client, Constants.IpAddress, Constants.HttpPort))
                .ToArray();
            await Task.WhenAll(clientTasks);
        }

        private static async Task ConsumeOneAsync(HttpClient client, IPAddress ip, int port)
        {
            var outBytes = Encoding.UTF8.GetBytes(Constants.ClientToServer);
            var outContent=new ByteArrayContent(outBytes);
            var httpResponseMessage = await client.PostAsync($"http://{ip}:{port}/", outContent);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                TcpMessenger.ErrorConnectionReset("");
            }
            var actual = await httpResponseMessage.Content.ReadAsStringAsync();
            if (actual != Constants.ServerToClient)
            {
                TcpMessenger.ErrorWrongMessage(actual);
            }
        }
    }
}
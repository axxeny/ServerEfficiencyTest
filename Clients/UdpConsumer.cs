using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Clients
{
    internal class UdpConsumer
    {
        public static async Task ConsumeAllAsync()
        {
            await CheckConnectionAsync();
            var clientTasks = Enumerable
                .Range(0, Constants.ClientCount)
                .Select(_ => new UdpClient())
                .Select(client => ConsumeOneAsync(client, Constants.IpAddress, Constants.UdpPort))
                .ToArray();
            await Task.WhenAll(clientTasks);
        }

        private static async Task ConsumeOneAsync(UdpClient client, IPAddress ipAddress, int port)
        {
            client.Connect(ipAddress, port);
            var bytes = Encoding.UTF8.GetBytes(Constants.ClientToServer);
            var tasks = new[]
            {
                client.SendAsync(bytes, bytes.Length),
                ReceiveAndCheckAsync(client, Constants.ServerToClient)
            };
            await Task.WhenAll(tasks);
        }

        private static async Task ReceiveAndCheckAsync(UdpClient client, string expectedMessage)
        {
                var incomingBytes = new List<byte>();
            var connectionReset=false;
            try
            {
                do
                {
                    var udpReceiveResult = await client.ReceiveAsync();
                    incomingBytes.AddRange(udpReceiveResult.Buffer);
                } while (TcpMessenger.IsReadingShouldContinue(incomingBytes));
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    connectionReset = true;
                }
            }
            var actual = Encoding.UTF8.GetString(incomingBytes.ToArray());
            if (connectionReset)
            {
                TcpMessenger.ErrorConnectionReset(actual);
            }
            else if (actual != expectedMessage)
            {
                TcpMessenger.ErrorWrongMessage(actual);
            }
        }

        private static async Task CheckConnectionAsync()
        {
            var connectionCheckClient = new UdpClient();
            var success = false;
            while (!success)
            {
                try
                {
                    connectionCheckClient.Connect(Constants.IpAddress, Constants.UdpPort);
                    success = true;
                }
                catch (SocketException)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
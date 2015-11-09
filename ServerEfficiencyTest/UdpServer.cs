using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ServerEfficiencyTest
{
    internal class UdpServer
    {
        private static int _sent;

        public static async Task ServeAsync(UdpClient server)
        {
            IDictionary<IPEndPoint, List<byte>> received=new Dictionary<IPEndPoint, List<byte>>();

            var sendTasks = new List<Task>();
            while (_sent < Constants.ClientCount)
            {
                await server.ReceiveAsync()
                    .ContinueWith(udpReceivedResult => sendTasks.Add(AddReceivedToDicAndSendIfShouldAsync(server, udpReceivedResult.Result, received)));
            }
            await Task.WhenAll(sendTasks);
        }

        private static Task AddReceivedToDicAndSendIfShouldAsync(UdpClient server, UdpReceiveResult r,
            IDictionary<IPEndPoint, List<byte>> received)
        {
            var clientEndPoint = r.RemoteEndPoint;
            var buffer = r.Buffer;
            if (received.ContainsKey(clientEndPoint))
            {
                received[clientEndPoint].AddRange(buffer);
            }
            else
            {
                received.Add(clientEndPoint, buffer.ToList());
            }
            var shouldSend = !IsReadingShouldContinue(received[clientEndPoint]);
            return shouldSend 
                ? SendIfShouldAsync(server, clientEndPoint) 
                : Task.CompletedTask;
        }

        private static async Task SendIfShouldAsync(UdpClient server, IPEndPoint remoteEndPoint)
        {
            await SendOutcomingAsync(server, remoteEndPoint, Constants.ServerToClient);
            Interlocked.Increment(ref _sent);
        }

        public static async Task SendOutcomingAsync(UdpClient server, IPEndPoint clientEndPoint, string message)
        {
            var outcomingBytes = Encoding.UTF8.GetBytes(message);
            await server.SendAsync(outcomingBytes, outcomingBytes.Length, clientEndPoint);
        }

        private static bool IsReadingShouldContinue(IReadOnlyList<byte> incomingBytes)
        {
            var count = incomingBytes.Count;
            return count < 4 || incomingBytes[count - 4] != 'O' || incomingBytes[count - 3] != 'V' || incomingBytes[count - 2] != 'E' || incomingBytes[count - 1] != 'R';
        }
    }
}
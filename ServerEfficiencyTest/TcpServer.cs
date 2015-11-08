using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace ServerEfficiencyTest
{
    class TcpServer
    {
        public static async Task ProcessAsync(TcpListener server)
        {
            var tcpClient = await server.AcceptTcpClientAsync();
            using (var stream = tcpClient.GetStream())
            {
                await TcpMessenger.ReadAndWriteAsync(stream, Constants.ClientToServer, Constants.ServerToClient);
            }
        }
    }
}
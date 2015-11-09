using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace ServerEfficiencyTest
{
    internal static class TcpTest
    {
        public static async Task<Stopwatch> TestAsync()
        {
            var server = new TcpListener(Constants.IpAddress, Constants.TcpPort);
            server.Start();
            Console.WriteLine("TCP-сервер готов.");
            var tasks = new Task[Constants.ClientCount];
            var stopwatch = new Stopwatch();
            while (!server.Pending())
            {
                await Task.Delay(1000);
            }
            Console.WriteLine("Начался TCP-тест.");
            stopwatch.Start();
            foreach (var i in Enumerable.Range(0, Constants.ClientCount))
            {
                tasks[i] = TcpServer.ProcessAsync(server);
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            server.Stop();
            return stopwatch;
        }
    }
}
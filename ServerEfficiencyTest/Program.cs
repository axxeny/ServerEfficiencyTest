using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;

namespace ServerEfficiencyTest
{
    class Program
    {

        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync());
            Console.Write("Программа отработана. Нажмите Энтр!");
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            Console.WriteLine("Программа запущена.");
            Console.WriteLine($@"В этом тесте подключаются {Constants.ClientCount} клиентов,
отправляют сообщение длиной {Constants.ClientToServer.Length}
и принимают сообщение длиной {Constants.ServerToClient.Length}.
Все сообщения проверяются: как клиентом, так и сервером.");
            var tcpTime = await TestTcpAsync();
            Report("TCP", tcpTime);
            //var udpTime = await TestUdpAsync();
            //Report("UDP", udpTime);
            //var webSocketsTime = await TestWebSocketsAsync();
            //Report("System.Net.WebSockets", webSocketsTime);
            //var signalRTime = await TestSignalRAsync();
            //Report("SignalR", signalRTime);
        }

        private static void Report(string serverName, Stopwatch udpTime)
        {
            var ticksPerClient = (((double)udpTime.ElapsedTicks)/Constants.ClientCount);
            var timePerClient = udpTime.Elapsed.TotalMilliseconds/Constants.ClientCount;
            Console.WriteLine($@"{serverName}-сервер отработал.
Общее время: {udpTime.ElapsedMilliseconds.ToString("N0")} миллисекунд ({udpTime.ElapsedTicks.ToString("N0")} тиков).
Время на одного клиента: {timePerClient.ToString("N")} миллисекунд ({ticksPerClient.ToString("N")} тиков).");
        }

        private static async Task<Stopwatch> TestTcpAsync()
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

        private static async Task<Stopwatch> TestUdpAsync()
        {
            var server = new UdpClient(Constants.UdpPort);
            Console.WriteLine("UDP-сервер готов.");
            var tasks = new Task[Constants.ClientCount];
            var stopwatch = new Stopwatch();
            while (server.Available<=0)
            {
                await Task.Delay(1000);
            }
            Console.WriteLine("Начался UDP-тест.");

            stopwatch.Start();
            await UdpServer.ServeAsync(server);
            stopwatch.Stop();
            return stopwatch;
        }
    }
}

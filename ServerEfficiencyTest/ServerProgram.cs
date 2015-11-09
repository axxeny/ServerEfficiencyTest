using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Nito.AsyncEx;

namespace ServerEfficiencyTest
{
    internal class ServerProgram
    {
        private static void Main()
        {
            AsyncContext.Run(MainAsync);
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
            var tcpTime = await TcpTest.TestAsync();
            Report("TCP", tcpTime);
            //var udpTime = await TestUdpAsync();
            //Report("UDP", udpTime);
            var httpTime = await HttpTest.TestAsync();
            Report("HTTP", httpTime);
            //var signalRTime = await TestSignalRAsync();
            //Report("SignalR", signalRTime);
        }

        private static void Report(string serverName, Stopwatch udpTime)
        {
            var ticksPerClient = (double)udpTime.ElapsedTicks/Constants.ClientCount;
            var timePerClient = udpTime.Elapsed.TotalMilliseconds/Constants.ClientCount;
            Console.WriteLine($@"{serverName}-сервер отработал.
Общее время: {udpTime.ElapsedMilliseconds.ToString("N0")} миллисекунд ({udpTime.ElapsedTicks.ToString("N0")} тиков).
Время на одного клиента: {timePerClient.ToString("N")} миллисекунд ({ticksPerClient.ToString("N")} тиков).");
        }
    }
}

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;
using ServerEfficiencyTest;

static internal class UdpTest
{
    public static async Task<Stopwatch> TestUdpAsync()
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
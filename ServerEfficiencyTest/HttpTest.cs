using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ServerEfficiencyTest
{
    internal class HttpTest
    {
        //static SemaphoreSlim _semaphore = new SemaphoreSlim(100);
        public static async Task<Stopwatch> TestAsync()
        {
            var server = new HttpListener();
            server.Prefixes.Add($"http://{Constants.IpAddress}:{Constants.HttpPort}/");
            server.Start();
            Console.WriteLine("HTTP-сервер готов.");
            var stopwatch = new Stopwatch();
            var contexts = new List<HttpListenerContext>();
            var firstGetContextTask =server.GetContextAsync();
            contexts.Add(await firstGetContextTask);
            Console.WriteLine("Начался HTTP-тест.");
            stopwatch.Start();
            var getContextOtherTasks = Enumerable
                .Repeat((byte)0, Constants.ClientCount - 1)
                .Select(_ =>
                {
                    //_semaphore.Wait();
                    var contextTask = server.GetContextAsync();
                    //_semaphore.Release();
                    return contextTask;
                });
            var getContextTasks = new HashSet<Task<HttpListenerContext>>
                (getContextOtherTasks)
            {
                firstGetContextTask
            };
            await ProcessAllAsync(getContextTasks);
            stopwatch.Stop();
            server.Stop();
            return stopwatch;
        }

        private static async Task ProcessAllAsync(ICollection<Task<HttpListenerContext>> getContextTasks)
        {
            var processTasks = new List<Task>();
            while (getContextTasks.Any())
            {
                var gotContextTask = await Task.WhenAny(getContextTasks);
                getContextTasks.Remove(gotContextTask);
                processTasks.Add(HttpServer.ProcessAsync(await gotContextTask, Constants.ClientToServer, Constants.ServerToClient));
            }
            await Task.WhenAll(processTasks);
        }
    }
}
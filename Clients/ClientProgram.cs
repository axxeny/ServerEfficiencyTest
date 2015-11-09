using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Clients
{
    internal class ClientProgram
    {
        private static void Main()
        {
            AsyncContext.Run(MainAsync);
            Console.Write("Программа отработана. Нажмите Энтр!");
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            await TcpConsumer.ConsumeAllAsync();
            await HttpConsumer.ConsumeAllAsync();
        }
    }
}

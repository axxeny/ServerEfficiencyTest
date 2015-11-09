using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TcpMessenger
    {
        public static async Task ReadAndWriteAsync(NetworkStream stream, string incoming, string outcoming)
        {
            var tasks = new[]
            {
                ReadIncomingAsync(stream, incoming),
                SendOutcomingAsync(stream, outcoming)
            };
            await Task.WhenAll(tasks);
        }

        public static async Task SendOutcomingAsync(Stream stream, string message)
        {
            var outcomingBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(outcomingBytes, 0, outcomingBytes.Length);
        }

        public static async Task ReadIncomingAsync(Stream stream, string expectedMessage)
        {
            var incomingBytes = new List<byte>();
            do
            {
                var buffer = new byte[1];
                var count = await stream.ReadAsync(buffer, 0, 1);
                if (count >= 1)
                {
                    incomingBytes.Add(buffer[0]);
                }
            } while (IsReadingShouldContinue(incomingBytes));
            var actual = Encoding.UTF8.GetString(incomingBytes.ToArray());
            if (actual != expectedMessage)
            {
                ErrorWrongMessage(actual);
            }
        }

        public static void ErrorWrongMessage(string actual)
        {
            throw new ServerEfficiencyTestException(
                $"Присланное сообщение не соответствует ожидаемому. Присланное сообщение: \"{actual}\".");
        }

        public static void ErrorConnectionReset(string actual)
        {
            throw new ServerEfficiencyTestException(
                $"Соединение разорвано. Успевшие прийти символы: \"{actual}\".");
        }

        public static bool IsReadingShouldContinue(IReadOnlyList<byte> incomingBytes)
        {
            var count = incomingBytes.Count;
            return count < 4 || incomingBytes[count - 4] != 'O' || incomingBytes[count - 3] != 'V' || incomingBytes[count - 2] != 'E' || incomingBytes[count - 1] != 'R';
        }

        public static void ErrorTooLong(long actualCount, long expectedCount)
        {
            throw new ServerEfficiencyTestException(
                $"Пришло слишком длинное сообщение. Длина сообщения: {actualCount}, ожидавшаяся длина: {expectedCount}.");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ServerEfficiencyTest
{
    internal class HttpServer
    {
        public static async Task ProcessAsync(HttpListenerContext context, string expectedIncoming, string outcoming)
        {
            var request = context.Request;
            var response = context.Response;
            await ReceiveAsync(expectedIncoming, request);
            await SendAsync(outcoming, response);
            context.Response.OutputStream.Close();
        }

        private static async Task SendAsync(string outcoming, HttpListenerResponse response)
        {
            var bytes = Encoding.UTF8.GetBytes(outcoming);
            response.ContentLength64 = bytes.Length;
            var stream = response.OutputStream;
            response.ContentEncoding=Encoding.UTF8;
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static async Task ReceiveAsync(string expectedMessage, HttpListenerRequest request)
        {
            var actualCount = request.ContentLength64;
            var expectedCount = Constants.ClientToServer.Length;
            if (actualCount > expectedCount)
            {
                TcpMessenger.ErrorTooLong(actualCount, expectedCount);
            }
            var bufferSize = (int) actualCount;
            var buffer = new byte[bufferSize];
            await request.InputStream.ReadAsync(buffer, 0, bufferSize);
            request.InputStream.Close();
            var actual = DecodeUtf8(buffer);
            if (actual != expectedMessage)
            {
                TcpMessenger.ErrorWrongMessage(actual);
            }
        }

        public static string DecodeUtf8(IList<byte> bytes)
        {
            var array = bytes as byte[] ?? bytes.ToArray();
            return Encoding.UTF8.GetString(array);
        }
    }
}
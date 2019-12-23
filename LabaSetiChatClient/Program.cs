using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LabaSetiChatClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var tcpClient = new TcpClient("127.0.0.1", 8888);
            Console.WriteLine("Подключено");

            var stream = tcpClient.GetStream();
            
            while (true)
            {
                Console.Write("Введите строку для передачи: ");
                var line = Console.ReadLine();

                var bytes = Encoding.UTF8.GetBytes(line);
                stream.Write(bytes, 0, bytes.Length);
                stream.WriteByte(0);

                var response = ReadString(stream);
                if (response == "close") break;
                
                Console.WriteLine("Получено: " + response);
            }
            
            stream.Close();
            tcpClient.Close();
        }

        private static string ReadString(NetworkStream stream)
        {
            var buffer = new byte[256];
            var result = "";
            var ready = false;

            do
            {
                var len = stream.Read(buffer, 0, buffer.Length);
                if (buffer[len - 1] == 0)
                {
                    ready = true;
                    len--;
                }

                result += Encoding.UTF8.GetString(buffer, 0, len);
            }
            while (!ready);

            return result;
        }
    }
}
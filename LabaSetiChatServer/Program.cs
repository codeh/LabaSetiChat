using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabaSetiChatServer
{
    internal class Program
    {
        
        const int port = 8888;
        static void Main(string[] args)
        {
            TcpListener server=null;
            try
            {
                var localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();
 
                while (true)
                {
                    Console.WriteLine("Ожидание подключений... ");
 
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");
                    
                    var stream = client.GetStream();
                    var close = false;

                    while (true)
                    {
                        var message = ReadString(stream);

                        var response = "Сервер получил " + message;
                        Console.WriteLine(response);

                        if (message == "close")
                        {
                            close = true;
                            response = "close";
                        }

                        var bytes = Encoding.UTF8.GetBytes(response);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.WriteByte(0);
                        
                        if (close) break;
                    }

                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
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
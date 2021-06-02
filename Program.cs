using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;

namespace курсач3сервер
{
    internal static class Program
    {
   
        public static string ConnString()
        {
            return ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
        }

        private const int Port = 6722;
        private static TcpListener _listener;

        private static void Main()
        {

            DBCreator.check();
            try
            {

                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
                _listener.Start();

                while (true)
                {
                    var client = _listener.AcceptSocket();

                    var clientObject = new ClientObject(client);

                    var thread = new Thread(clientObject.Process);
                    thread.Start();
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _listener?.Stop();
            }
        }
    }
}

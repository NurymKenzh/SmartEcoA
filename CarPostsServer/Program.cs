using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CarPostsServer
{
    class Program
    {
        const int port = 8089;
        static TcpListener listener;
        static void Main(string[] args)
        {
            try
            {
                //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                //listener = new TcpListener(iPEndPoint);
                listener.Start();
                Console.WriteLine("Program started!");

                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Client client = new Client(tcpClient);

                    Thread clientThread = new Thread(new ThreadStart(client.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}

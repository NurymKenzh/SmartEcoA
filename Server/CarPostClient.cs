using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    class CarPostClient
    {
        private readonly Logger Logger;
        private readonly TextBox TextBoxLog;
        private TcpClient _tcpClient;

        public CarPostClient(TcpClient tcpClient,
            TextBox TextBoxLog)
        {
            _tcpClient = tcpClient;
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
        }
        public CarPostClient(TextBox TextBoxLog)
        {
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
        }

        public void Process()
        {
            NetworkStream stream = null;
            string ip = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
            Logger.Log($"Подключился пост с IP: {ip}");
            stream = _tcpClient.GetStream();
            try
            {
                byte[] data = new byte[_tcpClient.ReceiveBufferSize];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    Logger.Log($"Id поста: {builder.ToString()}");
                }
                while (stream.DataAvailable);
            }
            catch (Exception ex)
            {
                Logger.Log($"{ip}: ошибка ({ex.Message})");
            }
            finally
            {
                stream?.Close();
                _tcpClient?.Close();
            }
        }

        public void Process(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on
            // the console.
            TcpClient client;
                try
            {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
                

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)
            Logger.Log("Подключился пост");

            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[client.ReceiveBufferSize];
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            Logger.Log(builder.ToString());

            // Signal the calling thread to continue.
            //tcpClientConnected.Set();

            client.Close();
            //client.Client.Disconnect(true);
            Logger.Log("Пост отключился");

        }
    }
}
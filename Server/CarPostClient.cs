using Dapper;
using Newtonsoft.Json;
using Npgsql;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ListView ListViewCarPosts;
        private readonly List<CarPost> CarPosts;

        public CarPostClient(TextBox TextBoxLog,
            ListView ListView,
            List<CarPost> CarPosts)
        {
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
            ListViewCarPosts = ListView;
            this.CarPosts = CarPosts;
        }

        public void Process(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client;
            try
            {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
            string ip = (client.Client.RemoteEndPoint as IPEndPoint as IPEndPoint).Address.ToString();
            Logger.Log($"Подключился пост ({ip})");
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
            string jsonString = builder.ToString();
            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            string carPostId = (string)obj.CarPostId;
            Logger.Log($"Id поста {ip} - {carPostId}");
            UpdateListView(carPostId, ip);

            client.Close();
            Logger.Log("Пост отключился");
        }

        private void UpdateListView(
            string Id,
            string IP)
        {
            string Name = CarPosts.FirstOrDefault(c => c.Id.ToString() == Id)?.Name;
            string[] row = { Id, Name, IP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            var listViewItem = new ListViewItem(row);
            Action action = () => ListViewCarPosts.Items.Add(listViewItem);
            ListViewCarPosts.Invoke(action);
        }
    }
}
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly string SmartEcoAConnectionString;

        public CarPostClient(TextBox TextBoxLog,
            ListView ListView,
            List<CarPost> CarPosts,
            string SmartEcoAConnectionString)
        {
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
            ListViewCarPosts = ListView;
            this.CarPosts = CarPosts;
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
        }

        public void Process(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client;
            try
            {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            string ip = (client.Client.RemoteEndPoint as IPEndPoint as IPEndPoint).Address.ToString();
            Logger.Log($"Подключился пост ({ip})");
            NetworkStream stream = client.GetStream();
            // Get CarPostId
            StringBuilder messageSB = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[client.ReceiveBufferSize];
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                messageSB.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            string jsonString = messageSB.ToString();
            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            string carPostId = (string)obj.CarPostId;
            // Send message with last data
            if (!string.IsNullOrEmpty(carPostId))
            {
                Logger.Log($"Id поста {ip} - {carPostId}");
                UpdateListView(carPostId, ip);
                int carPostIdInt = -1;
                if (!string.IsNullOrEmpty(carPostId) && Int32.TryParse(carPostId, out carPostIdInt))
                {
                    string messageS = GetLastData(carPostIdInt);

                    // Send message with last data
                    data = Encoding.Unicode.GetBytes(messageS);
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    dynamic err = new ExpandoObject();
                    err.Error = "CarPostId не может быть символьным или пустым!";
                    string message = JsonConvert.SerializeObject(err);

                    // Send error
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
                UpdateListView(carPostId, ip);
            }

            client.Close();
            Logger.Log("Пост отключился");
        }

        private void UpdateListView(
            string Id,
            string IP)
        {
            string Name = CarPosts.FirstOrDefault(c => c.Id.ToString() == Id)?.Name;
            ListViewItem listViewItem = null;
            ListViewCarPosts.Invoke(new MethodInvoker(delegate
            {
                listViewItem = ListViewCarPosts.Items.Cast<ListViewItem>().FirstOrDefault(l => l.Text == Id);
                if (listViewItem == null)
                {
                    string[] row = { Id, Name, IP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                    listViewItem = new ListViewItem(row);
                    ListViewCarPosts.Items.Add(listViewItem);
                }
                else
                {
                    listViewItem.SubItems[3].Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }));
        }

        private string GetLastData(int carPostId)
        {
            string carModelSmokeMeterName = string.Empty;
            int? carModelAutoTestId = null;
            DateTime? carPostDataAutoTestDate = new DateTime();
            DateTime? carPostDataSmokeMeterDate = new DateTime();
            string testerName = string.Empty;
            try
            {
                using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                        $"FROM public.\"CarModelSmokeMeter\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1");
                    carModelSmokeMeterName = carModelSmokeMetersv.FirstOrDefault() == null ? carModelSmokeMeterName : carModelSmokeMetersv.FirstOrDefault().Name;

                    var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                        $"FROM public.\"CarModelAutoTest\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1");
                    carModelAutoTestId = carModelAutoTestsv.FirstOrDefault() == null ? carModelAutoTestId : carModelAutoTestsv.FirstOrDefault().ParadoxId;

                    var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>($"SELECT * " +
                        $"FROM public.\"CarPostDataAutoTest\" as datas " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = datas.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1");
                    carPostDataAutoTestDate = carPostDataAutoTestsv.FirstOrDefault() == null ? carPostDataAutoTestDate : carPostDataAutoTestsv.FirstOrDefault().DateTime;

                    var carPostDataSmokeMetersv = connection.Query<CarPostDataSmokeMeter>($"SELECT * " +
                        $"FROM public.\"CarPostDataSmokeMeter\" as datas " +
                        $"JOIN public.\"CarModelSmokeMeter\" as model ON model.\"Id\" = datas.\"CarModelSmokeMeterId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1");
                    carPostDataSmokeMeterDate = carPostDataSmokeMetersv.FirstOrDefault() == null ? carPostDataSmokeMeterDate : carPostDataSmokeMetersv.FirstOrDefault().DateTime;

                    var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                        $"FROM public.\"Tester\" as tester " +
                        $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY tester.\"Id\" DESC " +
                        $"LIMIT 1");
                    testerName = testersv.FirstOrDefault() == null ? testerName : testersv.FirstOrDefault().Name;
                    connection.Close();
                }

                dynamic obj = new ExpandoObject();
                obj.carModelSmokeMeterName = carModelSmokeMeterName;
                obj.carModelAutoTestId = carModelAutoTestId;
                obj.carPostDataAutoTestDate = carPostDataAutoTestDate;
                obj.carPostDataSmokeMeterDate = carPostDataSmokeMeterDate;
                string json = JsonConvert.SerializeObject(obj);

                return json;
            }
            catch (Exception ex)
            {
                dynamic err = new ExpandoObject();
                err.Error = ex.Message;
                string json = JsonConvert.SerializeObject(err);
                return json;
            }
        }
    }
}
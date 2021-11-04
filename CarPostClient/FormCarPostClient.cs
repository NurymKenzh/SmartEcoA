using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CarPostClient.Models;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NickBuhro.Translit;

namespace CarPostClient
{
    public partial class FormCarPostClient : Form
    {
        private const int port = 8087;
        private const string server = "185.125.44.116";
        //private const string server = "192.168.0.165";
        //private const string server = "192.168.43.47";
        //private const string server = "127.0.0.1";
        private string CarPostId = null,
            AutoTestPath = null,
            SmokeMeterPath = null;
        private bool stop = false;
        private OleDbConnection connection;
        private OleDbConnection connection2;

        public FormCarPostClient()
        {
            InitializeComponent();
        }

        private void FormCarPostClient_Load(object sender, EventArgs e)
        {
            backgroundWorkerCarPostClient.RunWorkerAsync();
        }

        private void FormCarPostClient_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void FormCarPostClient_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void FormCarPostClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!stop)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void notifyIconWork_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void backgroundWorkerCarPostClient_DoWork(object sender, DoWorkEventArgs e)
        {
            Log($"Версия {GetProductVersion()}");
            DateTime dateTimeTryUpdate = DateTime.Now - new TimeSpan(0, 6, 0);
            if (File.Exists("stop"))
            {
                File.Delete("stop");
            }
            
            if (!ReadAppSettings())
            {
                Log("Ошибка чтения appsettings.json! Программа будет выключена через минуту!");
                Thread.Sleep(new TimeSpan(0, 1, 0));
                return;
            }
            Log("Значения с appsettings.json прочитаны:");
            Log($"Id поста: {CarPostId},");
            Log($"путь к базе данных Автотеста: {AutoTestPath},");
            Log($"путь к базе данных Дымомера: {SmokeMeterPath},");
            Log($"сервер: {server}:{port}.");
            Log("--------------------------------------");
            var provider = "Microsoft.Jet.OLEDB.4.0";
            string connectionString = $"Provider={provider};Data Source={AutoTestPath};Extended Properties=dBase IV;";
            connection = new OleDbConnection(connectionString);
            connection.Open();

            string connectionStringSmokeMeter = $"Provider={provider};Data Source={SmokeMeterPath};Extended Properties=dBase IV;";
            connection2 = new OleDbConnection(connectionStringSmokeMeter);
            connection2.Open();

            while (!backgroundWorkerCarPostClient.CancellationPending)
            {
                try
                {
                    // run updater
                    if (DateTime.Now - dateTimeTryUpdate > new TimeSpan(0, 5, 0))
                    {
                        try
                        {
                            Log("--------------------------------------");
                            Log($"Проверка обновления");
                            Process updater = new Process();
                            updater.StartInfo.FileName = Path.Combine("Updater", "CarPostClientUpdater.exe");
                            // for debug:
                            //updater.StartInfo.FileName = @"C:\Users\N\source\repos\NurymKenzh\SmartEcoA\CarPostClientUpdater\bin\Debug\netcoreapp3.1\CarPostClientUpdater.exe";
                            updater.Start();
                            dateTimeTryUpdate = DateTime.Now;
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                        }
                        catch (Exception ex)
                        {
                            Log($"Ошибка обновления: " + ex.Message);
                        }
                    }
                    // while updater is working
                    while (File.Exists("wait"))
                    {
                        if (!textBoxLog.Lines[textBoxLog.Lines.Count() - 2].Contains("Проверка обновления"))
                        {
                            Log("Проверка обновления");
                        }
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                    // update
                    if (File.Exists("stop"))
                    {
                        Log("Обновление");
                        notifyIconWork.Visible = false;
                        stop = true;
                        Application.Exit();
                    }

                    int sent = Connect();

                    if (sent == 0)
                    {
                        Log("Пауза 5 минут");
                        Thread.Sleep(new TimeSpan(0, 5, 0));
                    }
                    else
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }
                catch { }
            }

            connection.Close();
            connection.Dispose();
            connection2.Close();
            connection2.Dispose();
        }

        private string GetProductVersion()
        {
            return Application.ProductVersion;
        }

        private bool ReadAppSettings()
        {
            try
            {
                string settingsS = System.IO.File.ReadAllText("appsettings.json");
                JObject settingsJO;
                try
                {
                    settingsJO = JObject.Parse(settingsS);
                }
                catch
                {
                    try
                    {
                        settingsS = settingsS.Replace("\\", "\\\\");
                        settingsJO = JObject.Parse(settingsS);
                    }
                    catch
                    {
                        return false;
                    }
                }
                foreach (var item in settingsJO)
                {
                    switch (item.Key)
                    {
                        case "CarPostId":
                            CarPostId = item.Value.ToString();
                            break;
                        case "AutoTestPath":
                            AutoTestPath = item.Value.ToString();
                            break;
                        case "SmokeMeterPath":
                            SmokeMeterPath = item.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private int Connect()
        {
            int sentCount = 1;
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient();
                Log("--------------------------------------");
                Log($"Попытка подключения");
                client.Connect(server, port);
                Log($"Подключился");
                stream = client.GetStream();

                while (true)
                {
                    SendCarPostId(stream);

                    //Wait response from Server
                    dynamic obj = new ExpandoObject();
                    //byte[] dataResponse = new byte[256];
                    byte[] dataResponse = new byte[client.ReceiveBufferSize];
                    while (true)
                    {
                        StringBuilder messageSB = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(dataResponse, 0, dataResponse.Length);
                            messageSB.Append(Encoding.UTF8.GetString(dataResponse, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        if (messageSB.Length != 0)
                        {
                            string jsonString = messageSB.ToString();
                            obj = JsonConvert.DeserializeObject(jsonString);
                            break;
                        }
                    }
                    //Check error CarPostId
                    if (!String.IsNullOrEmpty((string)obj.Error))
                    {
                        throw new Exception((string)obj.Error);
                    }
                    JsonData jsonData = new JsonData();

                    jsonData.carModelSmokeMeter = CreateModelSmokeMeter((int?)obj.carModelSmokeMeterId);
                    jsonData.carModelAutoTest = CreateModelAutoTest((int?)obj.carModelAutoTestId);

                    if (jsonData.carModelSmokeMeter == null)
                    {
                        jsonData.carPostDataSmokeMeter = CreateDataSmokeMeter((DateTime?)obj.carPostDataSmokeMeterDate);
                    }
                    if (jsonData.carModelAutoTest == null)
                    {
                        jsonData.carPostDataAutoTest = CreateDataAutoTest((DateTime?)obj.carPostDataAutoTestDate);
                    }

                    string json = Environment.NewLine + JsonConvert.SerializeObject(jsonData) + Environment.NewLine;

                    if (jsonData.carModelSmokeMeter != null || jsonData.carPostDataSmokeMeter != null
                        || jsonData.carModelAutoTest != null || jsonData.carPostDataAutoTest != null)
                    {
                        //Sending data to the server
                        var data = Encoding.UTF8.GetBytes(json);
                        stream.Write(data, 0, data.Length);
                        if (jsonData.carModelAutoTest != null)
                        {
                            Log($"Данные отправлены: Автотест, модель автомобиля - {jsonData.carModelAutoTest.MODEL}");
                        }
                        if (jsonData.carModelSmokeMeter != null)
                        {
                            Log($"Данные отправлены: Дымомер, модель автомобиля - {jsonData.carModelSmokeMeter.MODEL}");
                        }
                        if (jsonData.carPostDataAutoTest != null)
                        {
                            Log($"Данные отправлены: Автотест, измерение - {FromDATATIME(jsonData.carPostDataAutoTest, null).ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        if (jsonData.carPostDataSmokeMeter != null)
                        {
                            //Log($"Данные отправлены: Дымомер, номер автомобиля - {jsonData.carPostDataSmokeMeter.NOMER}");
                            Log($"Данные отправлены: Дымомер, измерение - {FromDATATIME(null, jsonData.carPostDataSmokeMeter).ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetBytes(json);
                        stream.Write(data, 0, data.Length);
                        Log("Нет новых данных для отправки");
                        sentCount = 0;
                    }

                    break;
                }
            }
            catch (SocketException ex)
            {
                Log($"Ошибка подключения к серверу: {ex.Message}");
                sentCount = 0;
            }
            catch (Exception ex)
            {
                Log($"Ошибка передачи данных: {ex.Message}");
                sentCount = 0;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (client != null && client.Connected != false)
                {
                    client.Close();
                }
                Log("Отключился");
            }
            return sentCount;
        }

        private void SendCarPostId(NetworkStream stream)
        {
            dynamic obj = new ExpandoObject();
            obj.CarPostId = CarPostId;
            string json = JsonConvert.SerializeObject(obj);

            //Sending data to the server
            var data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

        private CarModelAutoTest CreateModelAutoTest(int? autoTestModelId)
        {
            try
            {
                CarModelAutoTest carModelAutoTest = null;
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                //var provider = "Microsoft.Jet.OLEDB.4.0";

                //string connectionString = $"Provider={provider};Data Source={AutoTestPath};Extended Properties=dBase IV;";

                //using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    //connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTest>($"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    if (autoTestModelId != null)
                    {
                        carModelAutoTest = carModelAutoTests.Where(c => c.ID > autoTestModelId).FirstOrDefault();
                        if (carModelAutoTest != null)
                        {
                            var typeEco = connection.Query<TypeEco>($"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                            carModelAutoTest.TypeEcoName = typeEco.NAME;
                        }
                    }
                    else
                    {
                        carModelAutoTest = carModelAutoTests.FirstOrDefault();
                        if (carModelAutoTest != null)
                        {
                            var typeEco = connection.Query<TypeEco>($"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                            carModelAutoTest.TypeEcoName = typeEco.NAME;
                        }
                    }
                    //var indexModel = carModelAutoTests.FindIndex(c => c.ID == autoTestModelId);
                    //if (indexModel != -1)
                    //{
                    //    carModelAutoTest = carModelAutoTests.Skip(indexModel + 1).FirstOrDefault();
                    //    if (carModelAutoTest != null)
                    //    {
                    //        var typeEco = connection.Query<TypeEco>(
                    //        $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                    //        carModelAutoTest.TypeEcoName = typeEco.NAME;
                    //    }
                    //}
                    //else
                    //{
                    //    carModelAutoTest = carModelAutoTests.FirstOrDefault();
                    //    if (carModelAutoTest != null)
                    //    {
                    //        var typeEco = connection.Query<TypeEco>(
                    //        $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                    //        carModelAutoTest.TypeEcoName = typeEco.NAME;
                    //    }
                    //}
                    //connection.Close();
                }
                return carModelAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}");
            }

            return null;
        }

        private CarModelSmokeMeter CreateModelSmokeMeter(int? smokeMeterModelId)
        {
            try
            {
                    CarModelSmokeMeter carModelSmokeMeter = null;

                    var carModelSmokeMeters = connection2.Query<CarModelSmokeMeter>($"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    if (smokeMeterModelId != null)
                    {
                        carModelSmokeMeter = carModelSmokeMeters.Where(c => c.ID > smokeMeterModelId).FirstOrDefault();
                        if (carModelSmokeMeter != null)
                        {
                            var typeEco = connection2.Query<TypeEco>($"SELECT * FROM type_eco as m WHERE m.ID = {carModelSmokeMeter.ID_ECOLOG}").FirstOrDefault();
                            carModelSmokeMeter.TypeEcoName = typeEco.NAME;
                        }
                    }
                    else
                    {
                        carModelSmokeMeter = carModelSmokeMeters.FirstOrDefault();
                        if (carModelSmokeMeter != null)
                        {
                            var typeEco = connection2.Query<TypeEco>($"SELECT * FROM type_eco as m WHERE m.ID = {carModelSmokeMeter.ID_ECOLOG}").FirstOrDefault();
                            carModelSmokeMeter.TypeEcoName = typeEco.NAME;
                        }
                    }

                return carModelSmokeMeter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private CarPostDataAutoTest CreateDataAutoTest(DateTime? autoTestDataDateTime)
        {
            try
            {
                CarPostDataAutoTest carPostDataAutoTest = null;
                //var provider = "Microsoft.Jet.OLEDB.4.0";
                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");

                //string connectionString = $"Provider={provider};Data Source={AutoTestPath};Extended Properties=dBase IV;";

                //using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    //connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTest>($"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    carPostDataAutoTest = connection.Query<CarPostDataAutoTest>(
                        $"SELECT * FROM Main")
                        .ToList()
                        .Where(c => c.DATA.Year >= 2020)
                        //.Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                        .Where(c => FromDATATIME(c, null) > Convert.ToDateTime(autoTestDataDateTime))
                        .Where(c => carModelAutoTests.Select(m => m.ID).Contains(c.ID_MODEL))
                        .OrderBy(c => FromDATATIME(c, null))
                        .FirstOrDefault();
                    if (carPostDataAutoTest != null)
                    {
                        var dopInfo = connection.Query<DopInfo>(
                            $"SELECT * FROM dop_info as m WHERE m.ID = {carPostDataAutoTest.ID}").FirstOrDefault();
                        if (dopInfo != null)
                        {
                            var tester = connection.Query<Tester>(
                                $"SELECT * FROM tester as m WHERE m.ID = {dopInfo.ID_TESTER}").FirstOrDefault();
                            if (tester != null)
                            {
                                dopInfo.TesterName = Transliteration.CyrillicToLatin(tester.NAME, Language.Russian);
                            }
                            carPostDataAutoTest.DopInfo = dopInfo;
                        }
                    }
                    //connection.Close();
                }
                return carPostDataAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data autotest >> {ex.Message}");
            }

            return null;
        }

        private CarPostDataSmokeMeter CreateDataSmokeMeter(DateTime? smokeMeterDataDateTime)
        {
            try
            {
                CarPostDataSmokeMeter carPostDataSmokeMeter = null;
                var lastTime = Convert.ToDateTime(smokeMeterDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(smokeMeterDataDateTime).ToString("MM/dd/yyyy");

                var carModelSmokeMeters = connection2.Query<CarModelSmokeMeter>($"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    carPostDataSmokeMeter = connection2.Query<CarPostDataSmokeMeter>(
                        $"SELECT * FROM Main")
                        .ToList()
                        .Where(c => c.DATA.Year >= 2020)
                      //.Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(smokeMeterDataDateTime))
                        .Where(c => FromDATATIME(null, c) > Convert.ToDateTime(smokeMeterDataDateTime))
                        .Where(c => carModelSmokeMeters.Select(m => m.ID).Contains(c.ID_MODEL))
                        .OrderBy(c => FromDATATIME(null, c))
                        .FirstOrDefault();
                    if (carPostDataSmokeMeter != null)
                    {
                        var dopInfo = connection2.Query<DopInfo>(
                            $"SELECT * FROM dop_info as m WHERE m.ID = {carPostDataSmokeMeter.ID}").FirstOrDefault();
                        if (dopInfo != null)
                        {
                            var tester = connection2.Query<Tester>(
                                $"SELECT * FROM tester as m WHERE m.ID = {dopInfo.ID_TESTER}").FirstOrDefault();
                            if (tester != null)
                            {
                                dopInfo.TesterName = Transliteration.CyrillicToLatin(tester.NAME, Language.Russian);
                        }
                            carPostDataSmokeMeter.DopInfo = dopInfo;
                        }
                    }

                return carPostDataSmokeMeter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private DateTime FromDATATIME(CarPostDataAutoTest carPostDataAutoTest, CarPostDataSmokeMeter carPostDataSmokeMeter)
        {
            if(carPostDataSmokeMeter == null)
            {
                return new DateTime(carPostDataAutoTest.DATA.Year,
                    carPostDataAutoTest.DATA.Month,
                    carPostDataAutoTest.DATA.Day,
                    Convert.ToInt32(carPostDataAutoTest.TIME.Split(':')[0]),
                    Convert.ToInt32(carPostDataAutoTest.TIME.Split(':')[1]),
                    Convert.ToInt32(carPostDataAutoTest.TIME.Split(':')[2]));
            }
            else
            {
                return new DateTime(carPostDataSmokeMeter.DATA.Year,
                    carPostDataSmokeMeter.DATA.Month,
                    carPostDataSmokeMeter.DATA.Day,
                    Convert.ToInt32(carPostDataSmokeMeter.TIME.Split(':')[0]),
                    Convert.ToInt32(carPostDataSmokeMeter.TIME.Split(':')[1]),
                    Convert.ToInt32(carPostDataSmokeMeter.TIME.Split(':')[2]));
            }
        }

        public void Log(string Message)
        {
            Action action = () => textBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> {Message}{Environment.NewLine}");
            textBoxLog.Invoke(action);
        }
    }
}

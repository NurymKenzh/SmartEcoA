using CarPostsClient.Models;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarPostClient
{
    public partial class FormMain : Form
    {
        private const int port = 8087;
        //private const string server = "185.125.44.116";
        //private const string server = "192.168.0.165";
        private const string server = "192.168.43.47";
        //private const string server = "127.0.0.1";
        private string CarPostId = null,
            AutoTestPath = null,
            SmokeMeterPath = null;
        private bool stop = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIconWork_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!stop)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            backgroundWorkerMain.RunWorkerAsync();
        }

        private void backgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
        {
            Log($"Версия {GetProductVersion()}");
            DateTime dateTimeTryUpdate = DateTime.Now/* - new TimeSpan(0, 11, 0)*/;
            if (File.Exists("stop"))
            {
                File.Delete("stop");
            }
            while (!backgroundWorkerMain.CancellationPending)
            {
                // run updater
                if (DateTime.Now - dateTimeTryUpdate > new TimeSpan(0, 5, 0))
                {
                    try
                    {
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

                if (!ReadAppSettings())
                {
                    Log("Ошибка чтения appsettings.json! Перерыв на 1 минуту.");
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                    continue;
                }
                Log("--------------------------------------");
                Log("Значения с appsettings.json прочитаны:");
                Log($"Id поста: {CarPostId},");
                Log($"путь к базе данных Автотеста: {AutoTestPath},");
                Log($"путь к базе данных Дымомера: {SmokeMeterPath},");
                Log($"сервер: {server}:{port}.");

                Connect();

                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
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

        private void Connect()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient();
                Log($"Попытка подключения");
                client.Connect(server, port);
                Log($"Получаю поток для записи");
                stream = client.GetStream();

                while (true)
                {
                    SendCarPostId(stream);
                    break;
                }
            }
            catch (SocketException ex)
            {
                Log($"Ошибка подключения к серверу: {ex.Message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Log($"Ошибка передачи данных: {ex.Message}{Environment.NewLine}");
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
            }
        }

        private void DBTest()
        {
            try
            {
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = AutoTestPath;
                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Db\\AutoTest;Extended Properties=dBase IV;";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTest>(
                        $"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Log($"Ошибка чтения базы данных: {ex.Message}");
            }
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

        public void Log(string Message)
        {
            Action action = () => textBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> {Message}{Environment.NewLine}");
            textBoxLog.Invoke(action);
        }
    }
}

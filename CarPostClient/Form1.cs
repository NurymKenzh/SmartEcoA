using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private const string server = "185.125.44.116";
        private string CarPostId = null,
            AutoTestPath = null,
            SmokeMeterPath;
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
            while (!backgroundWorkerMain.CancellationPending)
            {
                // run updater
                if (DateTime.Now - dateTimeTryUpdate > new TimeSpan(0, 10, 0))
                {
                    try
                    {
                        Log($"Проверка обновления");
                        Process updater = new Process();
                        //updater.StartInfo.FileName = @"Updater\CarPostClientUpdater.exe";
                        updater.StartInfo.FileName = @"C:\Users\N\source\repos\NurymKenzh\SmartEcoA\CarPostClientUpdater\bin\Debug\netcoreapp3.1\CarPostClientUpdater.exe";
                        //updater.Start();
                    }
                    catch { }
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
                Log("Значения с appsettings.json прочитаны.");
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }

        private string GetProductVersion()
        {
            return Application.ProductVersion;
            //FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
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

        public void Log(string Message)
        {
            Action action = () => textBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> {Message}{Environment.NewLine}");
            textBoxLog.Invoke(action);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarPostClientUpdater
{
    public partial class FormMain : Form
    {
        private const string CarPostClientFileName = "CarPostClient.exe",
            NewVersionDirectory = "CarPostClientNew";
        private string CarPostClientDir = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Hide();
            try
            {
                ReadAppSettings();
                if (string.IsNullOrEmpty(CarPostClientDir))
                {
                    CarPostClientDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
                }

                FileStream waitFile = File.Create(Path.Combine(CarPostClientDir, "wait"));
                waitFile.Close();
                if (NeedUpdate())
                {
                    FileStream stopFile = File.Create(Path.Combine(CarPostClientDir, "stop"));
                    stopFile.Close();
                    File.Delete(Path.Combine(CarPostClientDir, "wait"));
                    Thread.Sleep(new TimeSpan(0, 0, 30));

                    UpdateCarPostClient();

                    // run new CarPostClient
                    File.Delete(Path.Combine(CarPostClientDir, "stop"));
                    Process client = new Process();
                    client.StartInfo.FileName = Path.Combine(CarPostClientDir, CarPostClientFileName);
                    client.Start();
                }
                if (File.Exists(Path.Combine(CarPostClientDir, "wait")))
                {
                    File.Delete(Path.Combine(CarPostClientDir, "wait"));
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Errors.txt"),
                    $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> Update error: {ex.Message}{Environment.NewLine}");
            }
            finally
            {
                if (File.Exists(Path.Combine(CarPostClientDir, "wait")))
                {
                    File.Delete(Path.Combine(CarPostClientDir, "wait"));
                }
                if (File.Exists(Path.Combine(CarPostClientDir, "stop")))
                {
                    File.Delete(Path.Combine(CarPostClientDir, "stop"));
                }
                Process client = new Process();
                client.StartInfo.FileName = Path.Combine(CarPostClientDir, CarPostClientFileName);
                client.Start();
            }

            Application.Exit();
        }

        private void ReadAppSettings()
        {
            try
            {
                string settingsS = File.ReadAllText("appsettings.json");
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
                        return;
                    }
                }
                foreach (var item in settingsJO)
                {
                    switch (item.Key)
                    {
                        case "CarPostClientDir":
                            CarPostClientDir = item.Value.ToString();
                            if (string.IsNullOrEmpty(CarPostClientDir))
                            {
                                CarPostClientDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private bool NeedUpdate()
        {
            bool need = false;
            try
            {
                FileVersionInfo versionCurrent = GetCurrentProductVersion();
                string versionNew = GetNewVersion();
                int cur1 = versionCurrent.ProductMajorPart,
                    cur2 = versionCurrent.ProductMinorPart,
                    cur3 = versionCurrent.ProductBuildPart,
                    new1 = Convert.ToInt32(versionNew.Split('.')[0]),
                    new2 = Convert.ToInt32(versionNew.Split('.')[1]),
                    new3 = Convert.ToInt32(versionNew.Split('.')[2]);
                if (new1 > cur1)
                {
                    need = true;
                }
                else if (new2 > cur2 && new1 == cur1)
                {
                    need = true;
                }
                else if (new3 > cur3 && new1 == cur1 && new2 == cur2)
                {
                    need = true;
                }
            }
            catch { }
            return need;
        }

        private FileVersionInfo GetCurrentProductVersion()
        {
            return FileVersionInfo.GetVersionInfo(Path.Combine(CarPostClientDir, CarPostClientFileName));
        }

        private string GetNewVersion()
        {
            WebClient webClient = new WebClient();
            string version = webClient.DownloadString("http://smarteco.kz:8086/assets/CarPostClientVersion.txt");
            return version;
        }

        private void UpdateCarPostClient()
        {
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory)))
            {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory), true);
            }
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory));
            WebClient webClient = new WebClient();
            webClient.DownloadFile("http://smarteco.kz:8086/assets/CarPostClient.zip", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory, "CarPostClient.zip"));

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory, "CarPostClient.zip"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory));
                foreach (string file in Directory.GetFiles(CarPostClientDir))
                {
                    if (Path.GetFileName(file) == "stop" || Path.GetFileName(file) == "appsettings.json")
                    {
                        continue;
                    }
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
                foreach (string file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewVersionDirectory)))
                {
                    if (Path.GetFileName(file) != "CarPostClient.zip")
                    {
                        File.Move(file, Path.Combine(CarPostClientDir, Path.GetFileName(file)), true);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Errors.txt"),
                        $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> Update error: {ex.Message}{Environment.NewLine}");
            }
        }

        private void StopCarPostClient()
        {
            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process process in runningProcesses)
            {
                try
                {
                    if (process.MainModule.ModuleName == CarPostClientFileName)
                    {
                        process.Kill();
                    }
                }
                catch { }
            }
        }
    }
}
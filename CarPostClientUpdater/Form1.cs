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
            ReadAppSettings();
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
            Application.Exit();
        }

        private void ReadAppSettings()
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
                                CarPostClientDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
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
            if (Directory.Exists(NewVersionDirectory))
            {
                Directory.Delete(NewVersionDirectory, true);
            }
            Directory.CreateDirectory(NewVersionDirectory);
            WebClient webClient = new WebClient();
            webClient.DownloadFile("http://smarteco.kz:8086/assets/CarPostClient.zip", Path.Combine(NewVersionDirectory, "CarPostClient.zip"));
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
            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(NewVersionDirectory, "CarPostClient.zip"), CarPostClientDir);
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
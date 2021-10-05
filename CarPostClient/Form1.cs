using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            e.Cancel = true;
            Hide();
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
            while (!backgroundWorkerMain.CancellationPending)
            {
                if (!ReadAppSettings())
                {
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }

        private bool ReadAppSettings()
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
            return true;
        }
    }
}

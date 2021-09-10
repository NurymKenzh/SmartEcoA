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

namespace Server
{
    public partial class FormMain : Form
    {
        const string PostsDataConnectionString = "Host=localhost;Database=PostsData;Username=postgres;Password=postgres;Port=5432;CommandTimeout=0;Keepalive=0;",
            SmartEcoAConnectionString = "Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433;CommandTimeout=0;Keepalive=0;",
            LastReceivedPostDataDateTimeString = "LastReceivedPostDataDateTime",
            LastPostDataDividedDateTimeString = "LastPostDataDividedDateTime",
            LastPostDataAveragedDateTimeString = "LastPostDataAveragedDateTime";

        public FormMain()
        {
            InitializeComponent();
        }

        private void backgroundWorkerPosts_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorkerPosts.CancellationPending)
            {
                PostDataReceiver postDataReceiver = new PostDataReceiver(
                    PostsDataConnectionString,
                    SmartEcoAConnectionString,
                    LastReceivedPostDataDateTimeString,
                    textBoxPostsData);
                postDataReceiver.GetPostDatas();

                PostDataDivider postDataDivider = new PostDataDivider(
                    SmartEcoAConnectionString,
                    LastPostDataDividedDateTimeString,
                    textBoxPostsData);
                postDataDivider.DividePostDatas();

                PostDataAverager postDataAverager = new PostDataAverager(
                    SmartEcoAConnectionString,
                    LastPostDataAveragedDateTimeString,
                    textBoxPostsData);
                bool sleep = postDataAverager.AveragePostDatas();
                if (sleep)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            backgroundWorkerPosts.RunWorkerAsync();
        }

        private void backgroundWorkerGetPostsData_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void backgroundWorkerDividePostDatas_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void backgroundWorkerAveragePostsData_DoWork(object sender, DoWorkEventArgs e)
        {
        }
    }
}

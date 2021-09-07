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
            LastPostDataDividedDateTimeString = "LastPostDataDividedDateTime";

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //backgroundWorkerGetPostsData.RunWorkerAsync();
            backgroundWorkerDividePostDatas.RunWorkerAsync();
        }

        private void backgroundWorkerGetPostsData_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorkerGetPostsData.CancellationPending)
            {
                PostDataReceiver postDataReceiver = new PostDataReceiver(
                    PostsDataConnectionString,
                    SmartEcoAConnectionString,
                    LastReceivedPostDataDateTimeString,
                    textBoxGetPostsData);
                postDataReceiver.GetPostDatas();
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }

        private void backgroundWorkerDividePostDatas_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorkerDividePostDatas.CancellationPending)
            {
                PostDataDivider postDataDivider = new PostDataDivider(
                    SmartEcoAConnectionString,
                    LastPostDataDividedDateTimeString,
                    textBoxDividePostsDatas);
                postDataDivider.DividePostDatas();
                Thread.Sleep(new TimeSpan(0, 10, 10));
            }
        }
    }
}

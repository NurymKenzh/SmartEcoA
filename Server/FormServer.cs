using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        const int portCarPosts = 8087;


        TcpListener listener;

        private Logger LoggerCarPosts;

        enum Working
        {
            Work = 1,
            Stoping = 2,
            Stop = 3
        }
        Working workingPosts = Working.Work,
            workingCarPosts = Working.Work;

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

        private void backgroundWorkerCarPosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (workingCarPosts == Working.Stoping)
            {
                labelCarPostsStartStop.Text = "Остановлено";
                workingCarPosts = Working.Stop;
            }
        }

        private void buttonCarPostsStartStop_Click(object sender, EventArgs e)
        {
            if (workingCarPosts == Working.Work)
            {
                //CarPostClient carPostClient = new CarPostClient(textBoxCarPosts);
                //listener.EndAcceptTcpClient(new AsyncCallback(carPostClient.Process));
                

                backgroundWorkerCarPosts.CancelAsync();

                listener.Stop();

                labelCarPostsStartStop.Text = "Останавливается...";
                buttonCarPostsStartStop.Text = "Запустить";
                workingCarPosts = Working.Stoping;


            }
            else if (workingCarPosts == Working.Stop)
            {
                backgroundWorkerCarPosts.RunWorkerAsync();
                labelCarPostsStartStop.Text = "Работает";
                buttonCarPostsStartStop.Text = "Остановить";
                workingCarPosts = Working.Work;
            }
        }

        private void buttonPostsStartStop_Click(object sender, EventArgs e)
        {
            if (workingPosts == Working.Work)
            {
                backgroundWorkerPosts.CancelAsync();
                labelPostsStartStop.Text = "Останавливается...";
                buttonPostsStartStop.Text = "Запустить";
                workingPosts = Working.Stoping;
            }
            else if (workingPosts == Working.Stop)
            {
                backgroundWorkerPosts.RunWorkerAsync();
                labelPostsStartStop.Text = "Работает";
                buttonPostsStartStop.Text = "Остановить";
                workingPosts = Working.Work;
            }
        }

        private void backgroundWorkerPosts_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (workingPosts == Working.Stoping)
            {
                labelPostsStartStop.Text = "Остановлено";
                workingPosts = Working.Stop;
            }
        }

        private void backgroundWorkerCarPosts_DoWork(object sender, DoWorkEventArgs e)
        {
            //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener = new TcpListener(IPAddress.Any, portCarPosts);
            //listener = new TcpListener(portCarPosts);
            //listener = new TcpListener(iPEndPoint);
            listener.Start();
            LoggerCarPosts.Log("Жду подключения...");
            while (!backgroundWorkerCarPosts.CancellationPending)
            {
                //try
                //{
                //    TcpClient tcpClient = listener.AcceptTcpClient();
                //    CarPostClient client = new CarPostClient(tcpClient, textBoxCarPosts);

                //    Thread clientThread = new Thread(new ThreadStart(client.Process));
                //    clientThread.Start();
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
                //finally
                //{
                //    if (listener != null)
                //        listener.Stop();
                //}
                //Thread.Sleep(new TimeSpan(0, 0, 1));
                
                CarPostClient carPostClient = new CarPostClient(textBoxCarPosts);
                listener.BeginAcceptTcpClient(new AsyncCallback(carPostClient.Process), listener);
                Thread.Sleep(new TimeSpan(0, 0, 1));
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            backgroundWorkerPosts.RunWorkerAsync();
            LoggerCarPosts = new Logger(this.textBoxCarPosts);
            backgroundWorkerCarPosts.RunWorkerAsync();
        }
    }
}

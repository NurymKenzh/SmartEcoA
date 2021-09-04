using System;
using System.Windows.Forms;

namespace Server
{
    public class Logger
    {
        private readonly TextBox TextBoxLog;

        public Logger(TextBox TextBoxLog)
        {
            this.TextBoxLog = TextBoxLog;
        }
        public void Log(string Message)
        {
            Action action = () => TextBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} >> {Message}{Environment.NewLine}");
            TextBoxLog.Invoke(action);
        }
    }
}
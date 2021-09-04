using Dapper;
using Microsoft.Extensions.Primitives;
using Npgsql;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public class PostDataReceiver
    {
        private readonly string PostsDataConnectionString;
        private readonly string SmartEcoAConnectionString;
        private readonly string LastReceivedPostDataDateTimeString;
        private readonly TextBox TextBoxLog;
        private readonly Logger Logger;

        public PostDataReceiver(string PostsDataConnectionString,
            string SmartEcoAConnectionString,
            string LastReceivedPostDataDateTimeString,
            TextBox TextBoxLog)
        {
            this.PostsDataConnectionString = PostsDataConnectionString;
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
            this.LastReceivedPostDataDateTimeString = LastReceivedPostDataDateTimeString;
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
        }
        public void GetPostDatas()
        {
            // get date, time
            DateTime lastReceivedPostDataDateTime = GetLastReceivedPostDataDateTime();
            // get data
            List<PostData> postDatas = GetPostDatas(lastReceivedPostDataDateTime);
            if (postDatas.Count() == 0)
            {
                return;
            }
            // put data
            PutPostDatas(postDatas);
            // save date, time
            DateTime dateTimeMax = postDatas.Max(p => p.DateTime);
            SaveLastReceivedPostDataDateTime(dateTimeMax);
            // log
            Logger.Log($"Получено {postDatas.Count()} данных с постов с {lastReceivedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")} по {dateTimeMax.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        private DateTime GetLastReceivedPostDataDateTime()
        {
            DateTime? lastReceivedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                string lastAveragedPostDataDateTimeS = connection.Query<string>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastReceivedPostDataDateTimeString}' LIMIT 1;").FirstOrDefault();
                if (string.IsNullOrEmpty(lastAveragedPostDataDateTimeS))
                {
                    lastReceivedPostDataDateTime = new DateTime(2019, 5, 1);
                }
                else
                {
                    lastReceivedPostDataDateTime = new DateTime(
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(0, 4)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(5, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(8, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(11, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(14, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(17, 2)));
                }
                connection.Close();
            }
            return lastReceivedPostDataDateTime.Value;
        }

        private List<PostData> GetPostDatas(DateTime DateTimeFrom)
        {
            List<PostData> postDatas = new List<PostData>();
            using (var connection = new NpgsqlConnection(PostsDataConnectionString))
            {
                connection.Open();
                postDatas = connection.Query<PostData>(
                    $"SELECT \"Id\", \"DateTimeServer\" AS \"DateTime\", \"IP\", \"Data\"" +
                    $" FROM public.\"Data\"" +
                    $" WHERE \"DateTimeServer\" > '{DateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $" ORDER BY \"DateTimeServer\";").ToList();
                connection.Close();
            }
            return postDatas;
        }

        private void PutPostDatas(List<PostData> PostDatas)
        {
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                StringBuilder insert = new StringBuilder($"INSERT INTO public.\"PostData\"(\"DateTime\", \"IP\", \"Data\") VALUES");
                int currentCount = 0;
                for (int i = 0; i < PostDatas.Count; i++)
                {
                    if (currentCount > 1000000 || i == PostDatas.Count - 1)
                    {
                        insert.Remove(insert.Length - 1, 1);
                        connection.Execute(insert.ToString());
                        insert = new StringBuilder($"INSERT INTO public.\"PostData\"(\"DateTime\", \"IP\", \"Data\") VALUES");
                        currentCount = 0;
                    }
                    insert.Append($"('{PostDatas[i].DateTime.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $" '{PostDatas[i].IP}'," +
                        $" '{PostDatas[i].Data.Replace(Environment.NewLine, "")}'),");
                    currentCount++;
                }
                connection.Close();
            }
        }

        private void SaveLastReceivedPostDataDateTime(DateTime DateTimeTo)
        {
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                int lastDividedPostDataDateTimeCount = connection.Query<int>($"SELECT COUNT(*) FROM public.\"Stat\" WHERE \"Name\" = '{LastReceivedPostDataDateTimeString}';").FirstOrDefault();
                if (lastDividedPostDataDateTimeCount == 0)
                {
                    connection.Execute($"INSERT INTO public.\"Stat\"(\"Name\", \"Value\")" +
                        $" VALUES ('{LastReceivedPostDataDateTimeString}', '{DateTimeTo.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    connection.Execute($"UPDATE public.\"Stat\"" +
                        $" SET \"Value\" = '{DateTimeTo.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                        $" WHERE \"Name\" = '{LastReceivedPostDataDateTimeString}';");
                }
                connection.Close();
            }
        }
    }
}
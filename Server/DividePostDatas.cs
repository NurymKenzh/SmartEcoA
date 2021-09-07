using Dapper;
using Npgsql;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public class PostDataDivider
    {
        private readonly string SmartEcoAConnectionString;
        private readonly string LastPostDataDividedDateTimeString;
        private readonly TextBox TextBoxLog;
        private readonly Logger Logger;

        public PostDataDivider(string SmartEcoAConnectionString,
            string LastPostDataDividedDateTimeString,
            TextBox TextBoxLog)
        {
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
            this.LastPostDataDividedDateTimeString = LastPostDataDividedDateTimeString;
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
        }

        public void DividePostDatas()
        {
            // get date
            DateTime lastDividedPostDataDateTime = GetLastDividedPostDataDateTime();
            // select post data
            List<PostData> postDatas = SelectPostDatas(lastDividedPostDataDateTime);
            if (postDatas.Count() == 0)
            {
                return;
            }
            // divide post data
            List<PostDataDivided> postDataDivideds = GetPostDatasDivided(postDatas);
            // save divided post data
            SavePostDatasDivided(postDataDivideds);
            // save date
            DateTime dateTimeMax = postDatas.Max(p => p.DateTime);
            SaveLastDividedPostDataDateTime(dateTimeMax);
            // log
            Logger.Log($"Разделено {postDatas.Count().ToString("N0")} данных с постов с {lastDividedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")} по {dateTimeMax.ToString("yyyy-MM-dd HH:mm:ss")}." +
                $" Получено {postDataDivideds.Count().ToString("N0")} разделённых данных.");
        }

        private DateTime GetLastDividedPostDataDateTime()
        {
            DateTime? lastDividedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                string lastDividedPostDataDateTimeS = connection.Query<string>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}' LIMIT 1;").FirstOrDefault();
                if (string.IsNullOrEmpty(lastDividedPostDataDateTimeS))
                {
                    lastDividedPostDataDateTime = new DateTime(2019, 5, 1);
                }
                else
                {
                    lastDividedPostDataDateTime = new DateTime(
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(0, 4)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(5, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(8, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(11, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(14, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(17, 2)));
                }
                connection.Close();
            }
            return lastDividedPostDataDateTime.Value;
        }

        private List<PostData> SelectPostDatas(DateTime LastDividedPostDataDateTime)
        {
            List<PostData> postDatas = new List<PostData>();
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                postDatas = connection.Query<PostData>(
                    $"SELECT \"Id\", \"DateTime\", \"IP\", \"Data\"" +
                    $" FROM public.\"PostData\"" +
                    $" WHERE \"DateTime\" > '{LastDividedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}';")
                    .ToList();
                connection.Close();
            }
            return postDatas;
        }

        private List<PostDataDivided> GetPostDatasDivided(List<PostData> PostDatas)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            foreach (PostData postData in PostDatas)
            {
                postDataDivideds.AddRange(GetPostDatasDivided(postData));
            }
            return postDataDivideds;
        }

        private void SavePostDatasDivided(List<PostDataDivided> PostDataDivideds)
        {
            if (PostDataDivideds.Count() == 0)
            {
                return;
            }
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                string firstLine = $"INSERT INTO public.\"PostDataDivided\"(\"PostDataId\", \"MN\", \"OceanusCode\", \"Value\") VALUES";
                StringBuilder insert = new StringBuilder(firstLine);
                int currentCount = 0;
                for (int i = 0; i < PostDataDivideds.Count; i++)
                {
                    if (currentCount > 1000000 || i == PostDataDivideds.Count - 1)
                    {
                        insert.Remove(insert.Length - 1, 1);
                        connection.Execute(insert.ToString());
                        insert = new StringBuilder(firstLine);
                        currentCount = 0;
                    }
                    insert.Append($"({PostDataDivideds[i].PostDataId}," +
                        $" '{PostDataDivideds[i].MN}'," +
                        $" '{PostDataDivideds[i].OceanusCode}'," +
                        $" {PostDataDivideds[i].Value}),");
                    currentCount++;
                }
                connection.Close();
            }
        }

        private void SaveLastDividedPostDataDateTime(DateTime LastDividedPostDataDateTime)
        {
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                int lastDividedPostDataDateTimeCount = connection.Query<int>($"SELECT COUNT(*) FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}';").FirstOrDefault();
                if (lastDividedPostDataDateTimeCount == 0)
                {
                    connection.Execute($"INSERT INTO public.\"Stat\"(\"Name\", \"Value\")" +
                        $" VALUES ('{LastPostDataDividedDateTimeString}', '{LastDividedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    connection.Execute($"UPDATE public.\"Stat\"" +
                        $" SET \"Value\" = '{LastDividedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                        $" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}';");
                }
                connection.Close();
            }
        }

        private List<PostDataDivided> GetPostDatasDivided(PostData PostData)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            string MN = GetMN(PostData.Data);
            if (string.IsNullOrEmpty(MN))
            {
                return postDataDivideds;
            }
            postDataDivideds = GetPostDataDividedValues(PostData.Data);
            for (int i = 0; i < postDataDivideds.Count(); i++)
            {
                postDataDivideds[i].MN = MN;
                postDataDivideds[i].PostDataId = PostData.Id;
            }
            return postDataDivideds;
        }

        private string GetMN(string Data)
        {
            string MN = null;
            if (Data.Contains("MN="))
            {
                string[] DataA = Data.Split(';');
                string MNS = DataA.Where(d => d.Contains("MN=")).FirstOrDefault();
                int MNStartIndex = MNS.IndexOf("MN=") + 3;
                MN = MNS.Substring(MNStartIndex);
            }
            return MN;
        }

        private List<PostDataDivided> GetPostDataDividedValues(string Data)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            string[] DataA = Data.Split(';');
            foreach (string data in DataA)
            {
                if (data.Contains("-Rtd="))
                {
                    string oceanusCode = GetOceanusCode(data);
                    decimal? value = GetValue(data);
                    if (!string.IsNullOrEmpty(oceanusCode) && value != null)
                    {
                        postDataDivideds.Add(new PostDataDivided()
                        {
                            OceanusCode = oceanusCode,
                            Value = (decimal)value
                        });
                    }
                }
            }
            return postDataDivideds;
        }

        private string GetOceanusCode(string DataSplitted)
        {
            try
            {
                int aIndex = DataSplitted.IndexOf('a'),
                RtdIndex = DataSplitted.IndexOf("-Rtd=");
                return DataSplitted.Substring(aIndex, RtdIndex - aIndex);
            }
            catch
            {
                return null;
            }
        }

        private decimal? GetValue(string DataSplitted)
        {
            try
            {
                string value = "";
                int RtdIndex = DataSplitted.IndexOf("-Rtd=") + 5;
                for (int i = RtdIndex; i < DataSplitted.Length; i++)
                {
                    if (Char.IsDigit(DataSplitted[i]) || DataSplitted[i] == '.')
                    {
                        value += DataSplitted[i];
                    }
                    else
                    {
                        break;
                    }
                }
                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }
    }
}
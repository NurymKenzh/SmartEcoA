using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PostDataCalc
{
    class Program
    {
        const string ConnectionString = "Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433",
            LastPostDataDateTimeString = "LastPostDataDateTime";

        class PostData
        {
            public long Id { get; set; }
            public DateTime DateTime { get; set; }
            public string IP { get; set; }
            public string Data { get; set; }
        }

        public class PostDataDivided
        {
            public long Id { get; set; }
            public long PostDataId { get; set; }
            public string MN { get; set; }
            public string OceanusCode { get; set; }
            public decimal Value { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC to stop!");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                DividePostDatas();
            }
        }

        static void DividePostDatas()
        {
            // get date
            DateTime? lastPostDataDateTime = GetLastPostDataDateTime();
            if (lastPostDataDateTime == null)
            {
                return;
            }
            // select post data
            List<PostData> postDatas = SelectPostDatas(lastPostDataDateTime.Value);
            if (postDatas.Count() == 0 && (DateTime.Today - lastPostDataDateTime.Value).TotalDays > 10)
            {
                lastPostDataDateTime = lastPostDataDateTime.Value.AddDays(1);
            }
            else if (postDatas.Count() == 0 && (DateTime.Now - lastPostDataDateTime.Value).TotalHours > 72)
            {
                return;
            }
            else
            {
                lastPostDataDateTime = postDatas.Max(p => p.DateTime);
            }    
            // divide post data
            List<PostDataDivided> postDataDivideds = GetPostDataDivideds(postDatas);
            // save divided post data
            SavePostDataDivideds(postDataDivideds);
            // save date
            SaveLastPostDataDateTime(lastPostDataDateTime.Value);
            Console.Clear();
            Console.WriteLine("Press ESC to stop!");
            Log($"Last post data date time: {lastPostDataDateTime}");
        }

        static void Log(string Message)
        {
            Console.WriteLine($"{DateTime.Now} >> {Message}");
        }

        static void SaveLastPostDataDateTime(DateTime LastPostDataDateTime)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                int lastDividedPostDataDateTimeCount = connection.Query<int>($"SELECT COUNT(*) FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDateTimeString}';").FirstOrDefault();
                if (lastDividedPostDataDateTimeCount == 0)
                {
                    connection.Execute($"INSERT INTO public.\"Stat\"(\"Name\", \"Value\")" +
                        $" VALUES ('{LastPostDataDateTimeString}', '{LastPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    connection.Execute($"UPDATE public.\"Stat\"" +
                        $" SET \"Value\" = '{LastPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                        $" WHERE \"Name\" = '{LastPostDataDateTimeString}';");
                }
                connection.Close();
            }
        }

        static void SavePostDataDivideds(List<PostDataDivided> PostDataDivideds)
        {
            if (PostDataDivideds.Count() == 0)
            {
                return;
            }
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string insert = $"INSERT INTO public.\"PostDataDivided\"(\"PostDataId\", \"MN\", \"OceanusCode\", \"Value\") VALUES";
                foreach (PostDataDivided postDataDivided in PostDataDivideds)
                {
                    insert += $"({postDataDivided.PostDataId}," +
                        $" '{postDataDivided.MN}'," +
                        $" '{postDataDivided.OceanusCode}'," +
                        $" {postDataDivided.Value}),";
                }
                insert = insert.Remove(insert.Length - 1, 1) + ";";
                connection.Execute(insert);
                connection.Close();
            }
        }

        static List<PostDataDivided> GetPostDataDivideds(List<PostData> PostDatas)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            foreach (PostData postData in PostDatas)
            {
                postDataDivideds.AddRange(GetPostDataDivideds(postData));
            }
            return postDataDivideds;
        }

        static List<PostDataDivided> GetPostDataDivideds(PostData PostData)
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

        static List<PostDataDivided> GetPostDataDividedValues(string Data)
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
                            Value = (decimal) value
                        });
                    }
                }
            }
            return postDataDivideds;
        }

        static string GetOceanusCode(string DataSplitted)
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

        static decimal? GetValue(string DataSplitted)
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

        static string GetMN(string Data)
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

        static List<PostData> SelectPostDatas(DateTime LastPostDataDateTime)
        {
            List<PostData> postDatas = new List<PostData>();
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                postDatas = connection.Query<PostData>(
                    $"SELECT \"Id\", \"DateTime\", \"IP\", \"Data\"" +
                    $" FROM public.\"PostData\"" +
                    $" WHERE \"DateTime\" > '{LastPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $" AND \"DateTime\" < '{LastPostDataDateTime.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}';")
                    .ToList();
                connection.Close();
            }
            return postDatas;
        }

        static DateTime? GetLastPostDataDateTime()
        {
            DateTime? lastDividedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string lastDividedPostDataDateTimeS = connection.Query<string>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDateTimeString}' LIMIT 1;").FirstOrDefault();
                if (string.IsNullOrEmpty(lastDividedPostDataDateTimeS))
                {
                    lastDividedPostDataDateTime = connection.Query<DateTime?>($"SELECT MIN(\"DateTime\") FROM public.\"PostData\";").FirstOrDefault().Value.AddSeconds(-1);
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
            return lastDividedPostDataDateTime;
        }
    }
}

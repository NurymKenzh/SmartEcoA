// in DB Stat table change LastPostDataDateTime = 2020-11-30 23:59:59
// in DB Stat table change LastPostDataDividedDateTime = 2020-11-30 00:00:00

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
            LastPostDataDateTimeString = "LastPostDataDateTime",
            LastPostDataDividedDateTimeString = "LastPostDataDividedDateTime";

        public class PostData
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

        public class PostDataAvg
        {
            public long Id { get; set; }
            public DateTime DateTime { get; set; }
            public decimal Value { get; set; }
            public int MeasuredParameterId { get; set; }
            public int PostId { get; set; }
        }

        public class MeasuredParameter
        {
            public int Id { get; set; }

            public string OceanusCode { get; set; }

            public decimal OceanusCoefficient { get; set; }
        }

        public class Post
        {
            public int Id { get; set; }
            public string MN { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC to stop!");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                DividePostDatas();
                AverageDividedPostDatas();
            }
        }

        static void AverageDividedPostDatas()
        {
            // get date
            DateTime? lastPostDataDividedDateTime = GetLastPostDataDividedDateTime()?.AddSeconds(1);
            if (lastPostDataDividedDateTime == null)
            {
                return;
            }
            // get Measured Parameters
            MeasuredParameter[] measuredParameters = GetMeasuredParameters();
            // get Posts
            Post[] posts = GetPosts();
            // select post data divided
            List<PostDataDivided> postDataDivideds = SelectPostDataDivideds(lastPostDataDividedDateTime.Value);
            if (postDataDivideds.Count() == 0 && (DateTime.Today - lastPostDataDividedDateTime.Value).TotalDays > 10)
            {
                lastPostDataDividedDateTime = GetStartDateTime20(lastPostDataDividedDateTime.Value.AddMinutes(20));
            }
            else if (postDataDivideds.Count() == 0/* && (DateTime.Now - lastPostDataDateTime.Value).TotalHours > 72*/)
            {
                return;
            }
            else
            {
                lastPostDataDividedDateTime = GetFinishDateTime20(lastPostDataDividedDateTime.Value);
            }
            // average divided post data
            List<PostDataAvg> postDataAvgs = GetPostDataAvgs(
                postDataDivideds,
                lastPostDataDividedDateTime.Value,
                measuredParameters,
                posts);
            // save averaged post data
            SavePostDataAvgs(postDataAvgs);
            // save date
            SaveLastPostDataDividedDateTime(lastPostDataDividedDateTime.Value);
            Log($"Last post data divided date time: {lastPostDataDividedDateTime}");
        }

        static void SaveLastPostDataDividedDateTime(DateTime lastPostDataDividedDateTime)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                int lastDividedPostDataDateTimeCount = connection.Query<int>($"SELECT COUNT(*) FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}';").FirstOrDefault();
                if (lastDividedPostDataDateTimeCount == 0)
                {
                    connection.Execute($"INSERT INTO public.\"Stat\"(\"Name\", \"Value\")" +
                        $" VALUES ('{LastPostDataDividedDateTimeString}', '{lastPostDataDividedDateTime.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    connection.Execute($"UPDATE public.\"Stat\"" +
                        $" SET \"Value\" = '{lastPostDataDividedDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                        $" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}';");
                }
                connection.Close();
            }
        }

        static void SavePostDataAvgs(List<PostDataAvg> PostDataAvgs)
        {
            if (PostDataAvgs.Count() == 0)
            {
                return;
            }
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string insert = $"INSERT INTO public.\"PostDataAvg\"(\"DateTime\", \"Value\", \"MeasuredParameterId\", \"PostId\") VALUES";
                foreach (PostDataAvg postDataAvg in PostDataAvgs)
                {
                    insert += $"('{postDataAvg.DateTime.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $" {postDataAvg.Value}," +
                        $" {postDataAvg.MeasuredParameterId}," +
                        $" {postDataAvg.PostId}),";
                }
                insert = insert.Remove(insert.Length - 1, 1) + ";";
                connection.Execute(insert);
                connection.Close();
            }
        }

        static List<PostDataAvg> GetPostDataAvgs(
            List<PostDataDivided> PostDataDivideds,
            DateTime DateTime,
            MeasuredParameter[] MeasuredParameters,
            Post[] Posts)
        {
            List<PostDataAvg> postDataAvgs = new List<PostDataAvg>();
            List<string> mNs = PostDataDivideds.Select(p => p.MN).Distinct().ToList(),
                oceanusCodes = PostDataDivideds.Select(p => p.OceanusCode).Distinct().ToList();
            foreach(string mn in mNs)
            {
                foreach(string oceanusCode in oceanusCodes)
                {
                    int? measuredParameterId = MeasuredParameters.FirstOrDefault(m => m.OceanusCode == oceanusCode)?.Id,
                        postId = Posts.FirstOrDefault(p => p.MN == mn)?.Id;
                    int count = PostDataDivideds.Count(p => p.OceanusCode == oceanusCode && p.MN == mn);
                    if (measuredParameterId != null && postId != null && count > 0)
                    {
                        postDataAvgs.Add(new PostDataAvg()
                        {
                            DateTime = DateTime,
                            MeasuredParameterId = measuredParameterId.Value,
                            PostId = postId.Value,
                            Value = PostDataDivideds.Where(p => p.OceanusCode == oceanusCode && p.MN == mn).Average(p => p.Value)
                        });
                    }
                }
            }
            return postDataAvgs;
        }

        static List<PostDataDivided> SelectPostDataDivideds(DateTime LastPostDataDateTime)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                postDataDivideds = connection.Query<PostDataDivided>(
                    $"SELECT \"Id\", \"PostDataId\", \"MN\", \"OceanusCode\", \"Value\"" +
                    $" FROM public.\"PostDataDivided\"" +
                    $" WHERE \"PostDataId\" IN" +
                    $" (SELECT \"Id\"" +
                    $" FROM public.\"PostData\"" +
                    $" WHERE \"DateTime\" > '{GetStartDateTime20(LastPostDataDateTime).ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $" AND \"DateTime\" <= '{GetFinishDateTime20(LastPostDataDateTime).ToString("yyyy-MM-dd HH:mm:ss")}');")
                    .ToList();
                connection.Close();
            }
            return postDataDivideds;
        }

        static DateTime GetStartDateTime20(DateTime DateTime)
        {
            DateTime dateTime = DateTime;
            if (DateTime.Minute < 20)
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 0, 0);
            }
            else if (DateTime.Minute < 40)
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 20, 0);
            }
            else
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 40, 0);
            }
            return dateTime;
        }

        static DateTime GetFinishDateTime20(DateTime DateTime)
        {
            DateTime dateTime = DateTime;
            if (DateTime.Minute >= 40)
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 59, 59).AddSeconds(1);
            }
            else if (DateTime.Minute >= 20)
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 39, 59).AddSeconds(1);
            }
            else
            {
                dateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 19, 59).AddSeconds(1);
            }
            return dateTime;
        }

        static Post[] GetPosts()
        {
            Post[] posts = new Post[0];
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                posts = connection.Query<Post>("SELECT \"Id\", \"MN\" FROM public.\"Post\" WHERE \"MN\" <> '';").ToArray();
                connection.Close();
            }
            return posts.ToArray();
        }

        static MeasuredParameter[] GetMeasuredParameters()
        {
            MeasuredParameter[] measuredParameters = new MeasuredParameter[0];
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                measuredParameters = connection.Query<MeasuredParameter>("SELECT \"Id\", \"OceanusCode\", \"OceanusCoefficient\" FROM public.\"MeasuredParameter\";").ToArray();
                connection.Close();
            }
            return measuredParameters.ToArray();
        }

        static DateTime? GetLastPostDataDividedDateTime()
        {
            DateTime? lastAveragedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string lastAveragedPostDataDateTimeS = connection.Query<string>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataDividedDateTimeString}' LIMIT 1;").FirstOrDefault();
                if (string.IsNullOrEmpty(lastAveragedPostDataDateTimeS))
                {
                    lastAveragedPostDataDateTime = connection.Query<DateTime?>($"SELECT MIN(\"DateTime\") FROM public.\"PostData\";").FirstOrDefault().Value;
                }
                else
                {
                    lastAveragedPostDataDateTime = new DateTime(
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(0, 4)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(5, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(8, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(11, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(14, 2)),
                        Convert.ToInt32(lastAveragedPostDataDateTimeS.Substring(17, 2)));
                }
                connection.Close();
            }
            return lastAveragedPostDataDateTime;
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
            else if (postDatas.Count() == 0/* && (DateTime.Now - lastPostDataDateTime.Value).TotalHours > 72*/)
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

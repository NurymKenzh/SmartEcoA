using Dapper;
using Npgsql;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Server
{
    public class PostDataAverager
    {
        private readonly string SmartEcoAConnectionString;
        private readonly string LastPostDataAveragedDateTimeString;
        private readonly TextBox TextBoxLog;
        private readonly Logger Logger;

        public PostDataAverager(string SmartEcoAConnectionString,
            string LastPostDataAveragedDateTimeString,
            TextBox TextBoxLog)
        {
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
            this.LastPostDataAveragedDateTimeString = LastPostDataAveragedDateTimeString;
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
        }

        public bool AveragePostDatas()
        {
            // get date
            DateTime lastAveragedPostDataDateTime = GetLastAveragedPostDataDateTime(),
                dateTimeFinish = GetFinishDateTime20(lastAveragedPostDataDateTime);
            // it's not time
            if (dateTimeFinish == GetFinishDateTime20(DateTime.Now))
            {
                return true;
            }
            // get Measured Parameters
            MeasuredParameter[] measuredParameters = GetMeasuredParameters();
            // get Posts
            Post[] posts = GetPosts();
            // select post data divided
            List<PostDataDivided> postDataDivideds = SelectPostDataDivideds(lastAveragedPostDataDateTime, dateTimeFinish);
            bool noNewData = NoNewPostDataDivideds(dateTimeFinish);
            // finished all
            if (postDataDivideds.Count() == 0 && noNewData)
            {
                return true;
            }
            // period with no data
            else if (postDataDivideds.Count() == 0 && !noNewData)
            {
                // save date
                SaveLastAveragedPostDataDateTime(dateTimeFinish);
                return false;
            }
            // average divided post data and save
            int countAvgs = GetAndSavePostDataAvgs(
                dateTimeFinish,
                postDataDivideds,
                measuredParameters,
                posts);
            // save date
            SaveLastAveragedPostDataDateTime(dateTimeFinish);
            // log
            Logger.Log($"Усреднено {postDataDivideds.Count().ToString("N0")} разделённых данных с постов с {lastAveragedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")} по {dateTimeFinish.ToString("yyyy-MM-dd HH:mm:ss")}." +
                $" Получено {countAvgs.ToString("N0")} усреднённых данных.");
            return false;
        }

        private DateTime GetLastAveragedPostDataDateTime()
        {
            DateTime? lastAveragedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                string lastDividedPostDataDateTimeS = connection.Query<string>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataAveragedDateTimeString}' LIMIT 1;").FirstOrDefault();
                if (string.IsNullOrEmpty(lastDividedPostDataDateTimeS))
                {
                    lastAveragedPostDataDateTime = new DateTime(2019, 5, 1);
                }
                else
                {
                    lastAveragedPostDataDateTime = new DateTime(
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(0, 4)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(5, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(8, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(11, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(14, 2)),
                        Convert.ToInt32(lastDividedPostDataDateTimeS.Substring(17, 2)));
                }
                connection.Close();
            }
            return lastAveragedPostDataDateTime.Value;
        }

        private MeasuredParameter[] GetMeasuredParameters()
        {
            MeasuredParameter[] measuredParameters = new MeasuredParameter[0];
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                measuredParameters = connection.Query<MeasuredParameter>("SELECT \"Id\", \"OceanusCode\", \"OceanusCoefficient\" FROM public.\"MeasuredParameter\";").ToArray();
                connection.Close();
            }
            return measuredParameters;
        }

        private Post[] GetPosts()
        {
            Post[] posts = new Post[0];
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                posts = connection.Query<Post>("SELECT \"Id\", \"MN\" FROM public.\"Post\" WHERE \"MN\" <> '';").ToArray();
                connection.Close();
            }
            return posts;
        }

        private List<PostDataDivided> SelectPostDataDivideds(DateTime LastPostDataAveragedDateTime,
            DateTime DateTimeFinish)
        {
            List<PostDataDivided> postDataDivideds = new List<PostDataDivided>();
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                postDataDivideds = connection.Query<PostDataDivided>(
                    $"SELECT \"Id\", \"PostDataId\", \"MN\", \"OceanusCode\", \"Value\", \"PostDataAvgId\"" +
                    $" FROM public.\"PostDataDivided\"" +
                    $" WHERE \"PostDataId\" IN" +
                    $" (SELECT \"Id\"" +
                    $" FROM public.\"PostData\"" +
                    $" WHERE \"DateTime\" > '{LastPostDataAveragedDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $" AND \"DateTime\" <= '{DateTimeFinish.ToString("yyyy-MM-dd HH:mm:ss")}');")
                    .ToList();
                connection.Close();
            }
            return postDataDivideds;
        }

        private bool NoNewPostDataDivideds(DateTime DateTimeFinish)
        {
            long? count = null;
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                count = connection.Query<long?>(
                    $"SELECT \"Id\"" +
                    $" FROM public.\"PostData\"" +
                    $" WHERE \"DateTime\" > '{DateTimeFinish.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                    $" LIMIT 1;")
                    .FirstOrDefault();
                connection.Close();
            }
            if (count == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private DateTime GetFinishDateTime20(DateTime DateTime)
        {
            if (DateTime.Minute >= 40)
            {
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 59, 59).AddSeconds(1);
            }
            else if (DateTime.Minute >= 20)
            {
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 39, 59).AddSeconds(1);
            }
            else
            {
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, 19, 59).AddSeconds(1);
            }
            return DateTime;
        }

        private void SaveLastAveragedPostDataDateTime(DateTime LastAveragedPostDataDateTime)
        {
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                int lastDividedPostDataDateTimeCount = connection.Query<int>($"SELECT COUNT(*) FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataAveragedDateTimeString}';").FirstOrDefault();
                if (lastDividedPostDataDateTimeCount == 0)
                {
                    connection.Execute($"INSERT INTO public.\"Stat\"(\"Name\", \"Value\")" +
                        $" VALUES ('{LastPostDataAveragedDateTimeString}', '{LastAveragedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                else
                {
                    connection.Execute($"UPDATE public.\"Stat\"" +
                        $" SET \"Value\" = '{LastAveragedPostDataDateTime.ToString("yyyy-MM-dd HH:mm:ss")}'" +
                        $" WHERE \"Name\" = '{LastPostDataAveragedDateTimeString}';");
                }
                connection.Close();
            }
        }

        private int GetAndSavePostDataAvgs(
            DateTime DateTimeFinish,
            List<PostDataDivided> PostDataDivideds,
            MeasuredParameter[] MeasuredParameters,
            Post[] Posts)
        {
            int count = 0;
            List<string> mNs = PostDataDivideds.Select(p => p.MN).Distinct().ToList(),
                oceanusCodes = PostDataDivideds.Select(p => p.OceanusCode).Distinct().ToList();
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
            {
                connection.Open();
                foreach (string mn in mNs)
                {
                    foreach (string oceanusCode in oceanusCodes)
                    {
                        MeasuredParameter measuredParameter = MeasuredParameters.FirstOrDefault(m => m.OceanusCode == oceanusCode);
                        int? measuredParameterId = measuredParameter?.Id,
                            postId = Posts.FirstOrDefault(p => p.MN == mn)?.Id;
                        List<PostDataDivided> postDataDividedsCurrent = PostDataDivideds.Where(p => p.OceanusCode == oceanusCode && p.MN == mn).ToList();
                        if (measuredParameterId != null && postId != null && postDataDividedsCurrent.Count() > 0)
                        {
                            string insert = $"INSERT INTO public.\"PostDataAvg\"(\"DateTime\", \"Value\", \"MeasuredParameterId\", \"PostId\")" +
                                $" VALUES ('{DateTimeFinish.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                                $" {PostDataDivideds.Where(p => p.OceanusCode == oceanusCode && p.MN == mn).Average(p => p.Value) * measuredParameter.OceanusCoefficient}," +
                                $" {measuredParameterId.Value}," +
                                $" {postId.Value}) RETURNING \"Id\";";
                            long idAvg = connection.Query<long>(insert).FirstOrDefault();
                            string update = $"UPDATE public.\"PostDataDivided\"" +
                                $" SET \"PostDataAvgId\"={idAvg}" +
                                $" WHERE \"Id\" IN ({string.Join(", ", postDataDividedsCurrent.Select(p => p.Id))});";
                            connection.Execute(update);
                            count++;
                        }
                    }
                }
                connection.Close();
            }
            if(count == 0)
            {
                int h = 0;
            }
            return count;
        }
    }
}
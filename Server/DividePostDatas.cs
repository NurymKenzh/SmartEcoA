using Dapper;
using Npgsql;
using System;
using System.Linq;

namespace Server
{
    public class PostDataDivider
    {
        private readonly string SmartEcoAConnectionString;
        private readonly string LastPostDataDateTimeString;

        public PostDataDivider(string SmartEcoConnectionString,
            string LastPostDataDateTimeString)
        {
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
            this.LastPostDataDateTimeString = LastPostDataDateTimeString;
        }

        public void DividePostDatas()
        {
            //// get date
            //DateTime? lastPostDataDateTime = GetLastPostDataDateTime();
            //if (lastPostDataDateTime == null)
            //{
            //    return;
            //}
        }

        public DateTime? GetLastPostDataDateTime()
        {
            DateTime? lastDividedPostDataDateTime = null;
            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
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
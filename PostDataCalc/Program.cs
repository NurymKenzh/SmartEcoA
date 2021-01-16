using Dapper;
using Npgsql;
using System;
using System.Linq;

namespace PostDataCalc
{
    class Program
    {
        const string ConnectionString = "Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433",
            LastPostDataId = "LastPostDataId";

        class PostData
        {
            public long Id { get; set; }
            public DateTime DateTime { get; set; }
            public string IP { get; set; }
            public string Data { get; set; }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                SelectPostDatas();
            }
        }

        static void SelectPostDatas()
        {
            decimal lastPostDataId = GetLastPostDataId();

        }

        static decimal GetLastPostDataId()
        {
            decimal? lastDividedPostDataId = 0;
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                lastDividedPostDataId = connection.Query<decimal?>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataId}' LIMIT 1;").FirstOrDefault();
                // если null, то минимальный Id достать
                // ...
                connection.Close();
            }
            return lastDividedPostDataId ?? 0;
        }

        static PostData GetPostDataById(long Id)
        {
            PostData postData;
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                postData = connection.Query<PostData>($"SELECT \"Value\" FROM public.\"Stat\" WHERE \"Name\" = '{LastPostDataId}' LIMIT 1;").FirstOrDefault();
                connection.Close();
            }
            return postData;
        }
    }
}

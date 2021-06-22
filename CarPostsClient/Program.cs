using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CarPostsClient.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using NDbfReader;
using Newtonsoft.Json;

namespace CarPostsClient
{
    class Program
    {
        private const int port = 8089;
        private const string server = "185.125.44.116";
        //private const string server = "127.0.0.1";
        public static IConfigurationRoot _config;

        static void Main(string[] args)
        {
            Console.WriteLine("Program started!");
            ConfigureServices();
            TcpClient client = null;
            NetworkStream stream = null;
            while (true)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(server, port);
                    stream = client.GetStream();

                    while (true)
                    {
                        //Sending CarPostId to Server
                        SendCarPostId(stream);

                        //Wait response from Server
                        dynamic obj = new ExpandoObject();
                        byte[] dataResponse = new byte[256];
                        while (true)
                        {
                            StringBuilder builder = new StringBuilder();
                            int bytes = 0;
                            do
                            {
                                bytes = stream.Read(dataResponse, 0, dataResponse.Length);
                                builder.Append(Encoding.Unicode.GetString(dataResponse, 0, bytes));
                            }
                            while (stream.DataAvailable);

                            if (builder.Length != 0)
                            {
                                string jsonString = builder.ToString();
                                obj = JsonConvert.DeserializeObject(jsonString);
                                break;
                            }
                        }
                        //Check error CarPostId
                        if (!String.IsNullOrEmpty((string)obj.Error))
                        {
                            throw new Exception((string)obj.Error);
                        }

                        JsonData jsonData = new JsonData();
                        jsonData.carModelSmokeMeters = CreateModelSmokeMeter((string)obj.carModelSmokeMeterName);
                        jsonData.carPostDataSmokeMeters = CreateDataSmokeMeter((DateTime?)obj.carPostDataSmokeMeterDate);
                        jsonData.carModelAutoTests = CreateModelAutoTest((string)obj.carModelAutoTestName);
                        jsonData.carPostDataAutoTests = CreateDataAutoTest((DateTime?)obj.carPostDataAutoTestDate);
                        string json = JsonConvert.SerializeObject(jsonData);

                        if (jsonData.carModelSmokeMeters.Count != 0 || jsonData.carPostDataSmokeMeters.Count != 0
                            || jsonData.carModelAutoTests.Count != 0 || jsonData.carPostDataAutoTests.Count != 0)
                        {
                            //Sending data to the server
                            var data = Encoding.UTF8.GetBytes(json);
                            stream.Write(data, 0, data.Length);

                            Console.WriteLine($"{DateTime.Now} >> Data send successful{Environment.NewLine}");
                        }

                        Thread.Sleep(60000);    //one minute
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"{DateTime.Now} >> Error connect to server >> {ex.Message}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} >> Error send data >> {ex.Message}{Environment.NewLine}");
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream = null;
                    }
                    if (client != null && client.Connected != false)
                    {
                        client.Close();
                    }
                }

                Thread.Sleep(60000);    //one minute
            }
        }

        private static void SendCarPostId(NetworkStream stream)
        {
            var carPostId = _config.GetSection("CarPostId").Value ?? "";
            dynamic obj = new ExpandoObject();
            obj.CarPostId = carPostId;
            string json = JsonConvert.SerializeObject(obj);

            //Sending data to the server
            var data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

        private static List<CarModelSmokeMeter> CreateModelSmokeMeter(string smokeMeterModel)
        {
            List<CarModelSmokeMeter> carModelSmokeMeters = new List<CarModelSmokeMeter>();
            try
            {
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var smokeMeterPath = _config.GetConnectionString("SmokeMeterPath");
                string connectionString = $"Provider={provider};Data Source='{smokeMeterPath}';Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>(
                        $"SELECT * FROM model.dbf").AsQueryable();
                    carModelSmokeMeters = carModelSmokeMetersv
                        .ToList();
                    var indexModel = carModelSmokeMeters.FindIndex(c => c.MODEL == smokeMeterModel);
                    if (indexModel != -1)
                    {
                        carModelSmokeMeters = carModelSmokeMeters.Skip(indexModel + 1).Take(carModelSmokeMeters.Count - indexModel + 1).ToList();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return carModelSmokeMeters;
        }

        private static List<CarPostDataSmokeMeter> CreateDataSmokeMeter(DateTime? smokeMeterDataDateTime)
        {
            List<CarPostDataSmokeMeter> carPostDataSmokeMeters = new List<CarPostDataSmokeMeter>();
            try
            {
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var smokeMeterPath = _config.GetConnectionString("SmokeMeterPath");
                string connectionString = $"Provider={provider};Data Source={smokeMeterPath};Extended Properties=dBase IV;";

                var lastTime = Convert.ToDateTime(smokeMeterDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(smokeMeterDataDateTime).ToString("MM/dd/yyyy");
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carPostDataSmokeMetersv = connection.Query<CarPostDataSmokeMeter>(
                        $"SELECT * FROM data").AsQueryable();
                    carPostDataSmokeMeters = carPostDataSmokeMetersv
                        .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(smokeMeterDataDateTime))
                        .ToList();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return carPostDataSmokeMeters;
        }

        private static List<CarModelAutoTest> CreateModelAutoTest(string autoTestModel)
        {
            List<CarModelAutoTest> carModelAutoTests = new List<CarModelAutoTest>();
            try
            {
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");

                //Version 1 (model.dbf)
                if(File.Exists(Path.Combine(autoTestPath, "model.dbf")))
                {
                    string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>(
                            $"SELECT * FROM model").AsQueryable();
                        carModelAutoTests = carModelAutoTestsv
                            .ToList();
                        carModelAutoTests.ForEach(c => c.Version = 1);
                        var indexModel = carModelAutoTests.FindIndex(c => c.MODEL == autoTestModel);
                        if (indexModel != -1)
                        {
                            carModelAutoTests = carModelAutoTests.Skip(indexModel + 1).Take(carModelAutoTests.Count - indexModel + 1).ToList();
                        }
                        connection.Close();
                    }
                }
                //Version 2 (models.DB)
                else
                {
                    string connectionString = $"Provider={provider};Data Source={autoTestPath};Persist Security Info=False;Extended Properties=\"Paradox 7.x; HDR=YES\"";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>(
                            $"SELECT * FROM models").AsQueryable();
                        carModelAutoTests = carModelAutoTestsv
                            .ToList();
                        carModelAutoTests.ForEach(c => c.Version = 2);
                        var indexModel = carModelAutoTests.FindIndex(c => c.MODEL == autoTestModel);
                        if (indexModel != -1)
                        {
                            carModelAutoTests = carModelAutoTests.Skip(indexModel + 1).Take(carModelAutoTests.Count - indexModel + 1).ToList();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}{Environment.NewLine}");
            }

            return carModelAutoTests;
        }

        private static List<CarPostDataAutoTest> CreateDataAutoTest(DateTime? autoTestDataDateTime)
        {
            List<CarPostDataAutoTest> carPostDataAutoTests = new List<CarPostDataAutoTest>();
            try
            {
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");
                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");

                //Version 1 (model.dbf)
                if (File.Exists(Path.Combine(autoTestPath, "data.dbf")))
                {
                    string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>(
                            $"SELECT * FROM data").AsQueryable();
                        carPostDataAutoTests = carPostDataAutoTestsv
                            .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                            .ToList();
                        carPostDataAutoTests.ForEach(c => c.Version = 1);
                        connection.Close();
                    }
                }
                //Version 2 (models.DB)
                else
                {
                    string connectionString = $"Provider={provider};Data Source={autoTestPath};Persist Security Info=False;Extended Properties=\"Paradox 7.x; HDR=YES\"";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>(
                            $"SELECT * FROM reports").AsQueryable();
                        carPostDataAutoTests = carPostDataAutoTestsv
                            .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                            .ToList();
                        carPostDataAutoTests.ForEach(c => c.Version = 2);
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data autotest >> {ex.Message}");
            }

            return carPostDataAutoTests;
        }

        private static void ConfigureServices()
        {
            // Build configuration
            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error set file configuration >> {ex.Message}{Environment.NewLine}");
            }
        }
    }
}

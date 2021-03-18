using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
        private const int port = 8888;
        private const string server = "127.0.0.1";
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
                        JsonData jsonData = new JsonData();
                        jsonData.carModelSmokeMeters = CreateModelSmokeMeter();
                        jsonData.carPostDataSmokeMeters = CreateDataSmokeMeter();
                        jsonData.carModelAutoTests = CreateModelAutoTest();
                        jsonData.carPostDataAutoTests = CreateDataAutoTest();
                        var carPostId = _config.GetSection("CarPostId").Value;
                        if (!String.IsNullOrEmpty(carPostId))
                        {
                            jsonData.CarPostId = Convert.ToInt32(carPostId);
                        }
                        string json = JsonConvert.SerializeObject(jsonData);

                        if (jsonData.carModelSmokeMeters.Count != 0 && jsonData.carPostDataSmokeMeters.Count != 0
                            && jsonData.carModelAutoTests.Count != 0 && jsonData.carPostDataAutoTests.Count != 0)
                        {
                            //Sending data to the server
                            byte[] data = Encoding.UTF8.GetBytes(json);
                            stream.Write(data, 0, data.Length);

                            //Rewriting of last taken data in appsetting.json
                            if (jsonData.carModelSmokeMeters.Count != 0)
                            {
                                UpdateAppSetting("DateTimeLastData:SmokeMeterModel", $"{jsonData.carModelSmokeMeters.LastOrDefault()?.MODEL}");
                            }
                            if (jsonData.carPostDataSmokeMeters.Count != 0)
                            {
                                UpdateAppSetting("DateTimeLastData:SmokeMeterData", $"{jsonData.carPostDataSmokeMeters.LastOrDefault()?.DATA.ToShortDateString()} {jsonData.carPostDataSmokeMeters.LastOrDefault()?.TIME}");
                            }
                            if (jsonData.carModelAutoTests.Count != 0)
                            {
                                UpdateAppSetting("DateTimeLastData:AutoTestModel", $"{jsonData.carModelAutoTests.LastOrDefault()?.MODEL}");
                            }
                            if (jsonData.carPostDataAutoTests.Count != 0)
                            {
                                UpdateAppSetting("DateTimeLastData:AutoTestData", $"{jsonData.carPostDataAutoTests.LastOrDefault()?.DATA.ToShortDateString()} {jsonData.carPostDataAutoTests.LastOrDefault()?.TIME}");
                            }

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

        private static List<CarModelSmokeMeter> CreateModelSmokeMeter()
        {
            List<CarModelSmokeMeter> carModelSmokeMeters = new List<CarModelSmokeMeter>();
            try
            {
                var smokeMeterModelPath = _config.GetConnectionString("SmokeMeterModelPath");
                var smokeMeterModel = _config.GetSection("DateTimeLastData").GetSection("SmokeMeterModel").Value;
                string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={smokeMeterModelPath};Extended Properties=dBase IV;";
                DataTable dataTable = new DataTable();

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>(
                        $"SELECT * FROM model").AsQueryable();
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

        private static List<CarPostDataSmokeMeter> CreateDataSmokeMeter()
        {
            List<CarPostDataSmokeMeter> carPostDataSmokeMeters = new List<CarPostDataSmokeMeter>();
            try
            {
                var smokeMeterDataPath = _config.GetConnectionString("SmokeMeterDataPath");
                var smokeMeterDataDateTime = _config.GetSection("DateTimeLastData").GetSection("SmokeMeterData").Value;
                string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={smokeMeterDataPath};Extended Properties=dBase IV;";
                DataTable dataTable = new DataTable();

                var lastTime = Convert.ToDateTime(smokeMeterDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(smokeMeterDataDateTime).ToString("MM/dd/yyyy");
                var a = new DateTime().Date;
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

        private static List<CarModelAutoTest> CreateModelAutoTest()
        {
            List<CarModelAutoTest> carModelAutoTests = new List<CarModelAutoTest>();
            try
            {
                var autoTestModelPath = _config.GetConnectionString("AutoTestModelPath");
                var autoTestModel = _config.GetSection("DateTimeLastData").GetSection("AutoTestModel").Value;
                string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={autoTestModelPath};Extended Properties=dBase IV;";
                DataTable dataTable = new DataTable();

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelAutoTestsv = connection.Query<CarModelAutoTest>(
                        $"SELECT * FROM model").AsQueryable();
                    carModelAutoTests = carModelAutoTestsv
                        .ToList();
                    var indexModel = carModelAutoTests.FindIndex(c => c.MODEL == autoTestModel);
                    if (indexModel != -1)
                    {
                        carModelAutoTests = carModelAutoTests.Skip(indexModel + 1).Take(carModelAutoTests.Count - indexModel + 1).ToList();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}{Environment.NewLine}");
            }

            return carModelAutoTests;
        }

        private static List<CarPostDataAutoTest> CreateDataAutoTest()
        {
            List<CarPostDataAutoTest> carPostDataAutoTests = new List<CarPostDataAutoTest>();
            try
            {
                var autoTestDataPath = _config.GetConnectionString("AutoTestDataPath");
                var autoTestDataDateTime = _config.GetSection("DateTimeLastData").GetSection("AutoTestData").Value;
                string connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={autoTestDataPath};Extended Properties=dBase IV;";
                DataTable dataTable = new DataTable();

                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>(
                        $"SELECT * FROM data").AsQueryable();
                    carPostDataAutoTests = carPostDataAutoTestsv
                        .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                        .ToList();
                    connection.Close();
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

        private static void UpdateAppSetting<T>(string sectionPathKey, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error writing app settings >> {ex.Message}{Environment.NewLine}");
            }
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            try
            {
                // split the string at the first ':' character
                var remainingSections = sectionPathKey.Split(":", 2);

                var currentSection = remainingSections[0];
                if (remainingSections.Length > 1)
                {
                    // continue with the procress, moving down the tree
                    var nextSection = remainingSections[1];
                    SetValueRecursively(nextSection, jsonObj[currentSection], value);
                }
                else
                {
                    // we've got to the end of the tree, set the value
                    jsonObj[currentSection] = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error search \"{sectionPathKey}\" section >> {ex.Message}{Environment.NewLine}");
            }
        }
    }
}

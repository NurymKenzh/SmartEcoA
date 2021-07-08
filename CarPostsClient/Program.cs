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
                        jsonData.carModelSmokeMeter = CreateModelSmokeMeter((string)obj.carModelSmokeMeterName);

                        //Version 1 (model.dbf)
                        if (File.Exists(Path.Combine(_config.GetConnectionString("AutoTestPath"), "model.dbf")))
                        {
                            jsonData.carModelAutoTestV1 = CreateModelAutoTestV1((string)obj.carModelAutoTestName);
                        }
                        //Version 2 (models.DB)
                        else
                        {
                            jsonData.carModelAutoTestV2 = CreateModelAutoTestV2((string)obj.carModelAutoTestName);
                        }

                        if (jsonData.carModelSmokeMeter == null)
                        {
                            jsonData.carPostDataSmokeMeter = CreateDataSmokeMeter((DateTime?)obj.carPostDataSmokeMeterDate);
                        }
                        //Version 1 (data.dbf)
                        if (File.Exists(Path.Combine(_config.GetConnectionString("AutoTestPath"), "data.dbf")))
                        {
                            if (jsonData.carModelAutoTestV1 == null)
                            {
                                jsonData.carPostDataAutoTestV1 = CreateDataAutoTestV1((DateTime?)obj.carPostDataAutoTestDate);
                            }
                        }
                        //Version 2 (reports.DB)
                        else
                        {
                            if (jsonData.carModelAutoTestV2 == null)
                            {
                                jsonData.carPostDataAutoTestV2 = CreateDataAutoTestV2((DateTime?)obj.carPostDataAutoTestDate);
                            }
                        }
                        string json = JsonConvert.SerializeObject(jsonData);

                        if (jsonData.carModelSmokeMeter != null || jsonData.carPostDataSmokeMeter != null
                            || jsonData.carModelAutoTestV1 != null || jsonData.carPostDataAutoTestV1 != null
                            || jsonData.carModelAutoTestV2 != null || jsonData.carPostDataAutoTestV2 != null)
                        {
                            //Sending data to the server
                            var data = Encoding.UTF8.GetBytes(json);
                            stream.Write(data, 0, data.Length);

                            Console.WriteLine($"{DateTime.Now} >> Data send successful{Environment.NewLine}");
                        }

                        Thread.Sleep(5000);    //5 sec
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

        private static CarModelSmokeMeter CreateModelSmokeMeter(string smokeMeterModel)
        {
            try
            {
                CarModelSmokeMeter carModelSmokeMeter = null;
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var smokeMeterPath = _config.GetConnectionString("SmokeMeterPath");
                string connectionString = $"Provider={provider};Data Source='{smokeMeterPath}';Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelSmokeMeters = connection.Query<CarModelSmokeMeter>(
                        $"SELECT * FROM model.dbf").ToList();
                    var indexModel = carModelSmokeMeters.FindIndex(c => c.MODEL == smokeMeterModel);
                    if (indexModel != -1)
                    {
                        carModelSmokeMeter = carModelSmokeMeters.Skip(indexModel + 1).FirstOrDefault();
                    }
                    else
                    {
                        carModelSmokeMeter = carModelSmokeMeters.FirstOrDefault();
                    }
                    connection.Close();
                }
                return carModelSmokeMeter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private static CarPostDataSmokeMeter CreateDataSmokeMeter(DateTime? smokeMeterDataDateTime)
        {
            try
            {
                CarPostDataSmokeMeter carPostDataSmokeMeter = null;
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var smokeMeterPath = _config.GetConnectionString("SmokeMeterPath");
                string connectionString = $"Provider={provider};Data Source={smokeMeterPath};Extended Properties=dBase IV;";

                var lastTime = Convert.ToDateTime(smokeMeterDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(smokeMeterDataDateTime).ToString("MM/dd/yyyy");
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    carPostDataSmokeMeter = connection.Query<CarPostDataSmokeMeter>(
                        $"SELECT * FROM data")
                        .ToList()
                        .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(smokeMeterDataDateTime))
                        .FirstOrDefault();
                    connection.Close();
                }
                return carPostDataSmokeMeter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data smokemeter >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private static CarModelAutoTestV1 CreateModelAutoTestV1(string autoTestModel)
        {
            try
            {
                CarModelAutoTestV1 carModelAutoTest = null;
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTestV1>(
                        $"SELECT * FROM model").ToList();
                    carModelAutoTests.ForEach(c => c.Version = 1);
                    var indexModel = carModelAutoTests.FindIndex(c => c.MODEL == autoTestModel);
                    if (indexModel != -1)
                    {
                        carModelAutoTest = carModelAutoTests.Skip(indexModel + 1).FirstOrDefault();
                    }
                    else
                    {
                        carModelAutoTest = carModelAutoTests.FirstOrDefault();
                    }
                    connection.Close();
                }
                return carModelAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private static CarModelAutoTestV2 CreateModelAutoTestV2(string autoTestModel)
        {
            try
            {
                CarModelAutoTestV2 carModelAutoTest = null;
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Persist Security Info=False;Extended Properties=\"Paradox 7.x; HDR=YES\"";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTestV2>(
                        $"SELECT * FROM models").ToList();
                    carModelAutoTests.ForEach(c => c.Version = 2);
                    var indexModel = carModelAutoTests.FindIndex(c => c.Name == autoTestModel);
                    if (indexModel != -1)
                    {
                        carModelAutoTest = carModelAutoTests.Skip(indexModel + 1).FirstOrDefault();
                    }
                    else
                    {
                        carModelAutoTest = carModelAutoTests.FirstOrDefault();
                    }
                    connection.Close();
                }
                return carModelAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}{Environment.NewLine}");
            }

            return null;
        }

        private static CarPostDataAutoTestV1 CreateDataAutoTestV1(DateTime? autoTestDataDateTime)
        {
            try
            {
                CarPostDataAutoTestV1 carPostDataAutoTest = null;
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");
                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    carPostDataAutoTest = connection.Query<CarPostDataAutoTestV1>(
                        $"SELECT * FROM data")
                        .ToList()
                        .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                        .FirstOrDefault();
                    if (carPostDataAutoTest != null)
                    {
                        carPostDataAutoTest.Version = 1;
                    }
                    connection.Close();
                }
                return carPostDataAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data autotest >> {ex.Message}");
            }

            return null;
        }

        private static CarPostDataAutoTestV2 CreateDataAutoTestV2(DateTime? autoTestDataDateTime)
        {
            try
            {
                CarPostDataAutoTestV2 carPostDataAutoTest = null;
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");
                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Persist Security Info=False;Extended Properties=\"Paradox 7.x; HDR=YES\"";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    carPostDataAutoTest = connection.Query<CarPostDataAutoTestV2>(
                        $"SELECT * FROM reports")
                        .ToList()
                        .Where(c => Convert.ToDateTime($"{c.Date.ToShortDateString()} {c.Time.TimeOfDay}") > Convert.ToDateTime(autoTestDataDateTime))
                        .FirstOrDefault();
                    if (carPostDataAutoTest != null)
                    {
                        carPostDataAutoTest.Version = 2;
                        var carModeAutoTest = connection.Query<CarModelAutoTestV2>(
                            $"SELECT * FROM models as m WHERE m.Index = {carPostDataAutoTest.Model}").FirstOrDefault();
                        if (carModeAutoTest == null && carPostDataAutoTest.Model == 0)
                        {
                            carModeAutoTest = connection.Query<CarModelAutoTestV2>(
                            $"SELECT * FROM models as m WHERE m.Index is null").LastOrDefault();
                        }
                        carPostDataAutoTest.ModelName = carModeAutoTest.Name;
                    }
                    connection.Close();
                }
                return carPostDataAutoTest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create data autotest >> {ex.Message}");
            }

            return null;
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

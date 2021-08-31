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
                        jsonData.carModelAutoTest = CreateModelAutoTest((int?)obj.carModelAutoTestId);

                        if (jsonData.carModelSmokeMeter == null)
                        {
                            jsonData.carPostDataSmokeMeter = CreateDataSmokeMeter((DateTime?)obj.carPostDataSmokeMeterDate);
                        }
                        if (jsonData.carModelAutoTest == null)
                        {
                            jsonData.carPostDataAutoTest = CreateDataAutoTest((DateTime?)obj.carPostDataAutoTestDate);
                            jsonData.tester = CreateTester((string)obj.testerName);
                        }

                        string json = JsonConvert.SerializeObject(jsonData);

                        if (jsonData.carModelSmokeMeter != null || jsonData.carPostDataSmokeMeter != null
                            || jsonData.carModelAutoTest != null || jsonData.carPostDataAutoTest != null)
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

        private static CarModelAutoTest CreateModelAutoTest(int? autoTestModelId)
        {
            try
            {
                CarModelAutoTest carModelAutoTest = null;
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var carModelAutoTests = connection.Query<CarModelAutoTest>(
                        $"SELECT * FROM model").OrderBy(c => c.ID).ToList();
                    if (autoTestModelId != null)
                    {
                        carModelAutoTest = carModelAutoTests.Where(c => c.ID == autoTestModelId + 1).FirstOrDefault();
                        if (carModelAutoTest != null)
                        {
                            var typeEco = connection.Query<TypeEco>(
                            $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                            carModelAutoTest.TypeEcoName = typeEco.NAME;
                        }
                    }
                    else
                    {
                        carModelAutoTest = carModelAutoTests.FirstOrDefault();
                        if (carModelAutoTest != null)
                        {
                            var typeEco = connection.Query<TypeEco>(
                            $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                            carModelAutoTest.TypeEcoName = typeEco.NAME;
                        }
                    }
                    //var indexModel = carModelAutoTests.FindIndex(c => c.ID == autoTestModelId);
                    //if (indexModel != -1)
                    //{
                    //    carModelAutoTest = carModelAutoTests.Skip(indexModel + 1).FirstOrDefault();
                    //    if (carModelAutoTest != null)
                    //    {
                    //        var typeEco = connection.Query<TypeEco>(
                    //        $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                    //        carModelAutoTest.TypeEcoName = typeEco.NAME;
                    //    }
                    //}
                    //else
                    //{
                    //    carModelAutoTest = carModelAutoTests.FirstOrDefault();
                    //    if (carModelAutoTest != null)
                    //    {
                    //        var typeEco = connection.Query<TypeEco>(
                    //        $"SELECT * FROM type_eco as m WHERE m.ID = {carModelAutoTest.ID_ECOLOG}").FirstOrDefault();
                    //        carModelAutoTest.TypeEcoName = typeEco.NAME;
                    //    }
                    //}
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

        private static CarPostDataAutoTest CreateDataAutoTest(DateTime? autoTestDataDateTime)
        {
            try
            {
                CarPostDataAutoTest carPostDataAutoTest = null;
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");
                var lastTime = Convert.ToDateTime(autoTestDataDateTime).ToString("HH:mm:ss");
                var lastDate = Convert.ToDateTime(autoTestDataDateTime).ToString("dd.MM.yyyy");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    carPostDataAutoTest = connection.Query<CarPostDataAutoTest>(
                        $"SELECT * FROM Main")
                        .ToList()
                        .Where(c => Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}") > Convert.ToDateTime(autoTestDataDateTime))
                        .FirstOrDefault();
                    if (carPostDataAutoTest != null)
                    {
                        var dopInfo = connection.Query<DopInfo>(
                            $"SELECT * FROM dop_info as m WHERE m.ID = {carPostDataAutoTest.ID}").FirstOrDefault();
                        if (dopInfo != null)
                        {
                            var tester = connection.Query<Tester>(
                                $"SELECT * FROM tester as m WHERE m.ID = {dopInfo.ID_TESTER}").FirstOrDefault();
                            if (tester != null)
                            {
                                dopInfo.TesterName = tester.NAME;
                            }
                            carPostDataAutoTest.DopInfo = dopInfo;
                        }
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

        private static Tester CreateTester(string testerName)
        {
            try
            {
                Tester tester = null;
                //var provider = IntPtr.Size == 8 ? "Microsoft.ACE.OLEDB.12.0" : "Microsoft.Jet.OLEDB.4.0";
                var provider = "Microsoft.Jet.OLEDB.4.0";
                var autoTestPath = _config.GetConnectionString("AutoTestPath");

                string connectionString = $"Provider={provider};Data Source={autoTestPath};Extended Properties=dBase IV;";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    var testers = connection.Query<Tester>(
                        $"SELECT * FROM tester").OrderBy(t => t.ID).ToList();
                    var indexModel = testers.FindIndex(c => c.NAME == testerName);
                    if (indexModel != -1)
                    {
                        tester = testers.Skip(indexModel + 1).FirstOrDefault();
                    }
                    else
                    {
                        tester = testers.FirstOrDefault();
                    }
                    connection.Close();
                }
                return tester;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error create model autotest >> {ex.Message}{Environment.NewLine}");
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

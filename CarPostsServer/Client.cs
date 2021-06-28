using CarPostsServer.ClientModels;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Dapper;
using System.Net;
using CarPostsServer.ServerModels;
using System.Dynamic;

namespace CarPostsServer
{
    class Client
    {
        public TcpClient _tcpClient;
        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            string ip = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address.ToString();
            try
            {
                Console.WriteLine($"{DateTime.Now} >> Client {ip} connection{Environment.NewLine}");
                stream = _tcpClient.GetStream();
                byte[] data = new byte[_tcpClient.ReceiveBufferSize];
                string CarPostId = null;
                int carPostId = -1;
                while (true)
                {
                    data = new byte[_tcpClient.ReceiveBufferSize];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    if (builder.Length != 0)
                    {
                        string jsonString = builder.ToString();
                        dynamic obj = JsonConvert.DeserializeObject(jsonString);
                        CarPostId = (string)obj.CarPostId;

                        //Get CarPostId from Client
                        if (CarPostId != null)
                        {
                            if (!String.IsNullOrEmpty(CarPostId) && Int32.TryParse(CarPostId, out carPostId))
                            {
                                string message = GetLastData(carPostId);

                                //Send message with last data
                                data = Encoding.Unicode.GetBytes(message);
                                stream.Write(data, 0, data.Length);
                            }
                            else
                            {
                                dynamic err = new ExpandoObject();
                                err.Error = "CarPostId cannot be symbols, empty or absent!";
                                string message = JsonConvert.SerializeObject(err);

                                //Send error
                                data = Encoding.Unicode.GetBytes(message);
                                stream.Write(data, 0, data.Length);
                            }
                        }
                        //Get data from Client
                        else
                        {
                            if (carPostId != -1)
                            {
                                ParseData(jsonString, carPostId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Lost connection {ip} >> {ex.Message}{Environment.NewLine}");
            }
            finally
            {
                stream?.Close();
                _tcpClient?.Close();
            }
        }

        private string GetLastData(int carPostId)
        {
            string carModelSmokeMeterName = String.Empty;
            string carModelAutoTestName = String.Empty;
            DateTime? carPostDataAutoTestDate = new DateTime();
            DateTime? carPostDataSmokeMeterDate = new DateTime();
            try
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                        $"FROM public.\"CarModelSmokeMeter\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carModelSmokeMeterName = carModelSmokeMetersv.FirstOrDefault() == null ? carModelSmokeMeterName : carModelSmokeMetersv.FirstOrDefault().Name;

                    var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                        $"FROM public.\"CarModelAutoTest\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carModelAutoTestName = carModelAutoTestsv.FirstOrDefault() == null ? carModelAutoTestName : carModelAutoTestsv.FirstOrDefault().Name;

                    var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>($"SELECT * " +
                        $"FROM public.\"CarPostDataAutoTest\" as datas " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = datas.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carPostDataAutoTestDate = carPostDataAutoTestsv.FirstOrDefault() == null ? carPostDataAutoTestDate : carPostDataAutoTestsv.FirstOrDefault().DateTime;

                    var carPostDataSmokeMetersv = connection.Query<CarPostDataSmokeMeter>($"SELECT * " +
                        $"FROM public.\"CarPostDataSmokeMeter\" as datas " +
                        $"JOIN public.\"CarModelSmokeMeter\" as model ON model.\"Id\" = datas.\"CarModelSmokeMeterId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carPostDataSmokeMeterDate = carPostDataSmokeMetersv.FirstOrDefault() == null ? carPostDataSmokeMeterDate : carPostDataSmokeMetersv.FirstOrDefault().DateTime;
                    connection.Close();
                }

                dynamic obj = new ExpandoObject();
                obj.carModelSmokeMeterName = carModelSmokeMeterName;
                obj.carModelAutoTestName = carModelAutoTestName;
                obj.carPostDataAutoTestDate = carPostDataAutoTestDate;
                obj.carPostDataSmokeMeterDate = carPostDataSmokeMeterDate;
                string json = JsonConvert.SerializeObject(obj);

                return json;
            }
            catch (Exception ex)
            {
                dynamic err = new ExpandoObject();
                err.Error = ex.Message;
                string json = JsonConvert.SerializeObject(err);
                return json;
            }
        }

        private void ParseData(string jsonString, int carPostId)
        {
            try
            {
                var clientJsonData = JsonConvert.DeserializeObject<ClientJsonData>(jsonString);

                Console.WriteLine($"{DateTime.Now} >> Get data from CarPost {carPostId}{Environment.NewLine}");

                if (clientJsonData.carModelSmokeMeter != null)
                {
                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter started{Environment.NewLine}");

                        carModelSmokeMeter.Name = clientJsonData.carModelSmokeMeter.MODEL;
                        carModelSmokeMeter.Boost = clientJsonData.carModelSmokeMeter.NADDUV;
                        carModelSmokeMeter.DFreeMark = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.D_FREE);
                        carModelSmokeMeter.DMaxMark = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.D_MAX);
                        carModelSmokeMeter.CarPostId = carPostId;

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error parse model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            string execute = $"INSERT INTO public.\"CarModelSmokeMeter\"(\"CarPostId\", \"Name\", \"Boost\", \"DFreeMark\", \"DMaxMark\")" +
                                    $"VALUES({carModelSmokeMeter.CarPostId.ToString()}," +
                                    $"'{carModelSmokeMeter.Name}'," +
                                    $"{carModelSmokeMeter.Boost.ToString()}," +
                                    $"{carModelSmokeMeter.DFreeMark.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.DMaxMark.ToString().Replace(",", ".")});";
                            connection.Execute(execute);
                            connection.Close();
                        }
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error insert model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carModelAutoTestV1 != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

                        carModelAutoTest.Name = clientJsonData.carModelAutoTestV1.MODEL;
                        carModelAutoTest.EngineType = clientJsonData.carModelAutoTestV1.DVIG;
                        carModelAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MIN_TAH);
                        carModelAutoTest.DEL_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.DEL_MIN);
                        carModelAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MAX_TAH);
                        carModelAutoTest.DEL_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.DEL_MAX);
                        carModelAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MIN_CO);
                        carModelAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MAX_CO);
                        carModelAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MIN_CH);
                        carModelAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.MAX_CH);
                        carModelAutoTest.L_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.L_MIN);
                        carModelAutoTest.L_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.L_MAX);
                        carModelAutoTest.K_SVOB = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.K_SVOB);
                        carModelAutoTest.K_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTestV1.K_MAX);
                        carModelAutoTest.CarPostId = carPostId;
                        carModelAutoTest.Version = clientJsonData.carModelAutoTestV1.Version;

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error parse model autotest >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            string execute = $"INSERT INTO public.\"CarModelAutoTest\"(\"CarPostId\", \"Name\", \"EngineType\", \"MIN_TAH\", \"DEL_MIN\", \"MAX_TAH\", " +
                                    $"\"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\", \"Version\")" +
                                    $"VALUES({carModelAutoTest.CarPostId.ToString()}," +
                                    $"'{carModelAutoTest.Name}'," +
                                    $"{carModelAutoTest.EngineType.ToString()}," +
                                    $"{carModelAutoTest.MIN_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.DEL_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.DEL_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.L_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.L_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.K_SVOB.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.K_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.Version.ToString()}); ";
                            connection.Execute(execute);
                            connection.Close();
                        }
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error insert model autotest >> {ex.Message}{Environment.NewLine}");
                    }
                }
                else if (clientJsonData.carModelAutoTestV2 != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

                        carModelAutoTest.Name = clientJsonData.carModelAutoTestV2.Name;
                        carModelAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinTax);
                        carModelAutoTest.DEL_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinTaxD);
                        carModelAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxTax);
                        carModelAutoTest.DEL_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxTaxD);
                        carModelAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinCO);
                        carModelAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxCO);
                        carModelAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinCH);
                        carModelAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxCH);
                        carModelAutoTest.L_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinLambda);
                        carModelAutoTest.L_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxLambda);
                        carModelAutoTest.MIN_CO2 = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinCO2);
                        carModelAutoTest.MIN_O2 = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinO2);
                        carModelAutoTest.MIN_NOx = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MinNOx);
                        carModelAutoTest.MAX_CO2 = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxCO2);
                        carModelAutoTest.MAX_O2 = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxO2);
                        carModelAutoTest.MAX_NOx = Convert.ToDecimal(clientJsonData.carModelAutoTestV2.MaxNOx);
                        carModelAutoTest.CarPostId = carPostId;
                        carModelAutoTest.Version = clientJsonData.carModelAutoTestV2.Version;

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error parse model autotest >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            string execute = $"INSERT INTO public.\"CarModelAutoTest\"(\"CarPostId\", \"Name\", \"MIN_TAH\", \"DEL_MIN\", \"MAX_TAH\", " +
                                    $"\"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"MIN_CO2\", \"MIN_O2\", \"MIN_NOx\", " +
                                    $"\"MAX_CO2\", \"MAX_O2\", \"MAX_NOx\", \"Version\")" +
                                    $"VALUES({carModelAutoTest.CarPostId.ToString()}," +
                                    $"'{carModelAutoTest.Name}'," +
                                    $"{carModelAutoTest.MIN_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.DEL_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.DEL_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.L_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.L_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_CO2.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_O2.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MIN_NOx.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_CO2.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_O2.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.MAX_NOx.ToString().Replace(",", ".")}," +
                                    $"{carModelAutoTest.Version.ToString()}); ";
                            connection.Execute(execute);
                            connection.Close();
                        }
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error insert model autotest >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataSmokeMeter != null)
                {
                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        connection.Open();
                        var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelSmokeMeter\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"Name\" = '{clientJsonData.carPostDataSmokeMeter.MODEL}' " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelSmokeMeter = carModelSmokeMetersv.FirstOrDefault();
                    }

                    if (carModelSmokeMeter != null)
                    {
                        CarPostDataSmokeMeter carPostDataSmokeMeter = new CarPostDataSmokeMeter();
                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter started{Environment.NewLine}");

                            carPostDataSmokeMeter.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataSmokeMeter.DATA.ToShortDateString()} {clientJsonData.carPostDataSmokeMeter.TIME}");
                            carPostDataSmokeMeter.Number = clientJsonData.carPostDataSmokeMeter.NOMER;
                            carPostDataSmokeMeter.RunIn = clientJsonData.carPostDataSmokeMeter.TYPE;
                            carPostDataSmokeMeter.DFree = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.D_FREE);
                            carPostDataSmokeMeter.DMax = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.D_MAX);
                            carPostDataSmokeMeter.NDFree = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.N_D_FREE);
                            carPostDataSmokeMeter.NDMax = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.N_D_MAX);
                            carPostDataSmokeMeter.CarModelSmokeMeterId = carModelSmokeMeter.Id;

                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error parse data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter started{Environment.NewLine}");
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                            {
                                connection.Open();
                                string execute = $"INSERT INTO public.\"CarPostDataSmokeMeter\"(\"CarModelSmokeMeterId\", \"DateTime\", \"Number\", \"RunIn\", " +
                                            $"\"DFree\", \"DMax\", \"NDFree\", \"NDMax\")" +
                                            $"VALUES({carPostDataSmokeMeter.CarModelSmokeMeterId.ToString()}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataSmokeMeter.DateTime.Year.ToString()}, " +
                                                $"{carPostDataSmokeMeter.DateTime.Month.ToString()}, " +
                                                $"{carPostDataSmokeMeter.DateTime.Day.ToString()}, " +
                                                $"{carPostDataSmokeMeter.DateTime.Hour.ToString()}, " +
                                                $"{carPostDataSmokeMeter.DateTime.Minute.ToString()}, " +
                                                $"{carPostDataSmokeMeter.DateTime.Second.ToString()})," +
                                            $"'{carPostDataSmokeMeter.Number}'," +
                                            $"{carPostDataSmokeMeter.RunIn.ToString()}," +
                                            $"{carPostDataSmokeMeter.DFree.ToString().Replace(",", ".")}," +
                                            $"{carPostDataSmokeMeter.DMax.ToString().Replace(",", ".")}," +
                                            $"{carPostDataSmokeMeter.NDFree.ToString().Replace(",", ".")}," +
                                            $"{carPostDataSmokeMeter.NDMax.ToString().Replace(",", ".")});";
                                connection.Execute(execute);
                                connection.Close();
                            }
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error insert data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} >> Model for data smokemeter does not exist in DB. Data: {clientJsonData.carPostDataSmokeMeter}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataAutoTestV1 != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelAutoTest\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"Name\" = '{clientJsonData.carPostDataAutoTestV1.MODEL}' " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelAutoTest = carModelAutoTestsv.FirstOrDefault();
                        connection.Close();
                    }

                    if (carModelAutoTest != null)
                    {
                        CarPostDataAutoTest carPostDataAutoTest = new CarPostDataAutoTest();
                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotestV1 started{Environment.NewLine}");

                            carPostDataAutoTest.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataAutoTestV1.DATA.ToShortDateString()} {clientJsonData.carPostDataAutoTestV1.TIME}");
                            carPostDataAutoTest.Number = clientJsonData.carPostDataAutoTestV1.NOMER;
                            carPostDataAutoTest.DOPOL1 = clientJsonData.carPostDataAutoTestV1.DOPOL1;
                            carPostDataAutoTest.DOPOL2 = clientJsonData.carPostDataAutoTestV1.DOPOL2;
                            carPostDataAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_TAH);
                            carPostDataAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_CO);
                            carPostDataAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_CH);
                            carPostDataAutoTest.MIN_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_CO2);
                            carPostDataAutoTest.MIN_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_O2);
                            carPostDataAutoTest.MIN_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_L);
                            carPostDataAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_TAH);
                            carPostDataAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_CO);
                            carPostDataAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_CH);
                            carPostDataAutoTest.MAX_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_CO2);
                            carPostDataAutoTest.MAX_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_O2);
                            carPostDataAutoTest.MAX_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_L);
                            carPostDataAutoTest.ZAV_NOMER = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.ZAV_NOMER);
                            carPostDataAutoTest.K_1 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_1);
                            carPostDataAutoTest.K_2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_2);
                            carPostDataAutoTest.K_3 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_3);
                            carPostDataAutoTest.K_4 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_4);
                            carPostDataAutoTest.K_SVOB = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_SBOB);
                            carPostDataAutoTest.K_MAX = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.K_MAX);
                            carPostDataAutoTest.MIN_NO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MIN_NO);
                            carPostDataAutoTest.MAX_NO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV1.MAX_NO);
                            carPostDataAutoTest.CarModelAutoTestId = carModelAutoTest.Id;
                            carPostDataAutoTest.Version = clientJsonData.carPostDataAutoTestV1.Version;

                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotestV1 finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error parse data autotestV1 >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotestV1 started{Environment.NewLine}");
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                            {
                                connection.Open();
                                string execute = $"INSERT INTO public.\"CarPostDataAutoTest\"(\"CarModelAutoTestId\", \"DateTime\", \"Number\", \"DOPOL1\", \"DOPOL2\", \"MIN_TAH\", " +
                                            $"\"MIN_CO\", \"MIN_CH\", \"MIN_CO2\", \"MIN_O2\", \"MIN_L\", \"MAX_TAH\", \"MAX_CO\", \"MAX_CH\", \"MAX_CO2\", \"MAX_O2\", \"MAX_L\", \"ZAV_NOMER\", " +
                                            $"\"K_1\", \"K_2\", \"K_3\", \"K_4\", \"K_SVOB\", \"K_MAX\", \"MIN_NO\", \"MAX_NO\", \"Version\")" +
                                            $"VALUES({carPostDataAutoTest.CarModelAutoTestId.ToString()}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataAutoTest.DateTime.Year.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Month.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Day.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Hour.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Minute.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Second.ToString()})," +
                                            $"'{carPostDataAutoTest.Number}'," +
                                            $"'{carPostDataAutoTest.DOPOL1}'," +
                                            $"'{carPostDataAutoTest.DOPOL2}'," +
                                            $"{carPostDataAutoTest.MIN_TAH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CO2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_O2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_L.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_TAH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CO2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_O2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_L.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.ZAV_NOMER.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_1.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_3.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_4.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_SVOB.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.K_MAX.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_NO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_NO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.Version.ToString()});";
                                connection.Execute(execute);
                                connection.Close();
                            }
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotestV1 finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error insert data autotestV1 >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} >> Model for data autotestV1 does not exist in DB. Data: {clientJsonData.carPostDataAutoTestV1}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataAutoTestV2 != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelAutoTest\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"Name\" = '{clientJsonData.carPostDataAutoTestV2.ModelName}' " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelAutoTest = carModelAutoTestsv.FirstOrDefault();
                        connection.Close();
                    }

                    if (carModelAutoTest != null)
                    {
                        CarPostDataAutoTest carPostDataAutoTest = new CarPostDataAutoTest();
                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotestV2 started{Environment.NewLine}");

                            carPostDataAutoTest.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataAutoTestV2.Date.ToShortDateString()} {clientJsonData.carPostDataAutoTestV2.Time.TimeOfDay}");
                            carPostDataAutoTest.Number = clientJsonData.carPostDataAutoTestV2.GovNumber;
                            carPostDataAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinTax);
                            carPostDataAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinCO);
                            carPostDataAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinCH);
                            carPostDataAutoTest.MIN_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinCO2);
                            carPostDataAutoTest.MIN_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinO2);
                            carPostDataAutoTest.MIN_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinLambda);
                            carPostDataAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxTax);
                            carPostDataAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxCO);
                            carPostDataAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxCH);
                            carPostDataAutoTest.MAX_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxCO2);
                            carPostDataAutoTest.MAX_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxO2);
                            carPostDataAutoTest.MAX_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxLambda);
                            carPostDataAutoTest.ATNUM = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.ATNum);
                            carPostDataAutoTest.MIN_NOx = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MinNOx);
                            carPostDataAutoTest.MAX_NOx = Convert.ToDecimal(clientJsonData.carPostDataAutoTestV2.MaxNOx);
                            carPostDataAutoTest.CarModelAutoTestId = carModelAutoTest.Id;
                            carPostDataAutoTest.Version = clientJsonData.carPostDataAutoTestV2.Version;

                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotestV2 finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error parse data autotestV2 >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotestV2 started{Environment.NewLine}");
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                            {
                                connection.Open();
                                string execute = $"INSERT INTO public.\"CarPostDataAutoTest\"(\"CarModelAutoTestId\", \"DateTime\", \"Number\", \"MIN_TAH\", " +
                                            $"\"MIN_CO\", \"MIN_CH\", \"MIN_CO2\", \"MIN_O2\", \"MIN_L\", \"MAX_TAH\", \"MAX_CO\", \"MAX_CH\", \"MAX_CO2\", " +
                                            $"\"MAX_O2\", \"MAX_L\", \"ATNUM\", \"MIN_NOx\", \"MAX_NOx\", \"Version\")" +
                                            $"VALUES({carPostDataAutoTest.CarModelAutoTestId.ToString()}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataAutoTest.DateTime.Year.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Month.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Day.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Hour.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Minute.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Second.ToString()})," +
                                            $"'{carPostDataAutoTest.Number}'," +
                                            $"{carPostDataAutoTest.MIN_TAH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_CO2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_O2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_L.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_TAH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CO.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CH.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_CO2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_O2.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_L.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.ATNUM.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MIN_NOx.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.MAX_NOx.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.Version.ToString()});";
                                connection.Execute(execute);
                                connection.Close();
                            }
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotestV2 finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error insert data autotestV2 >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} >> Model for data autotestV2 does not exist in DB. Data: {clientJsonData.carPostDataAutoTestV2}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error parse data >> {ex.Message}{Environment.NewLine}");
            }
        }
    }
}

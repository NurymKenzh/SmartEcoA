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
            int? carModelSmokeMeterId = null;
            int? carModelAutoTestId = null;
            DateTime? carPostDataAutoTestDate = new DateTime();
            DateTime? carPostDataSmokeMeterDate = new DateTime();
            string testerName = string.Empty;
            try
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                        $"FROM public.\"CarModelSmokeMeter\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carModelSmokeMeterId = carModelSmokeMetersv.FirstOrDefault() == null ? carModelSmokeMeterId : carModelSmokeMetersv.FirstOrDefault().ParadoxId;

                    var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                        $"FROM public.\"CarModelAutoTest\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    carModelAutoTestId = carModelAutoTestsv.FirstOrDefault() == null ? carModelAutoTestId : carModelAutoTestsv.FirstOrDefault().ParadoxId;

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
                    ///////////
                    var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                        $"FROM public.\"Tester\" as tester " +
                        $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY tester.\"Id\" DESC " +
                        $"LIMIT 1", commandTimeout: 86400);
                    testerName = testersv.FirstOrDefault() == null ? testerName : testersv.FirstOrDefault().Name;
                    connection.Close();
                }

                dynamic obj = new ExpandoObject();
                obj.carModelSmokeMeterId = carModelSmokeMeterId;
                obj.carModelAutoTestId = carModelAutoTestId;
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
                    TypeEcoClass typeEcoClass = new TypeEcoClass();
                    if (!string.IsNullOrEmpty(clientJsonData.carModelSmokeMeter.TypeEcoName))
                    {
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            var typeEcoClassv = connection.Query<TypeEcoClass>($"SELECT \"Id\", \"Name\" " +
                                $"FROM public.\"TypeEcoClass\" " +
                                $"WHERE \"Name\" = '{clientJsonData.carModelSmokeMeter.TypeEcoName}' " +
                                $"ORDER BY \"Id\"", commandTimeout: 86400);
                            typeEcoClass = typeEcoClassv.FirstOrDefault();
                        }
                    }

                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter started{Environment.NewLine}");

                        carModelSmokeMeter.Name = clientJsonData.carModelSmokeMeter.MODEL;
                        carModelSmokeMeter.TypeEcoClassId = typeEcoClass.Id;
                        carModelSmokeMeter.Category = clientJsonData.carModelSmokeMeter.CATEGORY;
                        carModelSmokeMeter.EngineType = clientJsonData.carModelSmokeMeter.DVIG;
                        carModelSmokeMeter.MIN_TAH = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MIN_TAH);
                        carModelSmokeMeter.DEL_MIN = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.DEL_MIN);
                        carModelSmokeMeter.MAX_TAH = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MAX_TAH);
                        carModelSmokeMeter.DEL_MAX = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.DEL_MAX);
                        carModelSmokeMeter.MIN_CO = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MIN_CO);
                        carModelSmokeMeter.MAX_CO = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MAX_CO);
                        carModelSmokeMeter.MIN_CH = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MIN_CH);
                        carModelSmokeMeter.MAX_CH = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.MAX_CH);
                        carModelSmokeMeter.L_MIN = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.L_MIN);
                        carModelSmokeMeter.L_MAX = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.L_MAX);
                        carModelSmokeMeter.K_SVOB = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.K_SVOB);
                        carModelSmokeMeter.K_MAX = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.K_MAX);
                        carModelSmokeMeter.CarPostId = carPostId;
                        carModelSmokeMeter.ParadoxId = clientJsonData.carModelSmokeMeter.ID;

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
                            string execute = $"INSERT INTO public.\"CarModelSmokeMeter\"(\"CarPostId\", \"Name\", \"TypeEcoClassId\", \"Category\", \"EngineType\", \"MIN_TAH\", " +
                                $"\"DEL_MIN\", \"MAX_TAH\", \"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\", \"ParadoxId\")" +
                                    $"VALUES({carModelSmokeMeter.CarPostId.ToString()}," +
                                    $"'{carModelSmokeMeter.Name}'," +
                                    $"{carModelSmokeMeter.TypeEcoClassId.ToString()}," +
                                    $"'{carModelSmokeMeter.Category}'," +
                                    $"{carModelSmokeMeter.EngineType.ToString()}," +
                                    $"{carModelSmokeMeter.MIN_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.DEL_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.MAX_TAH.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.DEL_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.MIN_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.MAX_CO.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.MIN_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.MAX_CH.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.L_MIN.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.L_MAX.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.K_SVOB.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.K_MAX.ToString().Replace(",", ".")}," +
                                    $"{(carModelSmokeMeter.ParadoxId != null ? carModelSmokeMeter.ParadoxId.ToString() : "null")}); ";
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
                if (clientJsonData.carModelAutoTest != null)
                {
                    TypeEcoClass typeEcoClass = new TypeEcoClass();
                    if (!string.IsNullOrEmpty(clientJsonData.carModelAutoTest.TypeEcoName))
                    {
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            var typeEcoClassv = connection.Query<TypeEcoClass>($"SELECT \"Id\", \"Name\" " +
                                $"FROM public.\"TypeEcoClass\" " +
                                $"WHERE \"Name\" = '{clientJsonData.carModelAutoTest.TypeEcoName}' " +
                                $"ORDER BY \"Id\"", commandTimeout: 86400);
                            typeEcoClass = typeEcoClassv.FirstOrDefault();
                        }
                    }

                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

                        carModelAutoTest.Name = clientJsonData.carModelAutoTest.MODEL;
                        carModelAutoTest.TypeEcoClassId = typeEcoClass.Id;
                        carModelAutoTest.Category = clientJsonData.carModelAutoTest.CATEGORY;
                        carModelAutoTest.EngineType = clientJsonData.carModelAutoTest.DVIG;
                        carModelAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTest.MIN_TAH);
                        carModelAutoTest.DEL_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTest.DEL_MIN);
                        carModelAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carModelAutoTest.MAX_TAH);
                        carModelAutoTest.DEL_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTest.DEL_MAX);
                        carModelAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carModelAutoTest.MIN_CO);
                        carModelAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carModelAutoTest.MAX_CO);
                        carModelAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carModelAutoTest.MIN_CH);
                        carModelAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carModelAutoTest.MAX_CH);
                        carModelAutoTest.L_MIN = Convert.ToDecimal(clientJsonData.carModelAutoTest.L_MIN);
                        carModelAutoTest.L_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTest.L_MAX);
                        carModelAutoTest.K_SVOB = Convert.ToDecimal(clientJsonData.carModelAutoTest.K_SVOB);
                        carModelAutoTest.K_MAX = Convert.ToDecimal(clientJsonData.carModelAutoTest.K_MAX);
                        carModelAutoTest.CarPostId = carPostId;
                        carModelAutoTest.ParadoxId = clientJsonData.carModelAutoTest.ID;

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
                            string execute = $"INSERT INTO public.\"CarModelAutoTest\"(\"CarPostId\", \"Name\", \"TypeEcoClassId\", \"Category\", \"EngineType\", \"MIN_TAH\", " +
                                $"\"DEL_MIN\", \"MAX_TAH\", \"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\", \"ParadoxId\")" +
                                    $"VALUES({carModelAutoTest.CarPostId.ToString()}," +
                                    $"'{carModelAutoTest.Name}'," +
                                    $"{carModelAutoTest.TypeEcoClassId.ToString()}," +
                                    $"'{carModelAutoTest.Category}'," +
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
                                    $"{(carModelAutoTest.ParadoxId != null ? carModelAutoTest.ParadoxId.ToString() : "null")}); ";
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
                    Tester tester = null;
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        connection.Open();
                        var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                            $"FROM public.\"CarModelSmokeMeter\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"ParadoxId\" = '{clientJsonData.carPostDataSmokeMeter.ID_MODEL}' " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelSmokeMeter = carModelSmokeMetersv.FirstOrDefault();
                        ///////////////
                        if (!string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo.TesterName))
                        {
                            var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                                $"FROM public.\"Tester\" as tester " +
                                $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                                $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                                $"WHERE model.\"CarPostId\" = {carPostId} " +
                                $"ORDER BY tester.\"Id\" DESC", commandTimeout: 86400);
                            tester = testersv.FirstOrDefault();
                        }
                        connection.Close();
                    }

                    if (carModelSmokeMeter != null)
                    {
                        CarPostDataSmokeMeter carPostDataSmokeMeter = new CarPostDataSmokeMeter();
                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter started{Environment.NewLine}");

                            carPostDataSmokeMeter.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataSmokeMeter.DATA.ToShortDateString()} {clientJsonData.carPostDataSmokeMeter.TIME}");
                            carPostDataSmokeMeter.Number = clientJsonData.carPostDataSmokeMeter.NOMER;
                            carPostDataSmokeMeter.DOPOL1 = clientJsonData.carPostDataSmokeMeter.DOPOL1;
                            carPostDataSmokeMeter.DOPOL2 = clientJsonData.carPostDataSmokeMeter.DOPOL2;
                            carPostDataSmokeMeter.MIN_TAH = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_TAH);
                            carPostDataSmokeMeter.MIN_CO = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_CO);
                            carPostDataSmokeMeter.MIN_CH = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_CH);
                            carPostDataSmokeMeter.MIN_CO2 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_CO2);
                            carPostDataSmokeMeter.MIN_O2 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_O2);
                            carPostDataSmokeMeter.MIN_L = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_L);
                            carPostDataSmokeMeter.MAX_TAH = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_TAH);
                            carPostDataSmokeMeter.MAX_CO = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_CO);
                            carPostDataSmokeMeter.MAX_CH = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_CH);
                            carPostDataSmokeMeter.MAX_CO2 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_CO2);
                            carPostDataSmokeMeter.MAX_O2 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_O2);
                            carPostDataSmokeMeter.MAX_L = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_L);
                            carPostDataSmokeMeter.ZAV_NOMER = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.ZAV_NOMER);
                            carPostDataSmokeMeter.K_1 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_1);
                            carPostDataSmokeMeter.K_2 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_2);
                            carPostDataSmokeMeter.K_3 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_3);
                            carPostDataSmokeMeter.K_4 = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_4);
                            carPostDataSmokeMeter.K_SVOB = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_SBOB);
                            carPostDataSmokeMeter.K_MAX = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.K_MAX);
                            carPostDataSmokeMeter.MIN_NO = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MIN_NO);
                            carPostDataSmokeMeter.MAX_NO = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.MAX_NO);
                            carPostDataSmokeMeter.CarModelSmokeMeterId = carModelSmokeMeter.Id;
                            carPostDataSmokeMeter.Temperature = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo?.TEMP);
                            carPostDataSmokeMeter.Pressure = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo?.PRESS);
                            carPostDataSmokeMeter.GasSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo?.N_AUTOTEST);
                            carPostDataSmokeMeter.GasCheckDate = Convert.ToDateTime(clientJsonData.carPostDataSmokeMeter.DopInfo?.D_AUTOTEST);
                            carPostDataSmokeMeter.MeteoSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo?.N_METEO);
                            carPostDataSmokeMeter.MeteoCheckDate = Convert.ToDateTime(clientJsonData.carPostDataSmokeMeter.DopInfo?.D_METEO);
                            carPostDataSmokeMeter.TestNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo?.NUM_TEST);
                            carPostDataSmokeMeter.TesterId = tester?.Id;

                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error parse data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter started{Environment.NewLine}");
                            object id = null;
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                            {
                                connection.Open();

                                string execute = $"INSERT INTO public.\"CarPostDataSmokeMeter\"(\"CarModelSmokeMeterId\", \"DateTime\", \"Number\", \"DOPOL1\", \"DOPOL2\", \"MIN_TAH\", " +
                                           $"\"MIN_CO\", \"MIN_CH\", \"MIN_CO2\", \"MIN_O2\", \"MIN_L\", \"MAX_TAH\", \"MAX_CO\", \"MAX_CH\", \"MAX_CO2\", \"MAX_O2\", \"MAX_L\", \"ZAV_NOMER\", " +
                                           $"\"K_1\", \"K_2\", \"K_3\", \"K_4\", \"K_SVOB\", \"K_MAX\", \"MIN_NO\", \"MAX_NO\", \"Temperature\", \"Pressure\", \"GasSerialNumber\", \"GasCheckDate\", " +
                                           $"\"MeteoSerialNumber\", \"MeteoCheckDate\", \"TestNumber\", \"TesterId\")" +
                                           $"VALUES({carPostDataSmokeMeter.CarModelSmokeMeterId.ToString()}," +
                                           $"make_timestamptz(" +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Year.ToString()}, " +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Month.ToString()}, " +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Day.ToString()}, " +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Hour.ToString()}, " +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Minute.ToString()}, " +
                                               $"{carPostDataSmokeMeter.DateTime.Value.Second.ToString()})," +
                                           $"'{carPostDataSmokeMeter.Number}'," +
                                           $"'{carPostDataSmokeMeter.DOPOL1}'," +
                                           $"'{carPostDataSmokeMeter.DOPOL2}'," +
                                           $"{carPostDataSmokeMeter.MIN_TAH.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_CO.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_CH.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_CO2.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_O2.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_L.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_TAH.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_CO.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_CH.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_CO2.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_O2.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_L.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.ZAV_NOMER.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_1.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_2.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_3.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_4.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_SVOB.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.K_MAX.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MIN_NO.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.MAX_NO.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.Temperature.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.Pressure.ToString().Replace(",", ".")}," +
                                           $"{carPostDataSmokeMeter.GasSerialNumber.ToString().Replace(",", ".")}," +
                                           $"make_timestamptz(" +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Year.ToString()}, " +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Month.ToString()}, " +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Day.ToString()}, " +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Hour.ToString()}, " +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Minute.ToString()}, " +
                                               $"{carPostDataSmokeMeter.GasCheckDate.Value.Second.ToString()})," +
                                           $"{carPostDataSmokeMeter.MeteoSerialNumber.ToString().Replace(",", ".")}," +
                                           $"make_timestamptz(" +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Year.ToString()}, " +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Month.ToString()}, " +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Day.ToString()}, " +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Hour.ToString()}, " +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Minute.ToString()}, " +
                                               $"{carPostDataSmokeMeter.MeteoCheckDate.Value.Second.ToString()}), " +
                                           $"{carPostDataSmokeMeter.TestNumber.ToString().Replace(",", ".")}, " +
                                           $"{(carPostDataSmokeMeter.TesterId != null ? carPostDataSmokeMeter.TesterId.ToString() : "null")}) RETURNING \"Id\";";
                                
                                id = connection.ExecuteScalar(execute);
                                if (tester == null && id != null && !string.IsNullOrEmpty(clientJsonData.carPostDataSmokeMeter.DopInfo.TesterName))
                                {
                                    object testerId = null;
                                    string executeTester = $"INSERT INTO public.\"Tester\"(\"Name\")" +
                                            $"VALUES('{clientJsonData.carPostDataSmokeMeter.DopInfo.TesterName.ToString()}') RETURNING \"Id\";";
                                    testerId = connection.ExecuteScalar(executeTester);
                                    if (testerId != null)
                                    {
                                        connection.Execute($"UPDATE public.\"CarPostDataSmokeMeter\" SET \"TesterId\" = {testerId} WHERE \"Id\" = {id};", commandTimeout: 86400);
                                    }
                                }
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
                if (clientJsonData.carPostDataAutoTest != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    Tester tester = null;
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                            $"FROM public.\"CarModelAutoTest\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"ParadoxId\" = '{clientJsonData.carPostDataAutoTest.ID_MODEL}' " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelAutoTest = carModelAutoTestsv.FirstOrDefault();

                        if (!string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo.TesterName))
                        {
                            var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                                $"FROM public.\"Tester\" as tester " +
                                $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                                $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                                $"WHERE model.\"CarPostId\" = {carPostId} " +
                                $"ORDER BY tester.\"Id\" DESC", commandTimeout: 86400);
                            tester = testersv.FirstOrDefault();
                        }
                        connection.Close();
                    }

                    if (carModelAutoTest != null)
                    {
                        CarPostDataAutoTest carPostDataAutoTest = new CarPostDataAutoTest();
                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest started{Environment.NewLine}");

                            carPostDataAutoTest.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataAutoTest.DATA.ToShortDateString()} {clientJsonData.carPostDataAutoTest.TIME}");
                            carPostDataAutoTest.Number = clientJsonData.carPostDataAutoTest.NOMER;
                            carPostDataAutoTest.DOPOL1 = clientJsonData.carPostDataAutoTest.DOPOL1;
                            carPostDataAutoTest.DOPOL2 = clientJsonData.carPostDataAutoTest.DOPOL2;
                            carPostDataAutoTest.MIN_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_TAH);
                            carPostDataAutoTest.MIN_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_CO);
                            carPostDataAutoTest.MIN_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_CH);
                            carPostDataAutoTest.MIN_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_CO2);
                            carPostDataAutoTest.MIN_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_O2);
                            carPostDataAutoTest.MIN_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_L);
                            carPostDataAutoTest.MAX_TAH = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_TAH);
                            carPostDataAutoTest.MAX_CO = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_CO);
                            carPostDataAutoTest.MAX_CH = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_CH);
                            carPostDataAutoTest.MAX_CO2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_CO2);
                            carPostDataAutoTest.MAX_O2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_O2);
                            carPostDataAutoTest.MAX_L = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_L);
                            carPostDataAutoTest.ZAV_NOMER = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.ZAV_NOMER);
                            carPostDataAutoTest.K_1 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_1);
                            carPostDataAutoTest.K_2 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_2);
                            carPostDataAutoTest.K_3 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_3);
                            carPostDataAutoTest.K_4 = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_4);
                            carPostDataAutoTest.K_SVOB = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_SBOB);
                            carPostDataAutoTest.K_MAX = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.K_MAX);
                            carPostDataAutoTest.MIN_NO = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MIN_NO);
                            carPostDataAutoTest.MAX_NO = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.MAX_NO);
                            carPostDataAutoTest.CarModelAutoTestId = carModelAutoTest.Id;
                            carPostDataAutoTest.Temperature = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.TEMP);
                            carPostDataAutoTest.Pressure = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.PRESS);
                            carPostDataAutoTest.GasSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.N_AUTOTEST);
                            carPostDataAutoTest.GasCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo?.D_AUTOTEST);
                            carPostDataAutoTest.MeteoSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.N_METEO);
                            carPostDataAutoTest.MeteoCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo?.D_METEO);
                            carPostDataAutoTest.TestNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.NUM_TEST);
                            carPostDataAutoTest.TesterId = tester?.Id;

                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error parse data autotest >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotest started{Environment.NewLine}");
                            object id = null;
                            using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                            {
                                connection.Open();
                                string execute = $"INSERT INTO public.\"CarPostDataAutoTest\"(\"CarModelAutoTestId\", \"DateTime\", \"Number\", \"DOPOL1\", \"DOPOL2\", \"MIN_TAH\", " +
                                            $"\"MIN_CO\", \"MIN_CH\", \"MIN_CO2\", \"MIN_O2\", \"MIN_L\", \"MAX_TAH\", \"MAX_CO\", \"MAX_CH\", \"MAX_CO2\", \"MAX_O2\", \"MAX_L\", \"ZAV_NOMER\", " +
                                            $"\"K_1\", \"K_2\", \"K_3\", \"K_4\", \"K_SVOB\", \"K_MAX\", \"MIN_NO\", \"MAX_NO\", \"Temperature\", \"Pressure\", \"GasSerialNumber\", \"GasCheckDate\", " +
                                            $"\"MeteoSerialNumber\", \"MeteoCheckDate\", \"TestNumber\", \"TesterId\")" +
                                            $"VALUES({carPostDataAutoTest.CarModelAutoTestId.ToString()}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataAutoTest.DateTime.Value.Year.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Value.Month.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Value.Day.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Value.Hour.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Value.Minute.ToString()}, " +
                                                $"{carPostDataAutoTest.DateTime.Value.Second.ToString()})," +
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
                                            $"{carPostDataAutoTest.Temperature.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.Pressure.ToString().Replace(",", ".")}," +
                                            $"{carPostDataAutoTest.GasSerialNumber.ToString().Replace(",", ".")}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Year.ToString()}, " +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Month.ToString()}, " +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Day.ToString()}, " +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Hour.ToString()}, " +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Minute.ToString()}, " +
                                                $"{carPostDataAutoTest.GasCheckDate.Value.Second.ToString()})," +
                                            $"{carPostDataAutoTest.MeteoSerialNumber.ToString().Replace(",", ".")}," +
                                            $"make_timestamptz(" +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Year.ToString()}, " +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Month.ToString()}, " +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Day.ToString()}, " +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Hour.ToString()}, " +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Minute.ToString()}, " +
                                                $"{carPostDataAutoTest.MeteoCheckDate.Value.Second.ToString()}), " +
                                            $"{carPostDataAutoTest.TestNumber.ToString().Replace(",", ".")}, " +
                                            $"{(carPostDataAutoTest.TesterId != null ? carPostDataAutoTest.TesterId.ToString() : "null")}) RETURNING \"Id\";";

                                id = connection.ExecuteScalar(execute);
                                if (tester == null && id != null && !string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo.TesterName))
                                {
                                    object testerId = null;
                                    string executeTester = $"INSERT INTO public.\"Tester\"(\"Name\")" +
                                            $"VALUES('{clientJsonData.carPostDataAutoTest.DopInfo.TesterName.ToString()}') RETURNING \"Id\";";
                                    testerId = connection.ExecuteScalar(executeTester);
                                    if (testerId != null)
                                    {
                                        connection.Execute($"UPDATE public.\"CarPostDataAutoTest\" SET \"TesterId\" = {testerId} WHERE \"Id\" = {id};", commandTimeout: 86400);
                                    }
                                }
                                connection.Close();
                            }
                            Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotest finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} >> Error insert data autotest >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} >> Model for data autotest does not exist in DB. Data: {clientJsonData.carPostDataAutoTest}{Environment.NewLine}");
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

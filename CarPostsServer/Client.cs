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
                byte[] data = new byte[256];
                string CarPostId = null;
                int carPostId = -1;
                while (true)
                {
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

                if (clientJsonData.carModelSmokeMeters.Count != 0)
                {
                    List<CarModelSmokeMeter> carModelSmokeMeters = new List<CarModelSmokeMeter>();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter started{Environment.NewLine}");

                        carModelSmokeMeters = clientJsonData.carModelSmokeMeters
                            .Select(c => new CarModelSmokeMeter
                            {
                                Name = c.MODEL,
                                Boost = c.NADDUV,
                                DFreeMark = Convert.ToDecimal(c.D_FREE),
                                DMaxMark = Convert.ToDecimal(c.D_MAX),
                                CarPostId = carPostId
                            })
                            .ToList();

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter finished. Count: {carModelSmokeMeters.Count}{Environment.NewLine}");
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
                            foreach (var carModelSmokeMeter in carModelSmokeMeters)
                            {
                                string execute = $"INSERT INTO public.\"CarModelSmokeMeter\"(\"CarPostId\", \"Name\", \"Boost\", \"DFreeMark\", \"DMaxMark\")" +
                                    $"VALUES({carModelSmokeMeter.CarPostId.ToString()}," +
                                    $"'{carModelSmokeMeter.Name}'," +
                                    $"{carModelSmokeMeter.Boost.ToString()}," +
                                    $"{carModelSmokeMeter.DFreeMark.ToString().Replace(",", ".")}," +
                                    $"{carModelSmokeMeter.DMaxMark.ToString().Replace(",", ".")});";
                                connection.Execute(execute);
                            }
                            connection.Close();
                        }
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error insert model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carModelAutoTests.Count != 0)
                {
                    List<CarModelAutoTest> carModelAutoTests = new List<CarModelAutoTest>();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

                        carModelAutoTests = clientJsonData.carModelAutoTests
                            .Select(c => new CarModelAutoTest
                            {
                                Name = c.MODEL,
                                EngineType = c.DVIG,
                                MIN_TAH = Convert.ToDecimal(c.MIN_TAH),
                                DEL_MIN = Convert.ToDecimal(c.DEL_MIN),
                                MAX_TAH = Convert.ToDecimal(c.MAX_TAH),
                                DEL_MAX = Convert.ToDecimal(c.DEL_MAX),
                                MIN_CO = Convert.ToDecimal(c.MIN_CO),
                                MAX_CO = Convert.ToDecimal(c.MAX_CO),
                                MIN_CH = Convert.ToDecimal(c.MIN_CH),
                                MAX_CH = Convert.ToDecimal(c.MAX_CH),
                                L_MIN = Convert.ToDecimal(c.L_MIN),
                                L_MAX = Convert.ToDecimal(c.L_MAX),
                                K_SVOB = Convert.ToDecimal(c.K_SVOB),
                                K_MAX = Convert.ToDecimal(c.K_MAX),
                                CarPostId = carPostId
                            })
                            .ToList();

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest finished. Count: {carModelAutoTests.Count}{Environment.NewLine}");
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
                            foreach (var carModelAutoTest in carModelAutoTests)
                            {
                                string execute = $"INSERT INTO public.\"CarModelAutoTest\"(\"CarPostId\", \"Name\", \"EngineType\", \"MIN_TAH\", \"DEL_MIN\", \"MAX_TAH\", " +
                                    $"\"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\")" +
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
                                    $"{carModelAutoTest.K_MAX.ToString().Replace(",", ".")});";
                                connection.Execute(execute);
                            }
                            connection.Close();
                        }
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error insert model autotest >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataSmokeMeters.Count != 0 || clientJsonData.carPostDataAutoTests.Count != 0)
                {
                    var dataSmokeMeterModels = clientJsonData.carPostDataSmokeMeters
                        .Select(m => new { m.MODEL })
                        .Distinct()
                        .ToList();
                    var wheredataSmokeMeters = String.Empty;
                    foreach (var dataSmokeMeterModel in dataSmokeMeterModels)
                    {
                        wheredataSmokeMeters += $" \"Name\" = '{dataSmokeMeterModel.MODEL}' OR ";
                    }
                    wheredataSmokeMeters = wheredataSmokeMeters[0..^4];

                    var dataAutoTestModels = clientJsonData.carPostDataAutoTests
                        .Select(m => new { m.MODEL })
                        .Distinct()
                        .ToList();
                    var wheredataAutoTests = String.Empty;
                    foreach (var dataAutoTestModel in dataAutoTestModels)
                    {
                        wheredataAutoTests += $" \"Name\" = '{dataAutoTestModel.MODEL}' OR ";
                    }
                    wheredataAutoTests = wheredataAutoTests[0..^4];

                    List<CarModelSmokeMeter> carModelSmokeMeters = new List<CarModelSmokeMeter>();
                    List<CarModelAutoTest> carModelAutoTests = new List<CarModelAutoTest>();
                    using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                    {
                        connection.Open();
                        var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelSmokeMeter\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND ({wheredataSmokeMeters}) " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelSmokeMeters = carModelSmokeMetersv.ToList();

                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelAutoTest\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND ({wheredataAutoTests}) " +
                            $"ORDER BY \"Id\"", commandTimeout: 86400);
                        carModelAutoTests = carModelAutoTestsv.ToList();
                        connection.Close();
                    }

                    List<CarPostDataSmokeMeter> carPostDataSmokeMeters = new List<CarPostDataSmokeMeter>();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter started{Environment.NewLine}");

                        carPostDataSmokeMeters = clientJsonData.carPostDataSmokeMeters
                            .Select(c => new CarPostDataSmokeMeter
                            {
                                DateTime = Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}"),
                                Number = c.NOMER,
                                RunIn = c.TYPE,
                                DFree = Convert.ToDecimal(c.D_FREE),
                                DMax = Convert.ToDecimal(c.D_MAX),
                                NDFree = Convert.ToDecimal(c.N_D_FREE),
                                NDMax = Convert.ToDecimal(c.N_D_MAX),
                                CarModelSmokeMeterId = carModelSmokeMeters.Where(x => x.Name == c.MODEL).FirstOrDefault()?.Id
                            })
                            .ToList();

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter finished. Count: {carPostDataSmokeMeters.Count}{Environment.NewLine}");
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
                            foreach (var carPostDataSmokeMeter in carPostDataSmokeMeters)
                            {
                                if (carPostDataSmokeMeter.CarModelSmokeMeterId != null)
                                {
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

                    List<CarPostDataAutoTest> carPostDataAutoTests = new List<CarPostDataAutoTest>();
                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest started{Environment.NewLine}");

                        carPostDataAutoTests = clientJsonData.carPostDataAutoTests
                            .Select(c => new CarPostDataAutoTest
                            {
                                DateTime = Convert.ToDateTime($"{c.DATA.ToShortDateString()} {c.TIME}"),
                                Number = c.NOMER,
                                DOPOL1 = c.DOPOL1,
                                DOPOL2 = c.DOPOL2,
                                MIN_TAH = Convert.ToDecimal(c.MIN_TAH),
                                MIN_CO = Convert.ToDecimal(c.MIN_CO),
                                MIN_CH = Convert.ToDecimal(c.MIN_CH),
                                MIN_CO2 = Convert.ToDecimal(c.MIN_CO2),
                                MIN_O2 = Convert.ToDecimal(c.MIN_O2),
                                MIN_L = Convert.ToDecimal(c.MIN_L),
                                MAX_TAH = Convert.ToDecimal(c.MAX_TAH),
                                MAX_CO = Convert.ToDecimal(c.MAX_CO),
                                MAX_CH = Convert.ToDecimal(c.MAX_CH),
                                MAX_CO2 = Convert.ToDecimal(c.MAX_CO2),
                                MAX_O2 = Convert.ToDecimal(c.MAX_O2),
                                MAX_L = Convert.ToDecimal(c.MAX_L),
                                ZAV_NOMER = Convert.ToDecimal(c.ZAV_NOMER),
                                K_1 = Convert.ToDecimal(c.K_1),
                                K_2 = Convert.ToDecimal(c.K_2),
                                K_3 = Convert.ToDecimal(c.K_3),
                                K_4 = Convert.ToDecimal(c.K_4),
                                K_SVOB = Convert.ToDecimal(c.K_SBOB),
                                K_MAX = Convert.ToDecimal(c.K_MAX),
                                MIN_NO = Convert.ToDecimal(c.MIN_NO),
                                MAX_NO = Convert.ToDecimal(c.MAX_NO),
                                CarModelAutoTestId = carModelAutoTests.Where(x => x.Name == c.MODEL).FirstOrDefault()?.Id
                            })
                            .ToList();

                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest finished. Count: {carPostDataAutoTests.Count}{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now} >> Error parse data autotest >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotest started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection("Host=localhost;Database=SmartEcoA;Username=postgres;Password=postgres;Port=5433"))
                        {
                            connection.Open();
                            foreach (var carPostDataAutoTest in carPostDataAutoTests)
                            {
                                if (carPostDataAutoTest.CarModelAutoTestId != null)
                                {
                                    string execute = $"INSERT INTO public.\"CarPostDataAutoTest\"(\"CarModelAutoTestId\", \"DateTime\", \"Number\", \"DOPOL1\", \"DOPOL2\", \"MIN_TAH\", " +
                                        $"\"MIN_CO\", \"MIN_CH\", \"MIN_CO2\", \"MIN_O2\", \"MIN_L\", \"MAX_TAH\", \"MAX_CO\", \"MAX_CH\", \"MAX_CO2\", \"MAX_O2\", \"MAX_L\", \"ZAV_NOMER\", " +
                                        $"\"K_1\", \"K_2\", \"K_3\", \"K_4\", \"K_SVOB\", \"K_MAX\", \"MIN_NO\", \"MAX_NO\")" +
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
                                        $"{carPostDataAutoTest.MAX_NO.ToString().Replace(",", ".")});";
                                    connection.Execute(execute);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} >> Error parse data >> {ex.Message}{Environment.NewLine}");
            }
        }
    }
}

using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    class CarPostClient
    {
        private readonly Logger Logger;
        private readonly TextBox TextBoxLog;
        private readonly ListView ListViewCarPosts;
        private readonly List<CarPost> CarPosts;
        private readonly string SmartEcoAConnectionString;

        public CarPostClient(TextBox TextBoxLog,
            ListView ListView,
            List<CarPost> CarPosts,
            string SmartEcoAConnectionString)
        {
            this.TextBoxLog = TextBoxLog;
            Logger = new Logger(this.TextBoxLog);
            ListViewCarPosts = ListView;
            this.CarPosts = CarPosts;
            this.SmartEcoAConnectionString = SmartEcoAConnectionString;
        }

        public void Process(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = null;
            NetworkStream stream = null;
            string ip = "",
                carPostId = "";
            int carPostIdInt = -1;
            StringBuilder messageSB = new StringBuilder();
            try
            {
                client = listener.EndAcceptTcpClient(ar);
                ip = (client.Client.RemoteEndPoint as IPEndPoint as IPEndPoint).Address.ToString();
                //Logger.Log($"Подключился пост ({ip})");
                stream = client.GetStream();
                // Get CarPostId
                //StringBuilder messageSB = new StringBuilder();
                int bytes = 0;
                byte[] data = new byte[client.ReceiveBufferSize];
                while (true)
                {
                    bool completeData = false;
                    //if (client.Available == 0 && !string.IsNullOrEmpty(carPostId))
                    //{
                    //    Logger.Log($"{carPostId}: нет новых данных");
                    //    completeData = true;
                    //    break;
                    //}
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        messageSB.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        // maybe came complete data?
                        if (messageSB.ToString().Contains(Environment.NewLine))
                        {
                            string bufer = messageSB.ToString();
                            bufer = bufer.Replace(Environment.NewLine, "\t");
                            if (bufer.Split('\t').Length == 3 && bufer.Last() == '\t')
                            {
                                messageSB = new StringBuilder(bufer.Split('\t')[1]);
                                //Logger.Log($"{carPostId}: данные получены: {messageSB.ToString()}");
                                if (ParseData(messageSB.ToString(), carPostIdInt) == 0)
                                {
                                    completeData = true;
                                    UpdateListView(carPostId, ip);
                                    break;
                                }
                                completeData = true;
                                UpdateListView(carPostId, ip);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    while (stream.DataAvailable);
                    string jsonString = messageSB.ToString();
                    dynamic obj = JsonConvert.DeserializeObject(jsonString);
                    if (!string.IsNullOrEmpty((string)obj.CarPostId))
                    {
                        carPostId = (string)obj.CarPostId;
                    }
                    // Send message with last data
                    if (!string.IsNullOrEmpty((string)obj.CarPostId))
                    {
                        Logger.Log($"{carPostId}: подключился с IP {ip}");
                        UpdateListView(carPostId, ip);
                        if (!string.IsNullOrEmpty(carPostId) && Int32.TryParse(carPostId, out carPostIdInt))
                        {
                            string messageS = GetLastData(carPostIdInt);

                            // Send message with last data
                            data = Encoding.UTF8.GetBytes(messageS);
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            dynamic err = new ExpandoObject();
                            err.Error = "CarPostId не может быть символьным или пустым!";
                            string message = JsonConvert.SerializeObject(err);

                            // Send error
                            data = Encoding.UTF8.GetBytes(message);
                            stream.Write(data, 0, data.Length);
                        }
                        UpdateListView(carPostId, ip);
                    }
                    if (completeData)
                    {
                        break;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.Log($"Связь оборвалась с {ip}: {ex.Message}\n Данные: {messageSB.ToString()}");
            }
            finally
            {
                stream?.Close();
                client?.Close();
                if(!string.IsNullOrEmpty(carPostId))
                {
                    Logger.Log($"{carPostId}: отключился");
                }
            }
        }

        private void UpdateListView(
            string Id,
            string IP)
        {
            string Name = CarPosts.FirstOrDefault(c => c.Id.ToString() == Id)?.Name;
            ListViewItem listViewItem = null;
            ListViewCarPosts.Invoke(new MethodInvoker(delegate
            {
                listViewItem = ListViewCarPosts.Items.Cast<ListViewItem>().FirstOrDefault(l => l.Text == Id);
                if (listViewItem == null)
                {
                    string[] row = { Id, Name, IP, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                    listViewItem = new ListViewItem(row);
                    ListViewCarPosts.Items.Add(listViewItem);
                }
                else
                {
                    listViewItem.SubItems[3].Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }));
        }

        private string GetLastData(int carPostId)
        {
            DateTime versionDbAutotest;
            DateTime versionDbSmokemeter;
            int? carModelSmokeMeterId = null;
            int? carModelAutoTestId = null;
            //    carModelAutoTestCount = null;
            DateTime? carPostDataAutoTestDate = new DateTime();
            DateTime? carPostDataSmokeMeterDate = new DateTime();
            try
            {
                using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                {
                    connection.Open();
                    (versionDbAutotest, versionDbSmokemeter) = GetVersionDb(connection, carPostId);

                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                        $"FROM public.\"CarModelSmokeMeter\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1");
                    carModelSmokeMeterId = carModelSmokeMetersv.FirstOrDefault() == null ? carModelSmokeMeterId : carModelSmokeMetersv.FirstOrDefault().ParadoxId;

                    var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                        $"FROM public.\"CarModelAutoTest\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1");
                    carModelAutoTestId = carModelAutoTestsv.FirstOrDefault() == null ? carModelAutoTestId : carModelAutoTestsv.FirstOrDefault().ParadoxId;
                    //if (carModelAutoTestId != null)
                    //{
                    //    var carModelAutoTestCountv = connection.Query<int>($"SELECT COUNT(*) " +
                    //        $"FROM public.\"CarModelAutoTest\" " +
                    //        $"WHERE \"CarPostId\" = {carPostId} " +
                    //        $"AND \"ParadoxId\" = {carModelAutoTestId.Value}");
                    //    carModelAutoTestCount = carModelAutoTestCountv.FirstOrDefault();
                    //}

                    var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>($"SELECT * " +
                        $"FROM public.\"CarPostDataAutoTest\" as datas " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = datas.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1");
                    carPostDataAutoTestDate = carPostDataAutoTestsv.FirstOrDefault() == null ? carPostDataAutoTestDate : carPostDataAutoTestsv.FirstOrDefault().DateTime;

                    var carPostDataSmokeMetersv = connection.Query<CarPostDataSmokeMeter>($"SELECT * " +
                        $"FROM public.\"CarPostDataSmokeMeter\" as datas " +
                        $"JOIN public.\"CarModelSmokeMeter\" as model ON model.\"Id\" = datas.\"CarModelSmokeMeterId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1");
                    carPostDataSmokeMeterDate = carPostDataSmokeMetersv.FirstOrDefault() == null ? carPostDataSmokeMeterDate : carPostDataSmokeMetersv.FirstOrDefault().DateTime;
                    connection.Close();
                }

                dynamic obj = new ExpandoObject();
                obj.versionDbAutotest = versionDbAutotest.ToString("yyyy-MM-dd HH:mm:ss");
                obj.versionDbSmokemeter = versionDbSmokemeter.ToString("yyyy-MM-dd HH:mm:ss");
                obj.carModelSmokeMeterId = carModelSmokeMeterId;
                obj.carModelAutoTestId = carModelAutoTestId;
                //obj.carModelAutoTestCount = carModelAutoTestCount;
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

        private (DateTime, DateTime) GetVersionDb(NpgsqlConnection connection, int carPostId)
        {
            DateTime versionDbAutotest;
            DateTime versionDbSmokemeter;

            var carPostVersionDbAutotestv = connection.Query<CarPostVersionDb>($"SELECT \"Id\", \"NameDb\", \"Version\", \"CarPostId\" " +
                        $"FROM public.\"CarPostVersionDb\" " +
                        $"WHERE \"CarPostId\" = {carPostId} AND \"NameDb\" = 'autotest'" +
                        $"LIMIT 1");
            var carPostVersionDbAutotest = carPostVersionDbAutotestv.SingleOrDefault();

            var carPostVersionDbSmokemeterv = connection.Query<CarPostVersionDb>($"SELECT \"Id\", \"NameDb\", \"Version\", \"CarPostId\" " +
                        $"FROM public.\"CarPostVersionDb\" " +
                        $"WHERE \"CarPostId\" = {carPostId} AND \"NameDb\" = 'smokemeter'" +
                        $"LIMIT 1");
            var carPostVersionDbSmokemeter = carPostVersionDbSmokemeterv.SingleOrDefault();

            if (carPostVersionDbAutotest is null)
            {
                var carPostDataAutoTestsv = connection.Query<CarPostDataAutoTest>($"SELECT * " +
                        $"FROM public.\"CarPostDataAutoTest\" as datas " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = datas.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY datas.\"DateTime\" DESC " +
                        $"LIMIT 1");
                var carPostDataAutoTestDate = carPostDataAutoTestsv.FirstOrDefault() == null ? null : carPostDataAutoTestsv.FirstOrDefault().DateTime;

                var version = new DateTime();
                if (carPostDataAutoTestDate != null)
                {
                    version = (DateTime)carPostDataAutoTestDate;
                }

                string execute = $"INSERT INTO public.\"CarPostVersionDb\"(\"CarPostId\", \"NameDb\", \"Version\")" +
                    $"VALUES({carPostId}," +
                    $"'autotest'," +
                    $"make_timestamptz(" +
                        $"{version.Year}, " +
                        $"{version.Month}, " +
                        $"{version.Day}, " +
                        $"{version.Hour}, " +
                        $"{version.Minute}, " +
                        $"{version.Second}));";
                connection.Execute(execute);
                versionDbAutotest = version;
            }
            else
            {
                versionDbAutotest = carPostVersionDbAutotest.Version;
            }

            if (carPostVersionDbSmokemeter is null)
            {
                var carPostDataSmokeMetersv = connection.Query<CarPostDataSmokeMeter>($"SELECT * " +
                    $"FROM public.\"CarPostDataSmokeMeter\" as datas " +
                    $"JOIN public.\"CarModelSmokeMeter\" as model ON model.\"Id\" = datas.\"CarModelSmokeMeterId\" " +
                    $"WHERE model.\"CarPostId\" = {carPostId} " +
                    $"ORDER BY datas.\"DateTime\" DESC " +
                    $"LIMIT 1");
                var carPostDataSmokeMeterDate = carPostDataSmokeMetersv.FirstOrDefault() == null ? null : carPostDataSmokeMetersv.FirstOrDefault().DateTime;

                var version = new DateTime();
                if (carPostDataSmokeMeterDate != null)
                {
                    version = (DateTime)carPostDataSmokeMeterDate;
                }

                string execute = $"INSERT INTO public.\"CarPostVersionDb\"(\"CarPostId\", \"NameDb\", \"Version\")" +
                    $"VALUES({carPostId}," +
                    $"'smokemeter'," +
                    $"make_timestamptz(" +
                        $"{version.Year}, " +
                        $"{version.Month}, " +
                        $"{version.Day}, " +
                        $"{version.Hour}, " +
                        $"{version.Minute}, " +
                        $"{version.Second}));";
                connection.Execute(execute);
                versionDbSmokemeter = version;
            }
            else
            {
                versionDbSmokemeter = carPostVersionDbSmokemeter.Version;
            }

            return (versionDbAutotest, versionDbSmokemeter);
        }

        private int ParseData(string jsonString, int carPostId)
        {
            try
            {
                var clientJsonData = JsonConvert.DeserializeObject<ClientJsonData>(jsonString);

                //Console.WriteLine($"{DateTime.Now} >> Get data from CarPost {carPostId}{Environment.NewLine}");
                Logger.Log($"{carPostId}: получение данных...");
                if (!string.IsNullOrEmpty(clientJsonData.VersionDbAutotest))
                {
                    var version = Convert.ToDateTime(clientJsonData.VersionDbAutotest);
                    using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                    {
                        connection.Execute($"UPDATE public.\"CarPostVersionDb\" " +
                            $"SET \"Version\" = make_timestamptz(" +
                                $"{version.Year}, " +
                                $"{version.Month}, " +
                                $"{version.Day}, " +
                                $"{version.Hour}, " +
                                $"{version.Minute}, " +
                                $"{version.Second}) " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"NameDb\" = 'autotest';");
                    }
                }
                if (!string.IsNullOrEmpty(clientJsonData.VersionDbSmokemeter))
                {
                    var version = Convert.ToDateTime(clientJsonData.VersionDbSmokemeter);
                    using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                    {
                        connection.Execute($"UPDATE public.\"CarPostVersionDb\" " +
                            $"SET \"Version\" = make_timestamptz(" +
                                $"{version.Year}, " +
                                $"{version.Month}, " +
                                $"{version.Day}, " +
                                $"{version.Hour}, " +
                                $"{version.Minute}, " +
                                $"{version.Second}) " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"NameDb\" = 'smokemeter';");
                    }
                }
                if (clientJsonData.carModelSmokeMeter != null)
                {
                    TypeEcoClass typeEcoClass = new TypeEcoClass();
                    if (!string.IsNullOrEmpty(clientJsonData.carModelSmokeMeter.TypeEcoName))
                    {
                        using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                        {
                            connection.Open();
                            var typeEcoClassv = connection.Query<TypeEcoClass>($"SELECT \"Id\", \"Name\" " +
                                $"FROM public.\"TypeEcoClass\" " +
                                $"WHERE \"Name\" = '{clientJsonData.carModelSmokeMeter.TypeEcoName}' " +
                                $"ORDER BY \"Id\"");
                            typeEcoClass = typeEcoClassv.FirstOrDefault();
                        }
                    }

                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter started{Environment.NewLine}");

                        carModelSmokeMeter.Name = clientJsonData.carModelSmokeMeter.MODEL;
                        carModelSmokeMeter.TypeEcoClassId = typeEcoClass?.Id;
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

                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter finished{Environment.NewLine}");
                        Logger.Log($"{carPostId}: получены данные: Дымомер, модель автомобиля - {carModelSmokeMeter.Name}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"{carPostId}: Ошибка парсинга модели Дымомера: {jsonString}");
                        //Console.WriteLine($"{DateTime.Now} >> Error parse model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                        {
                            connection.Open();
                            string execute = $"INSERT INTO public.\"CarModelSmokeMeter\"(\"CarPostId\", \"Name\", \"TypeEcoClassId\", \"Category\", \"EngineType\", \"MIN_TAH\", " +
                                $"\"DEL_MIN\", \"MAX_TAH\", \"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\", \"ParadoxId\")" +
                                    $"VALUES({carModelSmokeMeter.CarPostId.ToString()}," +
                                    $"'{carModelSmokeMeter.Name.Replace("\'", "\'\'")}'," +   //Replace for special symbol '
                                    $"{(carModelSmokeMeter.TypeEcoClassId != null ? carModelSmokeMeter.TypeEcoClassId.ToString() : "null")}," +
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
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"{carPostId}: Ошибка добавления в БД модели Дымомера: {jsonString}");
                        //Console.WriteLine($"{DateTime.Now} >> Error insert model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carModelAutoTest != null)
                {
                    TypeEcoClass typeEcoClass = new TypeEcoClass();
                    if (!string.IsNullOrEmpty(clientJsonData.carModelAutoTest.TypeEcoName))
                    {
                        using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                        {
                            connection.Open();
                            var typeEcoClassv = connection.Query<TypeEcoClass>($"SELECT \"Id\", \"Name\" " +
                                $"FROM public.\"TypeEcoClass\" " +
                                $"WHERE \"Name\" = '{clientJsonData.carModelAutoTest.TypeEcoName.ToString()}' " +
                                $"ORDER BY \"Id\"");
                            typeEcoClass = typeEcoClassv.FirstOrDefault();
                        }
                    }

                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

                        carModelAutoTest.Name = clientJsonData.carModelAutoTest.MODEL;
                        carModelAutoTest.TypeEcoClassId = typeEcoClass?.Id;
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

                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest finished{Environment.NewLine}");
                        Logger.Log($"{carPostId}: получены данные: Автотест, модель автомобиля - {carModelAutoTest.Name}");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"{DateTime.Now} >> Error parse model autotest >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                        {
                            connection.Open();
                            string execute = $"INSERT INTO public.\"CarModelAutoTest\"(\"CarPostId\", \"Name\", \"TypeEcoClassId\", \"Category\", \"EngineType\", \"MIN_TAH\", " +
                                $"\"DEL_MIN\", \"MAX_TAH\", \"DEL_MAX\", \"MIN_CO\", \"MAX_CO\", \"MIN_CH\", \"MAX_CH\", \"L_MIN\", \"L_MAX\", \"K_SVOB\", \"K_MAX\", \"ParadoxId\")" +
                                    $"VALUES({carModelAutoTest.CarPostId.ToString()}," +
                                    $"'{carModelAutoTest.Name.Replace("\'", "\'\'")}'," +   //Replace for special symbol '
                                    $"{(carModelAutoTest.TypeEcoClassId != null ? carModelAutoTest.TypeEcoClassId.ToString() : "null")}," +
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
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model autotest finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"{DateTime.Now} >> Error insert model autotest >> {ex.Message}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataSmokeMeter != null)
                {
                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    Tester tester = null;
                    using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                    {
                        connection.Open();
                        var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                            $"FROM public.\"CarModelSmokeMeter\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"ParadoxId\" = '{clientJsonData.carPostDataSmokeMeter.ID_MODEL}' " +
                            $"ORDER BY \"Id\"");
                        carModelSmokeMeter = carModelSmokeMetersv.FirstOrDefault();

                        if (!string.IsNullOrEmpty(clientJsonData.carPostDataSmokeMeter.DopInfo?.TesterName))
                        {
                            var testersv = connection.Query<Tester>($"SELECT \"Id\", \"Name\"" +
                                $" FROM public.\"Tester\"" +
                                $" WHERE \"Name\" = '{clientJsonData.carPostDataSmokeMeter.DopInfo.TesterName}';");
                            tester = testersv.FirstOrDefault();
                        }
                        connection.Close();
                    }

                    if (carModelSmokeMeter != null)
                    {
                        CarPostDataSmokeMeter carPostDataSmokeMeter = new CarPostDataSmokeMeter();
                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter started{Environment.NewLine}");

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
                            if (clientJsonData.carPostDataSmokeMeter.DopInfo is null)
                            {
                                carPostDataSmokeMeter.Temperature = null;
                                carPostDataSmokeMeter.Pressure = null;
                                carPostDataSmokeMeter.GasSerialNumber = null;
                                carPostDataSmokeMeter.GasCheckDate = null;
                                carPostDataSmokeMeter.MeteoSerialNumber = null;
                                carPostDataSmokeMeter.MeteoCheckDate = null;
                                carPostDataSmokeMeter.TestNumber = null;
                            }
                            else
                            {
                                carPostDataSmokeMeter.Temperature = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo.TEMP);
                                carPostDataSmokeMeter.Pressure = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo.PRESS);
                                carPostDataSmokeMeter.GasSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo.N_AUTOTEST);
                                carPostDataSmokeMeter.GasCheckDate = Convert.ToDateTime(clientJsonData.carPostDataSmokeMeter.DopInfo.D_AUTOTEST);
                                carPostDataSmokeMeter.MeteoSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo.N_METEO);
                                carPostDataSmokeMeter.MeteoCheckDate = Convert.ToDateTime(clientJsonData.carPostDataSmokeMeter.DopInfo.D_METEO);
                                carPostDataSmokeMeter.TestNumber = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.DopInfo.NUM_TEST);
                            }
                            carPostDataSmokeMeter.TesterId = tester?.Id;

                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter finished{Environment.NewLine}");
                            Logger.Log($"{carPostId}: получены данные: Дымомер, измерение - {carPostDataSmokeMeter.DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"{carPostId}: Ошибка парсинга данных Дымомера: {jsonString}");
                            //Console.WriteLine($"{DateTime.Now} >> Error parse data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter started{Environment.NewLine}");
                            object id = null;
                            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
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
                                           $"{(carPostDataSmokeMeter.Temperature != null ? carPostDataSmokeMeter.Temperature.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataSmokeMeter.Pressure != null ? carPostDataSmokeMeter.Pressure.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataSmokeMeter.GasSerialNumber != null ? carPostDataSmokeMeter.GasSerialNumber.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataSmokeMeter.GasCheckDate == null ? "null" : $"make_timestamptz({carPostDataSmokeMeter.GasCheckDate.Value.Year}, {carPostDataSmokeMeter.GasCheckDate.Value.Month}, {carPostDataSmokeMeter.GasCheckDate.Value.Day}, {carPostDataSmokeMeter.GasCheckDate.Value.Hour}, {carPostDataSmokeMeter.GasCheckDate.Value.Minute}, {carPostDataSmokeMeter.GasCheckDate.Value.Second})")}," +
                                           $"{(carPostDataSmokeMeter.MeteoSerialNumber != null ? carPostDataSmokeMeter.MeteoSerialNumber.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataSmokeMeter.MeteoCheckDate == null ? "null" : $"make_timestamptz({carPostDataSmokeMeter.MeteoCheckDate.Value.Year}, {carPostDataSmokeMeter.MeteoCheckDate.Value.Month}, {carPostDataSmokeMeter.MeteoCheckDate.Value.Day}, {carPostDataSmokeMeter.MeteoCheckDate.Value.Hour}, {carPostDataSmokeMeter.MeteoCheckDate.Value.Minute}, {carPostDataSmokeMeter.MeteoCheckDate.Value.Second})")}," +
                                           $"{(carPostDataSmokeMeter.TestNumber != null ? carPostDataSmokeMeter.TestNumber.ToString().Replace(",", ".") : "null")}, " +
                                           $"{(carPostDataSmokeMeter.TesterId != null ? carPostDataSmokeMeter.TesterId.ToString() : "null")}) RETURNING \"Id\";";

                                id = connection.ExecuteScalar(execute);
                                if (tester == null && id != null && !string.IsNullOrEmpty(clientJsonData.carPostDataSmokeMeter.DopInfo?.TesterName))
                                {
                                    object testerId = null;
                                    string executeTester = $"INSERT INTO public.\"Tester\"(\"Name\")" +
                                            $"VALUES('{clientJsonData.carPostDataSmokeMeter.DopInfo.TesterName}') RETURNING \"Id\";";
                                    testerId = connection.ExecuteScalar(executeTester);
                                    if (testerId != null)
                                    {
                                        connection.Execute($"UPDATE public.\"CarPostDataSmokeMeter\" SET \"TesterId\" = {testerId} WHERE \"Id\" = {id};");
                                    }
                                }
                                connection.Close();
                            }
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"{carPostId}: Ошибка добавления в БД данных Дымомера: {jsonString}");
                            //Console.WriteLine($"{DateTime.Now} >> Error insert data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"{DateTime.Now} >> Model for data smokemeter does not exist in DB. Data: {clientJsonData.carPostDataSmokeMeter}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carPostDataAutoTest != null)
                {
                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    Tester tester = null;
                    using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                    {
                        var carModelAutoTestsv = connection.Query<CarModelAutoTest>($"SELECT \"Id\", \"Name\", \"ParadoxId\", \"CarPostId\" " +
                            $"FROM public.\"CarModelAutoTest\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"ParadoxId\" = '{clientJsonData.carPostDataAutoTest.ID_MODEL}' " +
                            $"ORDER BY \"Id\"");
                        carModelAutoTest = carModelAutoTestsv.FirstOrDefault();

                        if (!string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo?.TesterName))
                        {
                            //var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                            //    $"FROM public.\"Tester\" as tester " +
                            //    $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                            //    $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                            //    $"WHERE tester.\"Name\" = '{clientJsonData.carPostDataAutoTest.DopInfo.TesterName}' AND model.\"CarPostId\" = {carPostId} " +
                            //    $"ORDER BY tester.\"Id\" DESC");
                            var testersv = connection.Query<Tester>($"SELECT \"Id\", \"Name\"" +
                                $" FROM public.\"Tester\"" +
                                $" WHERE \"Name\" = '{clientJsonData.carPostDataAutoTest.DopInfo.TesterName}';");
                            tester = testersv.FirstOrDefault();
                        }
                        connection.Close();
                    }

                    if (carModelAutoTest != null)
                    {
                        CarPostDataAutoTest carPostDataAutoTest = new CarPostDataAutoTest();
                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest started{Environment.NewLine}");

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
                            if (clientJsonData.carPostDataAutoTest.DopInfo is null)
                            {
                                carPostDataAutoTest.Temperature = null;
                                carPostDataAutoTest.Pressure = null;
                                carPostDataAutoTest.GasSerialNumber = null;
                                carPostDataAutoTest.GasCheckDate = null;
                                carPostDataAutoTest.MeteoSerialNumber = null;
                                carPostDataAutoTest.MeteoCheckDate = null;
                                carPostDataAutoTest.TestNumber = null;
                            }
                            else
                            {
                                carPostDataAutoTest.Temperature = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo.TEMP);
                                carPostDataAutoTest.Pressure = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo.PRESS);
                                carPostDataAutoTest.GasSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo.N_AUTOTEST);
                                carPostDataAutoTest.GasCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo.D_AUTOTEST);
                                carPostDataAutoTest.MeteoSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo.N_METEO);
                                carPostDataAutoTest.MeteoCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo.D_METEO);
                                carPostDataAutoTest.TestNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo.NUM_TEST);
                            }
                            carPostDataAutoTest.TesterId = tester?.Id;

                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data autotest finished{Environment.NewLine}");
                            Logger.Log($"{carPostId}: получены данные: Автотест, измерение - {carPostDataAutoTest.DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"{DateTime.Now} >> Error parse data autotest >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotest started{Environment.NewLine}");
                            object id = null;
                            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
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
                                            $"{(carPostDataAutoTest.Temperature != null ? carPostDataAutoTest.Temperature.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataAutoTest.Pressure != null ? carPostDataAutoTest.Pressure.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataAutoTest.GasSerialNumber != null ? carPostDataAutoTest.GasSerialNumber.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataAutoTest.GasCheckDate == null ? "null" : $"make_timestamptz({carPostDataAutoTest.GasCheckDate.Value.Year}, {carPostDataAutoTest.GasCheckDate.Value.Month}, {carPostDataAutoTest.GasCheckDate.Value.Day}, {carPostDataAutoTest.GasCheckDate.Value.Hour}, {carPostDataAutoTest.GasCheckDate.Value.Minute}, {carPostDataAutoTest.GasCheckDate.Value.Second})")}," +
                                           $"{(carPostDataAutoTest.MeteoSerialNumber != null ? carPostDataAutoTest.MeteoSerialNumber.ToString().Replace(",", ".") : "null")}," +
                                           $"{(carPostDataAutoTest.MeteoCheckDate == null ? "null" : $"make_timestamptz({carPostDataAutoTest.MeteoCheckDate.Value.Year}, {carPostDataAutoTest.MeteoCheckDate.Value.Month}, {carPostDataAutoTest.MeteoCheckDate.Value.Day}, {carPostDataAutoTest.MeteoCheckDate.Value.Hour}, {carPostDataAutoTest.MeteoCheckDate.Value.Minute}, {carPostDataAutoTest.MeteoCheckDate.Value.Second})")}," +
                                           $"{(carPostDataAutoTest.TestNumber != null ? carPostDataAutoTest.TestNumber.ToString().Replace(",", ".") : "null")}, " +
                                           $"{(carPostDataAutoTest.TesterId != null ? carPostDataAutoTest.TesterId.ToString() : "null")}) RETURNING \"Id\";";

                                id = connection.ExecuteScalar(execute);
                                if (tester == null && id != null && !string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo?.TesterName))
                                {
                                    object testerId = null;
                                    string executeTester = $"INSERT INTO public.\"Tester\"(\"Name\")" +
                                            $"VALUES('{clientJsonData.carPostDataAutoTest.DopInfo.TesterName}') RETURNING \"Id\";";
                                    testerId = connection.ExecuteScalar(executeTester);
                                    if (testerId != null)
                                    {
                                        connection.Execute($"UPDATE public.\"CarPostDataAutoTest\" SET \"TesterId\" = {testerId} WHERE \"Id\" = {id};");
                                    }
                                }
                                connection.Close();
                            }
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data autotest finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"{DateTime.Now} >> Error insert data autotest >> {ex.Message}{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"{DateTime.Now} >> Model for data autotest does not exist in DB. Data: {clientJsonData.carPostDataAutoTest}{Environment.NewLine}");
                    }
                }
                if (clientJsonData.carModelSmokeMeter == null && clientJsonData.carModelAutoTest == null && clientJsonData.carPostDataSmokeMeter == null && clientJsonData.carPostDataAutoTest == null)
                {
                    Logger.Log($"{carPostId}: нет новых данных");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"{DateTime.Now} >> Error parse data >> {ex.Message}{Environment.NewLine}");
                Logger.Log($"{carPostId}: Ошибка данных: {jsonString}");
                return 0;
            }
            return 1;
        }

        private string ToTitleCase(string text)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var words = text.Split(' ');

            var newText = "";
            foreach (var word in words)
            {
                newText += char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant() + ' ';
            }
            return newText.Trim();
        }
    }
}
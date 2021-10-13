using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
            try
            {
                client = listener.EndAcceptTcpClient(ar);
                ip = (client.Client.RemoteEndPoint as IPEndPoint as IPEndPoint).Address.ToString();
                //Logger.Log($"Подключился пост ({ip})");
                stream = client.GetStream();
                // Get CarPostId
                StringBuilder messageSB = new StringBuilder();
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
                Logger.Log($"Связь оборвалась с {ip}: {ex.Message}");
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
            string carModelSmokeMeterName = string.Empty;
            int? carModelAutoTestId = null;
            //    carModelAutoTestCount = null;
            DateTime? carPostDataAutoTestDate = new DateTime();
            DateTime? carPostDataSmokeMeterDate = new DateTime();
            string testerName = string.Empty;
            try
            {
                using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                {
                    connection.Open();
                    var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                        $"FROM public.\"CarModelSmokeMeter\" " +
                        $"WHERE \"CarPostId\" = {carPostId} " +
                        $"ORDER BY \"Id\" DESC " +
                        $"LIMIT 1");
                    carModelSmokeMeterName = carModelSmokeMetersv.FirstOrDefault() == null ? carModelSmokeMeterName : carModelSmokeMetersv.FirstOrDefault().Name;

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

                    var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                        $"FROM public.\"Tester\" as tester " +
                        $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                        $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                        $"WHERE model.\"CarPostId\" = {carPostId} " +
                        $"ORDER BY tester.\"Id\" DESC " +
                        $"LIMIT 1");
                    testerName = testersv.FirstOrDefault() == null ? testerName : testersv.FirstOrDefault().Name;
                    connection.Close();
                }

                dynamic obj = new ExpandoObject();
                obj.carModelSmokeMeterName = carModelSmokeMeterName;
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

        private int ParseData(string jsonString, int carPostId)
        {
            try
            {
                var clientJsonData = JsonConvert.DeserializeObject<ClientJsonData>(jsonString);

                //Console.WriteLine($"{DateTime.Now} >> Get data from CarPost {carPostId}{Environment.NewLine}");
                Logger.Log($"{carPostId}: получение данных...");

                if (clientJsonData.carModelSmokeMeter != null)
                {
                    CarModelSmokeMeter carModelSmokeMeter = new CarModelSmokeMeter();
                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter started{Environment.NewLine}");

                        carModelSmokeMeter.Name = clientJsonData.carModelSmokeMeter.MODEL;
                        carModelSmokeMeter.Boost = clientJsonData.carModelSmokeMeter.NADDUV;
                        carModelSmokeMeter.DFreeMark = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.D_FREE);
                        carModelSmokeMeter.DMaxMark = Convert.ToDecimal(clientJsonData.carModelSmokeMeter.D_MAX);
                        carModelSmokeMeter.CarPostId = carPostId;

                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model smokemeter finished{Environment.NewLine}");
                        Logger.Log($"{carPostId}: получены данные: Дымомер, модель автомобиля - {carModelSmokeMeter.Name}");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"{DateTime.Now} >> Error parse model smokemeter >> {ex.Message}{Environment.NewLine}");
                    }

                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter started{Environment.NewLine}");
                        using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
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
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert model smokemeter finished{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
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
                                $"WHERE \"Id\" = '{clientJsonData.carModelAutoTest.ID_ECOLOG.ToString()}' " +
                                $"ORDER BY \"Id\"");
                            typeEcoClass = typeEcoClassv.FirstOrDefault();
                        }
                    }

                    CarModelAutoTest carModelAutoTest = new CarModelAutoTest();
                    try
                    {
                        //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get model autotest started{Environment.NewLine}");

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
                    using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
                    {
                        connection.Open();
                        var carModelSmokeMetersv = connection.Query<CarModelSmokeMeter>($"SELECT \"Id\", \"Name\", \"CarPostId\" " +
                            $"FROM public.\"CarModelSmokeMeter\" " +
                            $"WHERE \"CarPostId\" = {carPostId} AND \"Name\" = '{clientJsonData.carPostDataSmokeMeter.MODEL}' " +
                            $"ORDER BY \"Id\"");
                        carModelSmokeMeter = carModelSmokeMetersv.FirstOrDefault();
                    }

                    if (carModelSmokeMeter != null)
                    {
                        CarPostDataSmokeMeter carPostDataSmokeMeter = new CarPostDataSmokeMeter();
                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter started{Environment.NewLine}");

                            carPostDataSmokeMeter.DateTime = Convert.ToDateTime($"{clientJsonData.carPostDataSmokeMeter.DATA.ToShortDateString()} {clientJsonData.carPostDataSmokeMeter.TIME}");
                            carPostDataSmokeMeter.Number = clientJsonData.carPostDataSmokeMeter.NOMER;
                            carPostDataSmokeMeter.RunIn = clientJsonData.carPostDataSmokeMeter.TYPE;
                            carPostDataSmokeMeter.DFree = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.D_FREE);
                            carPostDataSmokeMeter.DMax = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.D_MAX);
                            carPostDataSmokeMeter.NDFree = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.N_D_FREE);
                            carPostDataSmokeMeter.NDMax = Convert.ToDecimal(clientJsonData.carPostDataSmokeMeter.N_D_MAX);
                            carPostDataSmokeMeter.CarModelSmokeMeterId = carModelSmokeMeter.Id;

                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Get data smokemeter finished{Environment.NewLine}");
                            Logger.Log($"{carPostId}: получены данные: Дымомер, номер автомобиля - {carPostDataSmokeMeter.Number}");
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"{DateTime.Now} >> Error parse data smokemeter >> {ex.Message}{Environment.NewLine}");
                        }

                        try
                        {
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter started{Environment.NewLine}");
                            using (var connection = new NpgsqlConnection(SmartEcoAConnectionString))
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
                            //Console.WriteLine($"{DateTime.Now} >> CarPost {carPostId}: Insert data smokemeter finished{Environment.NewLine}");
                        }
                        catch (Exception ex)
                        {
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

                        if (!string.IsNullOrEmpty(clientJsonData.carPostDataAutoTest.DopInfo.TesterName))
                        {
                            var testersv = connection.Query<Tester>($"SELECT tester.\"Id\", tester.\"Name\" " +
                                $"FROM public.\"Tester\" as tester " +
                                $"JOIN public.\"CarPostDataAutoTest\" as data ON data.\"TesterId\" = tester.\"Id\" " +
                                $"JOIN public.\"CarModelAutoTest\" as model ON model.\"Id\" = data.\"CarModelAutoTestId\" " +
                                $"WHERE model.\"CarPostId\" = {carPostId} " +
                                $"ORDER BY tester.\"Id\" DESC");
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
                            carPostDataAutoTest.Temperature = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.TEMP);
                            carPostDataAutoTest.Pressure = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.PRESS);
                            carPostDataAutoTest.GasSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.N_AUTOTEST);
                            carPostDataAutoTest.GasCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo?.D_AUTOTEST);
                            carPostDataAutoTest.MeteoSerialNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.N_METEO);
                            carPostDataAutoTest.MeteoCheckDate = Convert.ToDateTime(clientJsonData.carPostDataAutoTest.DopInfo?.D_METEO);
                            carPostDataAutoTest.TestNumber = Convert.ToDecimal(clientJsonData.carPostDataAutoTest.DopInfo?.NUM_TEST);
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
            }
            return 1;
        }
    }
}
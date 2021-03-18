using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsServer.ClientModels
{
    public class ClientJsonData
    {
        public int? CarPostId { get; set; }
        public List<ClientCarModelSmokeMeter> carModelSmokeMeters { get; set; }
        public List<ClientCarPostDataSmokeMeter> carPostDataSmokeMeters { get; set; }
        public List<ClientCarModelAutoTest> carModelAutoTests { get; set; }
        public List<ClientCarPostDataAutoTest> carPostDataAutoTests { get; set; }

        public ClientJsonData()
        {
            carPostDataSmokeMeters = new List<ClientCarPostDataSmokeMeter>();
            carPostDataAutoTests = new List<ClientCarPostDataAutoTest>();
            carModelSmokeMeters = new List<ClientCarModelSmokeMeter>();
            carModelAutoTests = new List<ClientCarModelAutoTest>();
        }
    }
}

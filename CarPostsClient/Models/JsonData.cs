using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class JsonData
    {
        public int? CarPostId { get; set; }
        public List<CarModelSmokeMeter> carModelSmokeMeters { get; set; }
        public List<CarPostDataSmokeMeter> carPostDataSmokeMeters { get; set; }
        public List<CarModelAutoTest> carModelAutoTests { get; set; }
        public List<CarPostDataAutoTest> carPostDataAutoTests { get; set; }

        public JsonData()
        {
            carPostDataSmokeMeters = new List<CarPostDataSmokeMeter>();
            carPostDataAutoTests = new List<CarPostDataAutoTest>();
            carModelSmokeMeters = new List<CarModelSmokeMeter>();
            carModelAutoTests = new List<CarModelAutoTest>();
        }
    }
}

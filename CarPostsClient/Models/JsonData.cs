using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class JsonData
    {
        public CarModelSmokeMeter carModelSmokeMeter { get; set; }
        public CarPostDataSmokeMeter carPostDataSmokeMeter { get; set; }
        public CarModelAutoTestV1 carModelAutoTestV1 { get; set; }
        public CarPostDataAutoTestV1 carPostDataAutoTestV1 { get; set; }
        public CarModelAutoTestV2 carModelAutoTestV2 { get; set; }
        public CarPostDataAutoTestV2 carPostDataAutoTestV2 { get; set; }
    }
}

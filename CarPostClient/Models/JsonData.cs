using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostClient.Models
{
    public class JsonData
    {
        public CarModelSmokeMeter carModelSmokeMeter { get; set; }
        public CarPostDataSmokeMeter carPostDataSmokeMeter { get; set; }
        public CarModelAutoTest carModelAutoTest { get; set; }
        public CarPostDataAutoTest carPostDataAutoTest { get; set; }
        public Tester tester { get; set; }
        public string VersionDbAutotest { get; set; }
        public string VersionDbSmokemeter { get; set; }
    }
}

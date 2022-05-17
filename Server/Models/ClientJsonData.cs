using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class ClientJsonData
    {
        public ClientCarModelSmokeMeter carModelSmokeMeter { get; set; }
        public ClientCarPostDataSmokeMeter carPostDataSmokeMeter { get; set; }
        public ClientCarModelAutoTest carModelAutoTest { get; set; }
        public ClientCarPostDataAutoTest carPostDataAutoTest { get; set; }
        public ClientTester tester { get; set; }
        public string VersionDbAutotest { get; set; }
        public string VersionDbSmokemeter { get; set; }
    }
}

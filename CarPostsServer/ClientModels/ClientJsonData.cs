using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsServer.ClientModels
{
    public class ClientJsonData
    {
        //public int? CarPostId { get; set; }
        public ClientCarModelSmokeMeter carModelSmokeMeter { get; set; }
        public ClientCarPostDataSmokeMeter carPostDataSmokeMeter { get; set; }
        public ClientCarModelAutoTestV1 carModelAutoTestV1 { get; set; }
        public ClientCarPostDataAutoTestV1 carPostDataAutoTestV1 { get; set; }
        public ClientCarModelAutoTestV2 carModelAutoTestV2 { get; set; }
        public ClientCarPostDataAutoTestV2 carPostDataAutoTestV2 { get; set; }
    }
}

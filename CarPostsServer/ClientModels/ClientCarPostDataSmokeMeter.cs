using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsServer.ClientModels
{
    public class ClientCarPostDataSmokeMeter
    {
        public DateTime DATA { get; set; }
        public string TIME { get; set; }
        public bool TYPE { get; set; }
        public double D_FREE { get; set; }
        public double D_MAX { get; set; }
        public double N_D_FREE { get; set; }
        public double N_D_MAX { get; set; }
        public string NOMER { get; set; }
        public string MODEL { get; set; }
    }
}

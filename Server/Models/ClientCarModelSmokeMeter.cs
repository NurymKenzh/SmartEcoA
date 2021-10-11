using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class ClientCarModelSmokeMeter
    {
        public bool NADDUV { get; set; }
        public bool CONFIRM { get; set; }
        public double? D_FREE { get; set; }
        public double? D_MAX { get; set; }
        public string MODEL { get; set; }
    }
}

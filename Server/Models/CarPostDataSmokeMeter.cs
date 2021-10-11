using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarPostDataSmokeMeter
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }
        public string Number { get; set; }
        public bool RunIn { get; set; }
        public decimal? DFree { get; set; }
        public decimal? DMax { get; set; }
        public decimal? NDFree { get; set; }
        public decimal? NDMax { get; set; }
        public int? CarModelSmokeMeterId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarModelSmokeMeter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Boost { get; set; }
        public decimal? DFreeMark { get; set; }
        public decimal? DMaxMark { get; set; }
        public int CarPostId { get; set; }
    }
}

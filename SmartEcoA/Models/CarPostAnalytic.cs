using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarPostAnalytic
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Measurement { get; set; }

        public int Exceeding { get; set; }

        public int CarPostId { get; set; }

        public CarPost CarPost { get; set; }
    }
}

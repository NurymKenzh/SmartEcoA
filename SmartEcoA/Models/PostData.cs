using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class PostData
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        public string IP { get; set; }

        public string Data { get; set; }
    }
}

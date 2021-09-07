using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class PostDataDivided
    {
        public long Id { get; set; }

        public long PostDataId { get; set; }

        public PostData PostData { get; set; }

        public long? PostDataAvgId { get; set; }

        public PostDataAvg PostDataAvg { get; set; }

        public string MN { get; set; }

        public string OceanusCode { get; set; }

        public decimal Value { get; set; }
    }
}

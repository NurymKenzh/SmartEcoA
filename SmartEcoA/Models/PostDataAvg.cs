using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class PostDataAvg
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        public decimal Value { get; set; }

        public int MeasuredParameterId { get; set; }

        public MeasuredParameter MeasuredParameter { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class DopInfo
    {
        public int ID { get; set; }
        public decimal TEMP { get; set; }
        public decimal PRESS { get; set; }
        public long N_AUTOTEST { get; set; }
        public DateTime D_AUTOTEST { get; set; }
        public long N_METEO { get; set; }
        public DateTime D_METEO { get; set; }
        public int ID_TESTER { get; set; }
        public long NUM_TEST { get; set; }

        //доп. поле для сервера
        public string TesterName { get; set; }
    }
}

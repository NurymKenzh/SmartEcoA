using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class MeasuredParameter
    {
        public int Id { get; set; }

        public string NameKK { get; set; }

        public string NameRU { get; set; }

        public string NameEN { get; set; }

        public string Name
        {
            get
            {
                string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                switch (language)
                {
                    case "en":
                        return NameEN;
                    case "pl":
                        return NameRU;
                    case "kk":
                        return NameKK;
                    default:
                        return NameRU;
                }
            }
        }

        public decimal? MPCDailyAverage { get; set; } // maximum permissible concentration

        public decimal? MPCMaxOneTime { get; set; }

        public string OceanusCode { get; set; }

        public string KazhydrometCode { get; set; }
    }
}

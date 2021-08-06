using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class Report
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string NameEN { get; set; }

        public string NameRU { get; set; }

        public string NameKK { get; set; }

        public string Name
        {
            get
            {
                string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                switch (language)
                {
                    case "en":
                        return NameEN;
                    case "ru":
                        return NameRU;
                    case "kk":
                        return NameKK;
                    default:
                        return NameRU;
                }
            }
        }

        public string InputParametersEN { get; set; }

        public string InputParametersRU { get; set; }

        public string InputParametersKK { get; set; }

        public string InputParameters
        {
            get
            {
                string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                switch (language)
                {
                    case "en":
                        return InputParametersEN;
                    case "ru":
                        return InputParametersRU;
                    case "kk":
                        return InputParametersKK;
                    default:
                        return InputParametersRU;
                }
            }
        }

        // Входные параметры для формирования отчета через ';'
        // Пример, "Param1=test1;Param2=\"test2\""
        public string Inputs { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTime? CarPostStartDate { get; set; }

        public DateTime? CarPostEndDate { get; set; }

        public string FileName { get; set; }
    }
}

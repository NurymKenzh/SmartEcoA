using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarPostDataSmokeMeter
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        // NOMER
        // Номерной знак
        public string Number { get; set; }

        // TYPE
        // Обкатанный автомобиль
        public bool RunIn { get; set; }

        // D_FREE
        // Дымность, 1/м
        // Свободное ускорение
        public decimal DFree { get; set; }

        // D_MAX
        public decimal DMax { get; set; }

        // N_D_FREE
        // Дымность, 1/м
        // Норма
        public decimal NDFree { get; set; }

        // N_D_MAX
        public decimal NDMax { get; set; }

        public int? CarModelSmokeMeterId { get; set; }

        public CarModelSmokeMeter CarModelSmokeMeter { get; set; }
    }
}

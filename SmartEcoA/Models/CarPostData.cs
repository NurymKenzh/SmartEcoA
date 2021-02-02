using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarPostData
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

        // D_FREE
        // Дымность, 1/м
        // Свободное ускорение
        public decimal DMax { get; set; }

        // N_D_FREE
        public decimal NDFree { get; set; }

        // N_D_FREE
        public decimal NDMax { get; set; }

        public int CarModelId { get; set; }

        public CarModel CarModel { get; set; }
    }
}

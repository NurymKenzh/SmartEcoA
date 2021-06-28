using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarPostDataAutoTest
    {
        public long Id { get; set; }

        public DateTime DateTime { get; set; }

        // NOMER
        // Номерной знак
        public string Number { get; set; }

        // DOPOL1
        // Дополнительное поле 1
        public string DOPOL1 { get; set; }

        // DOPOL2
        // Дополнительное поле 2
        public string DOPOL2 { get; set; }

        // бензиновый

        // MIN_TAH
        // Частота при минимальных оборотах, 1/мин
        public decimal? MIN_TAH { get; set; }

        // MIN_CO
        // CO при минимальных оборотах, %
        public decimal? MIN_CO { get; set; }

        // MIN_CH
        // CH при минимальных оборотах, ppm
        public decimal? MIN_CH { get; set; }

        // MIN_CO2
        // CO2 при минимальных оборотах, %
        public decimal? MIN_CO2 { get; set; }

        // MIN_O2
        // O2 при минимальных оборотах, %
        public decimal? MIN_O2 { get; set; }

        // MIN_L
        // λ при минимальных оборотах, %
        public decimal? MIN_L { get; set; }

        // MAX_TAH
        // Частота при повышенных оборотах, 1/мин
        public decimal? MAX_TAH { get; set; }

        // MAX_CO
        // CO при повышенных оборотах, %
        public decimal? MAX_CO { get; set; }

        // MAX_CH
        // CH при повышенных оборотах, ppm
        public decimal? MAX_CH { get; set; }

        // MAX_CO2
        // CO2 при повышенных оборотах, %
        public decimal? MAX_CO2 { get; set; }

        // MAX_O2
        // O2 при повышенных оборотах, %
        public decimal? MAX_O2 { get; set; }

        // MAX_L
        // λ при повышенных оборотах, %
        public decimal? MAX_L { get; set; }

        // ZAV_NOMER
        // Номер измерения
        public decimal? ZAV_NOMER { get; set; }

        // дизельный

        // K_1
        // Показатель ослабления светового потока K №1, 1/м
        public decimal? K_1 { get; set; }

        // K_2
        // Показатель ослабления светового потока K №2, 1/м
        public decimal? K_2 { get; set; }

        // K_3
        // Показатель ослабления светового потока K №3, 1/м
        public decimal? K_3 { get; set; }

        // K_4
        // Показатель ослабления светового потока K №4, 1/м
        public decimal? K_4 { get; set; }

        // K_SVOB
        // Показатель ослабления светового потока K при свободном ускорении, 1/м
        public decimal? K_SVOB { get; set; }

        // K_MAX
        // Показатель ослабления светового потока K при максимальной частоте вращения, 1/м
        public decimal? K_MAX { get; set; }

        // MIN_NO
        // NO при минимальных оборотах
        public decimal? MIN_NO { get; set; }

        // MAX_NO
        // NO при повышенных оборотах
        public decimal? MAX_NO { get; set; }

        public decimal? ATNUM { get; set; }
        public decimal? MIN_NOx { get; set; }
        public decimal? MAX_NOx { get; set; }
        public int? Version { get; set; }

        public int? CarModelAutoTestId { get; set; }

        public CarModelAutoTest CarModelAutoTest { get; set; }
    }
}

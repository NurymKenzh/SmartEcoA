using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarModelSmokeMeter
    {
        public int Id { get; set; }

        // MODEL
        // Модель
        public string Name { get; set; }

        // ID_ECOLOG
        // Тип экологической классификации
        public int? TypeEcoClassId { get; set; }
        public TypeEcoClass TypeEcoClass { get; set; }

        // CATEGORY
        // Категория автомобиля
        public string Category { get; set; }

        // DVIG
        // Тип двигателя
        // 0 - бензиновый, 1 - дизельный, 2 - газовый
        public decimal? EngineType { get; set; }

        // Порог ограничения выхлопа
        // бензиновый, газовый

        // MIN_TAH
        // Минимальные обороты, 1/мин
        public decimal? MIN_TAH { get; set; }

        // DEL_MIN
        // Минимальные обороты, +/-, 1/мин
        public decimal? DEL_MIN { get; set; }

        // MAX_TAH
        // Повышенные обороты, 1/мин
        public decimal? MAX_TAH { get; set; }

        // DEL_MAX
        // Повышенные обороты, +/-, 1/мин
        public decimal? DEL_MAX { get; set; }

        // MIN_CO
        // CO при минимальных оборотах, %
        public decimal? MIN_CO { get; set; }

        // MAX_CO
        // CO при повышенных оборотах, %
        public decimal? MAX_CO { get; set; }

        // MIN_CH
        // CH при минимальных оборотах, ppm
        public decimal? MIN_CH { get; set; }

        // MAX_CH
        // CH при повышенных оборотах, ppm
        public decimal? MAX_CH { get; set; }

        // L_MIN
        // λ минимальная
        public decimal? L_MIN { get; set; }

        // L_MAX
        // λ максимальная
        public decimal? L_MAX { get; set; }

        // дизельный

        // K_SVOB
        // Показатель ослабления светового потока K при свободном ускорении, 1/м
        public decimal? K_SVOB { get; set; }

        // K_MAX
        // Показатель ослабления светового потока K при максимальной частоте вращения, 1/м
        public decimal? K_MAX { get; set; }

        public int CarPostId { get; set; }

        public CarPost CarPost { get; set; }
        public int? ParadoxId { get; set; }
    }
}

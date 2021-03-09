using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class CarModelAutoTest
    {
        public int Id { get; set; }

        // MODEL
        // Модель
        public string Name { get; set; }

        // DVIG
        // Тип двигателя
        // 0 - бензиновый, 1 - дизельный
        public int EngineType { get; set; }

        // Порог ограничения выхлопа
        // бензиновый

        // MIN_TAH
        // Минимальные обороты, 1/мин
        public decimal MIN_TAH { get; set; }

        // DEL_MIN
        // Минимальные обороты, +/-, 1/мин
        public decimal DEL_MIN { get; set; }

        // MAX_TAH
        // Повышенные обороты, 1/мин
        public decimal MAX_TAH { get; set; }

        // DEL_MAX
        // Повышенные обороты, +/-, 1/мин
        public decimal DEL_MAX { get; set; }

        // MIN_CO
        // CO при минимальных оборатах, %
        public decimal MIN_CO { get; set; }

        // MAX_CO
        // CO при повышенных оборатах, %
        public decimal MAX_CO { get; set; }

        // MIN_CH
        // CH при минимальных оборатах, ppm
        public decimal MIN_CH { get; set; }

        // MAX_CH
        // CH при повышенных оборатах, ppm
        public decimal MAX_CH { get; set; }

        // L_MIN
        // λ минимальная
        public decimal L_MIN { get; set; }

        // L_MAX
        // λ максимальная
        public decimal L_MAX { get; set; }

        // дизельный

        // K_SVOB
        // Показатель ослабления сетевого потока K при свободном ускорении, 1/м
        public decimal K_SVOB { get; set; }

        // K_MAX
        // Показатель ослабления сетевого потока K при максимальной частоте вращения, 1/м
        public decimal K_MAX { get; set; }

        public int CarPostId { get; set; }

        public CarPost CarPost { get; set; }
    }
}

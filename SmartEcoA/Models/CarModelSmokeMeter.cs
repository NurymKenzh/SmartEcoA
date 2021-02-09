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

        // NADDUV
        // Двигатель с наддувом
        public bool Boost { get; set; }

        // D_FREE
        // Предельно-допустимые значения коэффициента поглощения света k, указанные в знаке официального утверждения
        // в режиме свободного ускорения
        public decimal? DFreeMark { get; set; }

        // D_MAX
        public decimal? DMaxMark { get; set; }

        // CONFIRM
        // Имеет знак официального утверждения
        //public bool ApprovalMark
        //{
        //    get
        //    {
        //        return DFreeMark != null ? true : false;
        //    }
        //}

        public int CarPostId { get; set; }

        public CarPost CarPost { get; set; }
    }
}

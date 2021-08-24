using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsServer.ClientModels
{
    public class ClientCarModelAutoTest
    {
        public int ID { get; set; }
        public int DVIG { get; set; }
        public int ID_ECOLOG { get; set; }
        public string CATEGORY { get; set; }
        public decimal MIN_TAH { get; set; }
        public decimal DEL_MIN { get; set; }
        public decimal MAX_TAH { get; set; }
        public decimal DEL_MAX { get; set; }
        public decimal MIN_CO { get; set; }
        public decimal MAX_CO { get; set; }
        public decimal MIN_CH { get; set; }
        public decimal MAX_CH { get; set; }
        public decimal L_MIN { get; set; }
        public decimal L_MAX { get; set; }
        public decimal K_SVOB { get; set; }
        public decimal K_MAX { get; set; }
        public string MODEL { get; set; }

        //доп. поле для сервера
        public string TypeEcoName { get; set; }
    }
}
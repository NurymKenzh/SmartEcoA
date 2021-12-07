using NickBuhro.Translit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class ClientCarModelSmokeMeter
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
        private string typeEcoName { get; set; }
        public string TypeEcoName
        {
            get
            {
                return typeEcoName;
            }
            set
            {
                typeEcoName = Transliteration.LatinToCyrillic(value, Language.Russian);
            }
        }
    }
}

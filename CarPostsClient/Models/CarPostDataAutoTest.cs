using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarPostDataAutoTest
    {
        private string model;
        private string nomer;

        public int ID { get; set; }
        public DateTime DATA { get; set; }
        public string TIME { get; set; }
        public string DOPOL1 { get; set; }
        public string DOPOL2 { get; set; }
        public decimal MIN_TAH { get; set; }
        public decimal MIN_CO { get; set; }
        public decimal MIN_CH { get; set; }
        public decimal MIN_CO2 { get; set; }
        public decimal MIN_O2 { get; set; }
        public decimal MIN_L { get; set; }
        public decimal MAX_TAH { get; set; }
        public decimal MAX_CO { get; set; }
        public decimal MAX_CH { get; set; }
        public decimal MAX_CO2 { get; set; }
        public decimal MAX_O2 { get; set; }
        public decimal MAX_L { get; set; }
        public decimal ZAV_NOMER { get; set; }
        public decimal K_1 { get; set; }
        public decimal K_2 { get; set; }
        public decimal K_3 { get; set; }
        public decimal K_4 { get; set; }
        public decimal K_SBOB { get; set; }
        public decimal K_MAX { get; set; }
        public decimal MIN_NO { get; set; }
        public decimal MAX_NO { get; set; }
        public string NOMER
        {
            get
            {
                return nomer;
            }
            set
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //nomer = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value)); //for provider "Microsoft.ACE.OLEDB.12.0"
                nomer = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value)); // for  provider "Microsoft.Jet.OLEDB.4.0"
            }
        }
        public string MODEL
        {
            get
            {
                return model;
            }
            set
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //model = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value)); //for provider "Microsoft.ACE.OLEDB.12.0"
                model = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value)); // for  provider "Microsoft.Jet.OLEDB.4.0"
            }
        }
        public int ID_MODEL { get; set; }

        public DopInfo DopInfo { get; set; }
    }

    public class DopInfo
    {
        public int ID { get; set; }
        public decimal TEMP { get; set; }
        public decimal PRESS { get; set; }
        public long N_AUTOTEST { get; set; }
        public DateTime D_AUTOTEST { get; set; }
        public long N_METEO { get; set; }
        public DateTime D_METEO { get; set; }
        public int ID_TESTER { get; set; }
        public long NUM_TEST { get; set; }

        //доп. поле для сервера
        public string TesterName { get; set; }
    }
}

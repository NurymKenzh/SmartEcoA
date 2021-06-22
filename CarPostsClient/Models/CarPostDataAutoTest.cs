using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarPostDataAutoTest
    {
        private string model;
        private string nomer;

        public DateTime DATA { get; set; }
        public string TIME { get; set; }
        public string DOPOL1 { get; set; }
        public string DOPOL2 { get; set; }
        public double MIN_TAH { get; set; }
        public double MIN_CO { get; set; }
        public double MIN_CH { get; set; }
        public double MIN_CO2 { get; set; }
        public double MIN_O2 { get; set; }
        public double MIN_L { get; set; }
        public double MAX_TAH { get; set; }
        public double MAX_CO { get; set; }
        public double MAX_CH { get; set; }
        public double MAX_CO2 { get; set; }
        public double MAX_O2 { get; set; }
        public double MAX_L { get; set; }
        public double ZAV_NOMER { get; set; }
        public double K_1 { get; set; }
        public double K_2 { get; set; }
        public double K_3 { get; set; }
        public double K_4 { get; set; }
        public double K_SBOB { get; set; }
        public double K_MAX { get; set; }
        public double MIN_NO { get; set; }
        public double MAX_NO { get; set; }
        public string NOMER
        {
            get
            {
                return nomer;
            }
            set
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                nomer = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value));
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
                model = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value));
            }
        }

        public int Version { get; set; }
    }
}

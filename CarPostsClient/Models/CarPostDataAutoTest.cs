using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarPostDataAutoTestV1
    {
        private string model;
        private string nomer;

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

        public int Version { get; set; }
    }

    public class CarPostDataAutoTestV2
    {
        private string govNumber;

        public int Index { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public decimal ATNum { get; set; }
        public decimal MinTax { get; set; }
        public decimal MinCO { get; set; }
        public decimal MinCH { get; set; }
        public decimal MinCO2 { get; set; }
        public decimal MinO2 { get; set; }
        public decimal MinNOx { get; set; }
        public decimal MinLambda { get; set; }
        public decimal MaxTax { get; set; }
        public decimal MaxCO { get; set; }
        public decimal MaxCH { get; set; }
        public decimal MaxCO2 { get; set; }
        public decimal MaxO2 { get; set; }
        public decimal MaxNOx { get; set; }
        public decimal MaxLambda { get; set; }
        public string GovNumber
        {
            get
            {
                return govNumber;
            }
            set
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //govHumber = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value)); //for provider "Microsoft.ACE.OLEDB.12.0"
                govNumber = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value)); // for  provider "Microsoft.Jet.OLEDB.4.0"
            }
        }
        public int Model { get; set; }

        public int Version { get; set; }
        public string ModelName { get; set; }
    }
}

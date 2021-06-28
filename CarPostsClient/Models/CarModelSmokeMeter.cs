using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarModelSmokeMeter
    {
        private string model;

        public bool NADDUV { get; set; }
        public bool CONFIRM { get; set; }
        public double? D_FREE { get; set; }
        public double? D_MAX { get; set; }
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
    }
}

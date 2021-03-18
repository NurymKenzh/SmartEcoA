using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarPostDataSmokeMeter
    {
        private string model;
        private string nomer;

        public DateTime DATA { get; set; }
        public string TIME { get; set; }
        public bool TYPE { get; set; }
        public double D_FREE { get; set; }
        public double D_MAX { get; set; }
        public double N_D_FREE { get; set; }
        public double N_D_MAX { get; set; }

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
    }
}

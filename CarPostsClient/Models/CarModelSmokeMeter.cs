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
                model = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value));
            }
        }
    }
}

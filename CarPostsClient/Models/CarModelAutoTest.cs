using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class CarModelAutoTest
    {
        private string model;

        public int DVIG { get; set; }
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

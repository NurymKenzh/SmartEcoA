using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsClient.Models
{
    public class Tester
    {
        private string name;

        public int ID { get; set; }
        public string NAME
        {
            get
            {
                return name;
            }
            set
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //model = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(866).GetBytes(value)); //for provider "Microsoft.ACE.OLEDB.12.0"
                name = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value)); // for  provider "Microsoft.Jet.OLEDB.4.0"
            }
        }
    }
}

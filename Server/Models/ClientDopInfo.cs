using NickBuhro.Translit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class ClientDopInfo
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
        private string tName { get; set; }
        public string TesterName
        {
            get
            {
                return tName;
            }
            set
            {
                tName = ToTitleCase(Transliteration.LatinToCyrillic(value, Language.Russian));
            }
        }

        private string ToTitleCase(string text)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var words = text.Split(' ');

            var newText = "";
            foreach (var word in words)
            {
                newText += char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant() + ' ';
            }
            return newText.Trim();
        }
    }
}

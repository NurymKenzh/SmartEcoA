using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarPostVersionDb
    {
        public int Id { get; set; }
        public string NameDb { get; set; }
        public DateTime Version { get; set; }
        public int CarPostId { get; set; }
        public CarPost CarPost { get; set; }
    }
}

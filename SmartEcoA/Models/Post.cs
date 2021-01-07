using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MN { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Information { get; set; }

        public string PhoneNumber { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        public int PollutionEnvironmentId { get; set; }
        public PollutionEnvironment PollutionEnvironment { get; set; }

        public int DataProviderId { get; set; }
        public DataProvider DataProvider { get; set; }

        public int? KazhydrometID { get; set; }

        public bool Automatic { get; set; }
    }
}

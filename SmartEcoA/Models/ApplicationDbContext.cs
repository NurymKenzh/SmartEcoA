using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartEcoA.Models;

namespace SmartEcoA.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options) : base(options)
        {
        }
        public DbSet<SmartEcoA.Models.PollutionEnvironment> PollutionEnvironment { get; set; }
        public DbSet<SmartEcoA.Models.MeasuredParameter> MeasuredParameter { get; set; }
        public DbSet<SmartEcoA.Models.DataProvider> DataProvider { get; set; }
        public DbSet<SmartEcoA.Models.Project> Project { get; set; }
        public DbSet<SmartEcoA.Models.Post> Post { get; set; }
        public DbSet<SmartEcoA.Models.PostData> PostData { get; set; }
        public DbSet<SmartEcoA.Models.PostDataDivided> PostDataDivided { get; set; }
        public DbSet<SmartEcoA.Models.Stat> Stat { get; set; }
        public DbSet<SmartEcoA.Models.CarPost> CarPost { get; set; }
    }
}

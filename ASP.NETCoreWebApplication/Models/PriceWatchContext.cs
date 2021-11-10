using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASP.NETCoreWebApplication.Models
{
    public class PriceWatchContext : DbContext
    {



        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<PermsList> Perms { get; set; }
        public DbSet<HousingObject> HousingObjects { get; set; }
        public DbSet<ItemObject> ItemObjects { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<Photo> ItemPhotos { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<Proxy> Proxies { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        
        public PriceWatchContext()
        {
            
        }
        
        public PriceWatchContext(DbContextOptions<PriceWatchContext> options) : base(options)
        {
            
        }
        
    };
}
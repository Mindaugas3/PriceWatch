using ASP.NETCoreWebApplication.Models.Schemas;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreWebApplication.Models
{
    public class PriceWatchContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<HousingObject> HousingObjects { get; set; }
        public DbSet<ItemObject> ItemObjects { get; set; }
        public DbSet<Photo> ItemPhotos { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public PriceWatchContext()
        {
            
        }
        
        public PriceWatchContext(DbContextOptions<PriceWatchContext> options) : base(options)
        {
            
        }
        
    };
}
using System.Collections.Generic;
using System.Linq;

namespace ASP.NETCoreWebApplication.Models
{
    public static class PWDatabaseInitializer
    {
        public static void init(PriceWatchContext priceWatchContext)
        {
            priceWatchContext.Database.EnsureCreated();
        }
        
        public static void insert(PriceWatchContext context, HousingObject obj)
        {
            context.HousingObjects.Add(obj);
            context.SaveChanges();
        }

        public static void InsertMany(PriceWatchContext context, List<HousingObject> hList)
        {
            foreach (var obj in hList)
            {
                var urlsUnique = context.HousingObjects.Select(c => c.url).ToHashSet();
                if(!urlsUnique.Contains(obj.url))
                {
                    context.HousingObjects.Add(obj);              
                }

            }

            context.SaveChanges();
        }
    }
}
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
        public static void InsertItems(PriceWatchContext context, List<ItemObject> IList)
        {
            foreach(var obj in IList)
            {
                var urlsUnique = context.ItemObjects.Select(c => c.url).ToArray();
                if (!urlsUnique.Contains(obj.url))
                {
                    context.ItemObjects.Add(obj);
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Models.Schemas;

namespace ASP.NETCoreWebApplication.Models.Repositories
{
    public class ItemsRepository: RepositoryBase
    {
        public ItemsRepository(PriceWatchContext priceWatchContext) : base(priceWatchContext)
        {
        }
        
        public void InsertMany(List<ItemObject> ItemsList)
        {
            foreach (var itemObject in from itemObject in ItemsList let urlsUnique = _priceWatchContext.ItemObjects.Select(c => c.url).ToArray() where !urlsUnique.Contains(itemObject.url) select itemObject)
            {
                _priceWatchContext.ItemObjects.Add(itemObject);
            }

            _priceWatchContext.SaveChanges();
        }
    }
}
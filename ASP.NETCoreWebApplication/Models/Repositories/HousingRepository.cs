using System;
using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Models.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ASP.NETCoreWebApplication.Models.Repositories
{
    public class HousingRepository: RepositoryBase
    {
        public HousingRepository(PriceWatchContext priceWatchContext) : base(priceWatchContext)
        {
        }

        public void InsertMany( List<HousingObject> realEstateList)
        {
            foreach (var realEstate in from realEstate in realEstateList let urlsUnique = _priceWatchContext.HousingObjects.Select(c => c.url).ToHashSet() where !urlsUnique.Contains(realEstate.url) select realEstate)
            {
                _priceWatchContext.HousingObjects.Add(realEstate);
            }

            _priceWatchContext.SaveChanges();
        }

        public List<HousingObject> GetAll()
        {
            return _priceWatchContext.HousingObjects.ToList();
        }

        public List<HousingObject> GetSome(Dictionary<string, string> filterParams)
        {
            IQueryable<HousingObject> housingObjects = _priceWatchContext.HousingObjects;

            if (filterParams["roomsMin"] != StringValues.Empty && filterParams["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(filterParams["roomsMin"]);
                var roomsMax = Int32.Parse(filterParams["roomsMax"]);

                housingObjects = housingObjects.Where(r => r.rooms <= roomsMax && r.rooms >= roomsMin);
            }

            if (filterParams["priceMin"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price > Int32.Parse(filterParams["priceMin"]));
            }
            
            if (filterParams["priceMax"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price < Int32.Parse(filterParams["priceMax"]));
            }
            
            if (filterParams["searchKey"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.title.Contains(filterParams["searchKey"]));
            }

            return housingObjects.ToList();
        }

        public IEnumerable<HousingObject> GetSome(Dictionary<string, StringValues> filterParams)
        {
            IQueryable<HousingObject> housingObjects = _priceWatchContext.HousingObjects;

            if (filterParams["roomsMin"] != StringValues.Empty && filterParams["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(filterParams["roomsMin"]);
                var roomsMax = Int32.Parse(filterParams["roomsMax"]);

                housingObjects = housingObjects.Where(r => r.rooms <= roomsMax && r.rooms >= roomsMin);
            }

            if (filterParams["priceMin"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price > Int32.Parse(filterParams["priceMin"]));
            }
            
            if (filterParams["priceMax"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price < Int32.Parse(filterParams["priceMax"]));
            }
            
            if (filterParams["searchKey"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.title.Contains(filterParams["searchKey"]));
            }

            return housingObjects?.ToList() ?? new List<HousingObject>();
        }
    }
}
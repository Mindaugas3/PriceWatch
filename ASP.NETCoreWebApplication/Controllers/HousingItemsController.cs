using System;
using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Models;
using ASP.NETCoreWebApplication.Models.DataSources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Range = ASP.NETCoreWebApplication.Utils.Range;

namespace ASP.NETCoreWebApplication.Controllers
{
    
    [Route("[controller]/{Action}")]
    public class HousingItemsController : ControllerBase
    {
        private readonly PriceWatchContext pc;

        public HousingItemsController(PriceWatchContext context)
        {
            this.pc = context;
        }
        
        [HttpGet]
        public IEnumerable<HousingObject> SqlGet()
        {
            
            var requestQuery = this.Request.Query;
            
            Console.WriteLine(requestQuery["priceMax"]);

            IEnumerable<HousingObject> housingObjects;
            
            if(requestQuery.Keys.ToArray().Length == 0)
            {
                return this.pc.HousingObjects.ToArray();
            }

            housingObjects = this.pc.HousingObjects.ToArray();
            
            if (requestQuery["roomsMin"] != StringValues.Empty && requestQuery["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(requestQuery["roomsMin"]);
                var roomsMax = Int32.Parse(requestQuery["roomsMax"]);

                housingObjects = housingObjects.Where(r => r.rooms <= roomsMax && r.rooms >= roomsMin);
            }

            if (requestQuery["priceMin"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price > Int32.Parse(requestQuery["priceMin"]));
            }
            
            if (requestQuery["priceMax"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.price < Int32.Parse(requestQuery["priceMax"]));
            }
            
            if (requestQuery["searchKey"] != StringValues.Empty)
            {
                housingObjects = housingObjects.Where(r => r.title.Contains(requestQuery["searchKey"]));
            }

            return housingObjects;
        }
        
        [HttpGet]
        public IEnumerable<HousingObject> Get()
        {
            Console.Write("GET HousingItemsController");
            
            var requestQuery = this.Request.Query;

            Range? roomsRange = null;
            Range? priceRange = null;
            Range? floorsRange = null;

            if (requestQuery["roomsMin"] != StringValues.Empty && requestQuery["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(requestQuery["roomsMin"]);
                var roomsMax = Int32.Parse(requestQuery["roomsMax"]);
                roomsRange = new Range(roomsMin, roomsMax);
            }

            if (requestQuery["floors"] != StringValues.Empty)
            {
                var floorsMin = Int32.Parse(requestQuery["floorsMin"]);
                var floorsMax = Int32.Parse(requestQuery["floorsMax"]);
                floorsRange = new Range(floorsMin, floorsMax);
            }
            
            if (requestQuery["priceMin"] != StringValues.Empty && requestQuery["priceMax"] != StringValues.Empty)
            {
                priceRange = new Range(Int32.Parse(requestQuery["priceMin"]), Int32.Parse(requestQuery["priceMax"]));
            }
            
            if (requestQuery["priceMin"] == StringValues.Empty && requestQuery["priceMax"] != StringValues.Empty)
            {
                priceRange = new Range(0, Int32.Parse(requestQuery["priceMax"]));
            }
            
            if (requestQuery["priceMin"] != StringValues.Empty && requestQuery["priceMax"] == StringValues.Empty)
            {
                priceRange = new Range(Int32.Parse(requestQuery["priceMax"]), Int32.MaxValue);
            }

            var aruodasData = new AruodasLt(
                HousingType.BuyFlat,
                roomsRange ?? new Range(1, 5),
                new Range(1, 150),
                floorsRange ?? new Range(1, 10),
                priceRange ?? new Range(0, 180000),
                requestQuery["searchKey"]
            ).Scrap(this.pc, 5);

            return aruodasData;
        }
    }
}
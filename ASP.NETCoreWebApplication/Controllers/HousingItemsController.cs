using System;
using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Models;
using ASP.NETCoreWebApplication.Models.DataSources;
using ASP.NETCoreWebApplication.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
            
            var param = this.Request.Query;

            var houses = this.pc.HousingObjects;
            
            Console.WriteLine(param["priceMax"]);

            IEnumerable<HousingObject> returnObject;
            
            if(param.Keys.ToArray().Length == 0)
            {
                return this.pc.HousingObjects.ToArray();
            }

            returnObject = this.pc.HousingObjects.ToArray();
            
            if (param["roomsMin"] != StringValues.Empty && param["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(param["roomsMin"]);
                var roomsMax = Int32.Parse(param["roomsMax"]);

                returnObject = returnObject.Where(r => r.rooms <= roomsMax && r.rooms >= roomsMin);
            }

            if (param["priceMin"] != StringValues.Empty)
            {
                returnObject = returnObject.Where(r => r.price > Int32.Parse(param["priceMin"]));
            }
            
            if (param["priceMax"] != StringValues.Empty)
            {
                returnObject = returnObject.Where(r => r.price < Int32.Parse(param["priceMax"]));
            }
            
            if (param["searchKey"] != StringValues.Empty)
            {
                returnObject = returnObject.Where(r => r.title.Contains(param["searchKey"]));
            }

            return returnObject;
        }
        
        [HttpGet]
        public IEnumerable<HousingObject> Get()
        {
            Console.Write("GET HousingItemsController");
            
            var param = this.Request.Query;
            
            DefaultParameters aruodasDefault = new DefaultParameters(new Dictionary<string, dynamic>
            {
                ["type"] = HousingType.BuyFlat,
                ["rooms"] = new RoomNumberDescriptor(1, 5),
                ["area"] = new AreaDescriptor(1, 150),
                ["priceRange"] = new PriceRange(0, 180000),
                ["optionalSearchText"] = null
            });

            RoomNumberDescriptor? rnd = null;
            PriceRange? prc = null;

            if (param["roomsMin"] != StringValues.Empty && param["roomsMax"] != StringValues.Empty)
            {
                var roomsMin = Int32.Parse(param["roomsMin"]);
                var roomsMax = Int32.Parse(param["roomsMax"]);

                if (roomsMax > roomsMin)
                {
                    rnd = new RoomNumberDescriptor(roomsMin, roomsMax);                    
                }

                if (roomsMax == roomsMin)
                {
                    rnd = new RoomNumberDescriptor(roomsMax);
                }
            }

            //TODO NOT IMPLEMENTED
            // if (param["floors"] != StringValues.Empty)
            // {
            //     returnObject = returnObject.Where(r => r.floorsThis == Int32.Parse(param["floors"]));
            // }
            
            if (param["priceMin"] != StringValues.Empty && param["priceMax"] != StringValues.Empty)
            {
                prc = new PriceRange(Int32.Parse(param["priceMin"]), Int32.Parse(param["priceMax"]));
            }
            
            if (param["priceMin"] == StringValues.Empty && param["priceMax"] != StringValues.Empty)
            {
                prc = new PriceRange(0, Int32.Parse(param["priceMax"]));
            }
            
            if (param["priceMin"] != StringValues.Empty && param["priceMax"] == StringValues.Empty)
            {
                prc = new PriceRange(Int32.Parse(param["priceMax"]), Int32.MaxValue);
            }

            var aruodasData = new AruodasLt(
                aruodasDefault["type"],
                (rnd != null) ? rnd : aruodasDefault["rooms"],
                aruodasDefault["area"],
                (prc != null)? prc : aruodasDefault["priceRange"],
                (param["searchKey"] != StringValues.Empty)? param["searchKey"] : aruodasDefault["optionalSearchText"]
            ).Scrap(this.pc, 5);

            return aruodasData;
        }
    }
}
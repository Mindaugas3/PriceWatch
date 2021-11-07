using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
        private static string ParseKeys(IEnumerable<KeyValuePair<string, StringValues>> values)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.AppendLine(value.Key + " = " + string.Join(", ", value.Value));
            }
            return sb.ToString();
        }

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
            
            //no query params? return SELECT * FROM housingobjects
            if(param.Keys.ToArray().Length == 0)
            {
                return this.pc.HousingObjects.ToArray();
            }

            //use LINQ instead of WHERE
            returnObject = this.pc.HousingObjects.ToArray();

            //TODO ROOMS NOT PRESENT IN DATABASE
            // if (param["roomsMin"] != StringValues.Empty && param["roomsMax"] != StringValues.Empty)
            // {
            //     returnObject = returnObject.Where()
            // }

            if (param["floors"] != StringValues.Empty)
            {
                returnObject = returnObject.Where(r => r.floorsThis == Int32.Parse(param["floors"]));
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
                returnObject = returnObject.Where(r => r.description.Contains(param["searchKey"]));
            }

            return returnObject;
        }
        
        [HttpGet]
        public IEnumerable<HousingObject> Get()
        {
            Console.Write("GET HousingItemsController");
            DefaultParameters aruodasDefault = new DefaultParameters(new Dictionary<string, dynamic>
            {
                ["type"] = HousingType.BuyFlat,
                ["mun"] = null,
                ["city"] = null,
                ["district"] = null,
                ["rooms"] = new RoomNumberDescriptor(3),
                ["area"] = new AreaDescriptor(1, 150),
                ["priceRange"] = new PriceRange(0, 180000),
                ["optionalSearchText"] = null
            });
            
            var param = this.Request.Query;

            var aruodasData = new AruodasLt(
                aruodasDefault["type"],
                aruodasDefault["mun"],
                aruodasDefault["city"],
                aruodasDefault["district"],
                aruodasDefault["rooms"],
                aruodasDefault["area"],
                aruodasDefault["priceRange"],
                (param["searchKey"] != StringValues.Empty)? param["searchKey"] : aruodasDefault["optionalSearchText"]
            ).Scrap(this.pc, 5);

            return aruodasData;
        }
    }
}
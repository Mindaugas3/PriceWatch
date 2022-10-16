using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCoreWebApplication.Models;
using ASP.NETCoreWebApplication.Models.DataSources;
using ASP.NETCoreWebApplication.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MySqlX.XDevAPI.Common;

namespace ASP.NETCoreWebApplication.Controllers
{
    
    [Route("[controller]/{Action}")]
    public class ItemsComparisonController : ControllerBase
    {
        private readonly PriceWatchContext pc;

        public ItemsComparisonController(PriceWatchContext context)
        {
            this.pc = context;
        }
        
        // [HttpGet]
        // public IEnumerable<ItemObject> SqlGet()
        // {
        //     
        //     var param = this.Request.Query;
        //
        //     var houses = this.pc.ItemObjects;
        //     
        //     Console.WriteLine(param["priceMax"]);
        //
        //     IEnumerable<ItemObject> returnObject;
        //     
        //     //no query params? return SELECT * FROM housingobjects
        //     if(param.Keys.ToArray().Length == 0)
        //     {
        //         return this.pc.ItemObjects.ToArray();
        //     }
        //
        //     //use LINQ instead of WHERE
        //     returnObject = this.pc.ItemObjects.ToArray();
        //
        //     // if (param["floors"] != StringValues.Empty)
        //     // {
        //     //     returnObject = returnObject.Where(r => r.floorsThis == Int32.Parse(param["floors"]));
        //     // }
        //
        //     if (param["category"] != String.Empty)
        //     {
        //         Category c = this.pc.Categories.First(r => r.name == param["category"]);
        //         returnObject = returnObject.Where(item => item.category.Id == c.Id);
        //     }
        //     
        //     if (param["priceMin"] != StringValues.Empty)
        //     {
        //         returnObject = returnObject.Where(r => r.price > Int32.Parse(param["priceMin"]));
        //     }
        //     
        //     if (param["priceMax"] != StringValues.Empty)
        //     {
        //         returnObject = returnObject.Where(r => r.price < Int32.Parse(param["priceMax"]));
        //     }
        //     
        //     if (param["searchKey"] != StringValues.Empty)
        //     {
        //         returnObject = returnObject.Where(r => r.title.Contains(param["searchKey"]));
        //     }
        //
        //     return returnObject;
        // }
        //
        
        [HttpGet]
        public IEnumerable<ItemObject> Get()
        {
            Console.Write("GET ItemsComparisonController");
            
            var param = this.Request.Query;

            // PriceRange? prc = null;
            //
            // if (param["priceMin"] != StringValues.Empty && param["priceMax"] != StringValues.Empty)
            // {
            //     prc = new PriceRange(Int32.Parse(param["priceMin"]), Int32.Parse(param["priceMax"]));
            // }
            //
            // if (param["priceMin"] == StringValues.Empty && param["priceMax"] != StringValues.Empty)
            // {
            //     prc = new PriceRange(0, Int32.Parse(param["priceMax"]));
            // }
            //
            // if (param["priceMin"] != StringValues.Empty && param["priceMax"] == StringValues.Empty)
            // {
            //     prc = new PriceRange(Int32.Parse(param["priceMax"]), Int32.MaxValue);
            // }

            var items = VarleLt.VarleLT(this.pc).Result;
            Console.WriteLine("SCRAP TASK RUNNING");
            Console.WriteLine("CONTROLLER RETURNED");
            return items;
        }
    }
}
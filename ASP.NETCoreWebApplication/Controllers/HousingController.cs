using System.Collections.Generic;
using ASP.NETCoreWebApplication.Models.DataSources;
using ASP.NETCoreWebApplication.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCoreWebApplication.Controllers
{
    
    public class HousingController : Controller
    {
        [HttpGet(Name = "Housing")]
        public IActionResult Index()
        {
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
            string aruodasData = new AruodasLt(
                aruodasDefault["type"],
                aruodasDefault["mun"],
                aruodasDefault["city"],
                aruodasDefault["district"],
                aruodasDefault["rooms"],
                aruodasDefault["area"],
                aruodasDefault["priceRange"],
                aruodasDefault["optionalSearchText"]
            ).Scrap(5);
            
            return Ok(aruodasData);
        }
    }
}
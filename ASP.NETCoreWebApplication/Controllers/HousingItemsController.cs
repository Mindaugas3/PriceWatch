using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCoreWebApplication.Models.Repositories;
using ASP.NETCoreWebApplication.Models.Schemas;
using ASP.NETCoreWebApplication.Scrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace ASP.NETCoreWebApplication.Controllers
{
    
    [Route("[controller]/{Action}")]
    public class HousingItemsController : ControllerBase
    {
        private readonly AruodasLt _aruodasLt;
        private readonly HousingRepository _repository;
        private readonly HousingItemsValidator _housingItemsValidator = new HousingItemsValidator();

        public HousingItemsController(HousingRepository repository, AruodasLt aruodasLt)
        {
            _repository = repository;
            _aruodasLt = aruodasLt;
        }
        
        [HttpGet]
        [Produces("application/json")]
        public IEnumerable<HousingObject> GetExisting()
        {
            var query = HttpContext.Request.QueryString.Value;
            Dictionary<string, StringValues> queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);
            
            IEnumerable<HousingObject> housingObjects;
            
            if(queryDictionary.Keys.ToArray().Length == 0)
            {
                return _repository.GetAll();
            }

            housingObjects = _repository.GetSome(queryDictionary);

            return housingObjects;
        }
        
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IEnumerable<HousingObject>> ScanRealEstate()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string text = await reader.ReadToEndAsync();
                var body = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

                var result = _housingItemsValidator.Validate(body);

                if (!result.IsValid)
                {
                    throw new BadHttpRequestException(result.Errors.ToString());
                }
                
                string targetUrl = _aruodasLt.BuildUrlFromParams(
                    HousingType.RentFlat,
                    Int32.Parse(body["roomsMin"]),
                    Int32.Parse(body["roomsMax"]),
                    Int32.Parse(body["priceMin"]),
                    Int32.Parse(body["priceMax"]),
                    Int32.Parse(body["areaMin"]),
                    Int32.Parse(body["areaMax"]),
                    Int32.Parse(body["floorsMin"]),
                    Int32.Parse(body["floorsMax"]),
                    FHouseState.None,
                    body["searchKey"]
                );
                
                var aruodasData = _aruodasLt.ScrapSearchResults(targetUrl);

                var housingObjects = aruodasData.ToList();
                _repository.InsertMany(housingObjects.ToList());

                return housingObjects;
            }
        }
    }
}
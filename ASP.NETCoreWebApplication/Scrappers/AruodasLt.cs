using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASP.NETCoreWebApplication.Infrastructure;
using ASP.NETCoreWebApplication.Models;
using ASP.NETCoreWebApplication.Models.Schemas;
using OpenQA.Selenium;

namespace ASP.NETCoreWebApplication.Scrappers
{
    public enum HousingType
    {
        RentFlat,
        BuyFlat,
        RentHouse,
        BuyHouse
    }

    public enum FHouseState
    {
        Full, //IRENGTAS
        Part, //DALINAI IRENGTAS
        Noteq, //Neirengtas
        Nfinished, //Nebaigtas statyti
        Foundation, //Pamatai
        None //Neatitinka jokiu kitu parametru
    }

    public class AruodasLt
    {
        private WebDriver _wd;

        public AruodasLt()
        {
             _wd = SeleniumScrapper.CreateFirefoxDriver();
        }

        public string BuildUrlFromParams(
            HousingType type,
            Int32 roomsMin,
            Int32 roomsMax, 
            Int32 priceMin,
            Int32 priceMax,
            Int32 areaMin,
            Int32 areaMax,
            Int32 floorsMin,
            Int32 floorsMax,
            FHouseState houseState, 
            string optionalSearch = null
            )
        {
            string location = type switch
            {
                HousingType.BuyFlat => "https://www.aruodas.lt/butai/?",
                HousingType.BuyHouse => "https://www.aruodas.lt/namai/?",
                HousingType.RentFlat => "https://www.aruodas.lt/butu-nuoma/?",
                HousingType.RentHouse => "https://www.aruodas.lt/namu-nuoma/",
                _ => throw new ArgumentException("Invalid argument for HousingType")
            };

            location += "FRoomNumMin=";
            location += roomsMin.ToString();
            location += "&";
            location += "FRoomNumMax=";
            location += roomsMax.ToString();
            location += "&";
            
            location += "FPriceMin=";
            location += priceMin.ToString();
            location += "&";
            location += "FPriceMax=";
            location += priceMax.ToString();
            location += "&";

            location += "FAreaOverAllMin=";
            location += areaMin.ToString();
            location += "&";
            location += "FAreaOverAllMax=";
            location += areaMax.ToString();
            location += "&";

            location += "FFloorNumMin=";
            location += floorsMin.ToString();
            location += "&";
            location += "FFloorNumMax=";
            location += floorsMax.ToString();

            if (optionalSearch != null)
            {
                location += "search_text=";
                location += optionalSearch;
            }

            Logger.WriteHttpGetScrappers(location);
            return location;
        }

        public IEnumerable<HousingObject> ScrapSearchResults(string url, int depth = 4)
        {
            
            if(depth < 1) throw new ArgumentException("depth cannot be zero or negative");
            _wd.Navigate().GoToUrl(url);
            
            Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>> rawValues = new Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>>
            {
                ["price"] = Tuple.Create("span", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName,"list-item-price-v2")),
                ["area"] = Tuple.Create("div", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName,"list-AreaOverall-v2")),
                ["rooms"] = Tuple.Create("div", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName, "list-RoomNum-v2")),
                ["floors"] = Tuple.Create("div", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName, "list-Floors-v2")),
                ["location"] = Tuple.Create("h3", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName, "")),
                ["url"] = Tuple.Create("a", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.Hyperlink, "")),
                ["img"] = Tuple.Create("img", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.Image, "---none")),

            };
            List<Dictionary<string, string>> collectedData = HTMLNodeParser.FeedHtml(_wd.PageSource, "div", "list-row-v2", rawValues);

            foreach (Dictionary<string,string> entry in collectedData)
            {
                var titleAndDescription = ScrapInsidePage(entry["url"]);
                entry["title"] = titleAndDescription["title"];
                entry["description"] = titleAndDescription["description"];
            }
            
            List<HousingObject> housingObjects = new List<HousingObject>();
            foreach (var entry in collectedData)
            {
                HousingObject obj = ParseRawStringValues(entry);
                housingObjects.Add(obj);
            }

            housingObjects = housingObjects
                .GroupBy(entry => entry.url)
                .Select(g => g.First())
                .ToList();
            
            return housingObjects.ToArray();
        }

        private Dictionary<string, string> ScrapInsidePage(string url)
        {
            Logger.WriteHttpGetScrappers(url);
            _wd.Navigate().GoToUrl(url);
            Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>> rawValues =
                new Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>>
                {
                    ["title"] = Tuple.Create("h1", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName, "obj-header-text")),
                    ["description"] = Tuple.Create("div", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementId, "collapsedTextBlock"))
                };
            List<Dictionary<string, string>> collectedData = HTMLNodeParser.FeedHtml(_wd.PageSource, "div", "obj-cont", rawValues);
            return collectedData.First();
        }

        private static HousingObject ParseRawStringValues(Dictionary<string, string> insertable)
        {
            string price = insertable["price"].Replace(" ", "").Replace("\n", "").Replace("\r", "");
            string currency = price.Substring(price.Length - 1); //last character

            int priceAmount = Int32.Parse(new string(price.Where(char.IsDigit).ToArray()));
            
            var floors = insertable["floors"].Replace(" ", "").Replace("\n", "").Replace("\r", "").Split("/");
            int currentFloor = Int32.Parse(floors[0]);
            int maxFloor = Int32.Parse(floors[1]);
            
            var area = (int) float.Parse(insertable["area"].Replace(" ", "").Replace("\n", "").Replace("\r", "")
                , CultureInfo.InvariantCulture);

            var location = insertable["location"].Replace("\n", " ").Replace("\r", " ");
            HousingObject dbObject = new HousingObject
            {
                Source_id = 1,
                title = insertable["title"],
                url = insertable["url"],
                price = priceAmount,
                location = location.Trim(),
                timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Currency = currency,
                area = area,
                rooms = Int32.Parse(insertable["rooms"].Trim()),
                floorsMax = maxFloor,
                floorsThis = currentFloor,
                description = insertable["description"],
                imgUrl = insertable["img"]
            };
            return dbObject;
        }

        ~AruodasLt(){
            _wd.Close();
        }
    }
}
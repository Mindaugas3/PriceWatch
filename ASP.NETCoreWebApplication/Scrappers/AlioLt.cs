using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASP.NETCoreWebApplication.Infrastructure;
using static ASP.NETCoreWebApplication.Infrastructure.HTMLNodeParser;
using ASP.NETCoreWebApplication.Models.Schemas;

namespace ASP.NETCoreWebApplication.Scrappers
{
    using Selector = Tuple<string, ParseOptions>;
    using SelectorMap = Dictionary<string, Tuple<string, ParseOptions>>;
    using MappedValues = Dictionary<string, string>;
    using HtmlAttribute = ParserFlags;
    
    public class AlioLt : ScrapperBase
    {

        public AlioLt(SeleniumScrapper seleniumScrapper) : base(seleniumScrapper)
        {
        }

        protected override string GetLinkFromType(string type)
        {
            string location = type switch
            {
                HousingType.BuyFlat => "?category_id=1373",
                HousingType.BuyHouse => "?category_id=1433",
                HousingType.RentFlat => "?category_id=1393",
                HousingType.RentHouse => "?category_id=1453",
                _ => throw new ArgumentException("Invalid argument for HousingType")
            };
            return location + "&advanced_filter=1&search_block=1";
        }
        
        public string BuildUrlFromParams(
            string type,
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
            string location = "https://www.alio.lt/paieska/";
            if (optionalSearch != null)
            {
                location += optionalSearch;
                location += "/";
            }
            location += GetLinkFromType(type);

            location += "search%5Bgte%5D%5Bkaina_1%5D=";
            location += priceMin.ToString();
            location += "&";
            location += "search%5Blte%5D%5Bkaina_1%5D=";
            location += priceMax.ToString();
            location += "&";

            location += "search%5Bgte%5D%5Bbusto_plotas_m_1%5D=";
            location += areaMin.ToString();
            location += "&";
            location += "search%5Blte%5D%5Bbusto_plotas_m_1%5D=";
            location += areaMax.ToString();
            location += "&";

            if (new[] {HousingType.BuyFlat, HousingType.RentFlat}.Contains(type))
            {
                location += "search%5Bgte%5D%5Bbuto_aukstas_1%5D=";
                location += floorsMin.ToString();
                location += "&";
                location += "search%5Blte%5D%5Bbuto_aukstas_1%5D=";
                location += floorsMax.ToString();
                location += "&";
                
                location += "search%5Bgte%5D%5Bkambariu_skaicius_1%5D=";
                location += roomsMin.ToString();
                location += "&";
                location += "search%5Blte%5D%5Bkambariu_skaicius_1%5D=";
                location += roomsMax.ToString();
            }

            if (new[] {HousingType.BuyHouse, HousingType.RentHouse}.Contains(type))
            {
                location += "search%5Bgte%5D%5Baukstu_skaicius%5D=";
                location += floorsMin.ToString();
                location += "&";
                location += "search%5Blte%5D%5Baukstu_skaicius%5D=5";
                location += floorsMax.ToString();
            }

            location += "&";
            location += "search%5Bwith_photos%5D=1";

            Logger.WriteHttpGetScrappers(location);
            return location;
        }

        protected override SelectorMap GetSelectors(string type)
        {
            if (new[] {HousingType.BuyFlat, HousingType.RentFlat}.Contains(type))
            {
                return new SelectorMap
                {
                    ["price"] = Tuple.Create("span", new ParseOptions(ClassName, "list-item-price-v2")),
                    ["area"] = Tuple.Create("div", new ParseOptions(ClassName, "list-AreaOverall-v2")),
                    ["rooms"] = Tuple.Create("div", new ParseOptions(ClassName,  "list-RoomNum-v2")),
                    ["floors"] = Tuple.Create("div", new ParseOptions(ClassName,  "list-Floors-v2")),
                    ["location"] = Tuple.Create("h3", new ParseOptions(ClassName,  "")),
                    ["url"] = Tuple.Create("a", new ParseOptions(Hyperlink, "")),
                    ["img"] = Tuple.Create("img", new ParseOptions(Image, "---none")),

                };
            }
            return new SelectorMap
            {
                ["img"] = Tuple.Create("img", new ParseOptions(Image, "---none")),
                ["price"] = Tuple.Create("span", new ParseOptions(ClassName, "list-item-price-v2")),
                ["area"] = Tuple.Create("div", new ParseOptions(ClassName, "list-AreaOverall-v2")),
                ["location"] = Tuple.Create("h3", new ParseOptions(ElementId, "")),
                ["url"] = Tuple.Create("a", new ParseOptions(Hyperlink, "---none")),
            };
        }
        
        private MappedValues ScrapInsidePage(string url, string type)
        {
            var webDriver = _seleniumScrapper.GetWebDriver();
            Logger.WriteHttpGetScrappers(url);
            webDriver.Navigate().GoToUrl(url);
            SelectorMap selectors =
                new SelectorMap
                {
                    ["title"] = Tuple.Create("h1", new ParseOptions(ClassName, "obj-header-text")),
                    ["description"] = Tuple.Create("div", new ParseOptions(ElementId, "collapsedTextBlock"))
                };
            if (new[] {HousingType.BuyHouse, HousingType.RentHouse}.Contains(type))
            {
                selectors["table"] = Tuple.Create("dl", new ParseOptions(ClassName, "obj-details"));
            }
            List<MappedValues> collectedData = FeedHtml(webDriver.PageSource, "div", "obj-cont", selectors);
            return collectedData.First();
        }
        
        public override IEnumerable<HousingObject> ScrapSearchResults(string url,  string type, int depth = 4)
        {
            var webDriver = _seleniumScrapper.GetWebDriver();
            
            if(depth < 1) throw new ArgumentException("depth cannot be zero or negative");
            
            webDriver.Navigate().GoToUrl(url);

            SelectorMap selectors = GetSelectors(type);
            List<MappedValues> collectedData = FeedHtml(webDriver.PageSource, "div", "list-row-v2", selectors);

            foreach (MappedValues entry in collectedData)
            {
                entry["propertyType"] = type;
                var titleAndDescription = ScrapInsidePage(entry["url"], type);
                entry["title"] = titleAndDescription["title"];
                entry["description"] = titleAndDescription["description"];
                if (new[] {HousingType.BuyHouse, HousingType.RentHouse}.Contains(type))
                {
                    entry["table"] = titleAndDescription["table"];
                }
            }
            
            List<HousingObject> housingObjects = collectedData.Select(entry => ParseRawStringValues(entry)).ToList();

            housingObjects = housingObjects
                .GroupBy(entry => entry.url)
                .Select(g => g.First())
                .ToList();
            
            return housingObjects.ToArray();
        }
        
                private static HousingObject ParseRawStringValues(MappedValues insertable)
        {
            string price = TrimStringValue(insertable["price"]);
            string currency = price.Substring(price.Length - 1); //last character

            int priceAmount = Int32.Parse(new string(price.Where(char.IsDigit).ToArray()));

            int currentFloor = -1;
            int maxFloor = -1;

            int rooms = -1;

            if (insertable.ContainsKey("floors"))
            {
                string[] floors = TrimStringValue(insertable["floors"]).Split("/");
                currentFloor = Int32.Parse(floors[0]);
                maxFloor = Int32.Parse(floors[1]);
            }
            else
            {
                var slicedTable = insertable["table"].Split("\r\n").Where(str =>
                {
                    return str.Trim() != "";
                }).Select(str => str.Trim()).ToArray();

                var tableEntryIndex = slicedTable
                    .ToList().IndexOf("Aukštų sk.:");
                
                currentFloor = Int32.Parse(slicedTable.ToArray()[tableEntryIndex + 1].Trim());
                maxFloor = currentFloor;
            }

            if (insertable.ContainsKey("rooms"))
            {
                rooms = Int32.Parse(insertable["rooms"].Trim());
            }
            else
            {
                rooms = -1;
            }
            
            int area = (int) float.Parse(TrimStringValue(insertable["area"])
                , CultureInfo.InvariantCulture);
            
            string location = insertable["location"].Replace("\n", " ").Replace("\r", " ");
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
                rooms = rooms,
                floorsMax = maxFloor,
                floorsThis = currentFloor,
                description = insertable["description"],
                imgUrl = insertable["img"],
                propertyType = insertable["propertyType"]
            };
            return dbObject;
        }
                
    }
    
}
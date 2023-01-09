using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASP.NETCoreWebApplication.Infrastructure;
using ASP.NETCoreWebApplication.Models.Schemas;
using static ASP.NETCoreWebApplication.Infrastructure.HTMLNodeParser;

namespace ASP.NETCoreWebApplication.Scrappers
{
    using ParseOptions = ParseOptions;
    using Selector = Tuple<string, ParseOptions>;
    using SelectorMap = Dictionary<string, Tuple<string, ParseOptions>>;
    using MappedValues = Dictionary<string, string>;
    using HtmlAttribute = ParserFlags;

    public class AruodasLt : ScrapperBase
    {
        private static readonly HtmlAttribute ClassName = HtmlAttribute.HtmlElementClassName;
        private static readonly HtmlAttribute Hyperlink = HtmlAttribute.Hyperlink;
        private static readonly HtmlAttribute Image = HtmlAttribute.Image;
        private static readonly HtmlAttribute ElementId = HtmlAttribute.HtmlElementId;

        protected override string GetLinkFromType(string type)
        {
            string location = type switch
            {
                HousingType.BuyFlat => "https://www.aruodas.lt/butai/?",
                HousingType.BuyHouse => "https://www.aruodas.lt/namai/?",
                HousingType.RentFlat => "https://www.aruodas.lt/butu-nuoma/?",
                HousingType.RentHouse => "https://www.aruodas.lt/namu-nuoma/",
                _ => throw new ArgumentException("Invalid argument for HousingType")
            };
            return location;
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
            string location = GetLinkFromType(type);

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

            if (new[] {HousingType.BuyFlat, HousingType.RentFlat}.Contains(type))
            {
                location += "FFloorNumMin=";
                location += floorsMin.ToString();
                location += "&";
                location += "FFloorNumMax=";
                location += floorsMax.ToString();
                
                location += "FRoomNumMin=";
                location += roomsMin.ToString();
                location += "&";
                location += "FRoomNumMax=";
                location += roomsMax.ToString();
                location += "&";
            }

            if (new[] {HousingType.BuyHouse, HousingType.RentHouse}.Contains(type))
            {
                location += "FHouseHeightMin=";
                location += floorsMin.ToString();
                location += "&";
                location += "FHouseHeightMax=";
                location += floorsMax.ToString();
            }

            if (optionalSearch != null)
            {
                location += "search_text=";
                location += optionalSearch;
            }

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

        public AruodasLt(SeleniumScrapper seleniumScrapper) : base(seleniumScrapper)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASP.NETCoreWebApplication.Interactors;
using ASP.NETCoreWebApplication.Utils;
using OpenQA.Selenium;
using Range = ASP.NETCoreWebApplication.Utils.Range;

namespace ASP.NETCoreWebApplication.Models.DataSources
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
        private readonly HousingType _housingType;
        private readonly Range _price;
        private readonly Range _rooms;
        private readonly Range _area; //kvadratiniai metrai
        private readonly Range _floors;
        private readonly string? _optionalSearch;

        public AruodasLt(HousingType type, Range rooms, Range area, Range floors, Range price, string optionalSearchText = null)
        {
            this._housingType = type;
            this._rooms = rooms;
            this._price = price;
            this._area = area;
            this._optionalSearch = optionalSearchText;
            this._floors = floors;
        }

        private string BuildUrl()
        {
            string location = _housingType switch
            {
                HousingType.BuyFlat => "https://www.aruodas.lt/butai/?",
                HousingType.BuyHouse => "https://www.aruodas.lt/namai/?",
                HousingType.RentFlat => "https://www.aruodas.lt/butu-nuoma/?",
                HousingType.RentHouse => "https://www.aruodas.lt/namu-nuoma/",
                _ => throw new ArgumentException("Invalid argument for HousingType")
            };

            location += "FRoomNumMin=";
            location += this._rooms.Min.ToString();
            location += "&";
            location += "FRoomNumMax=";
            location += this._rooms.Max.ToString();
            location += "&";
            
            location += "FPriceMin=";
            location += this._price.Min.ToString();
            location += "&";
            location += "FPriceMax=";
            location += this._price.Max.ToString();
            location += "&";

            location += "FAreaOverAllMin=";
            location += this._area.Min.ToString();
            location += "&";
            location += "FAreaOverAllMax=";
            location += this._area.Max.ToString();
            location += "&";

            location += "FFloorNumMin=";
            location += this._floors.Min.ToString();
            location += "&";
            location += "FFloorNumMax=";
            location += this._floors.Max.ToString();

            if (_optionalSearch != null)
            {
                location += "search_text=";
                location += _optionalSearch;
            }

            Logger.WriteHttpGetScrappers(location);
            return location;
        }

        public IEnumerable<HousingObject> Scrap(PriceWatchContext dbc, int depth = 4)
        {
            
            if(depth < 1) throw new ArgumentException("depth cannot be zero or negative");
            WebDriver wd = SeleniumScrapper.CreateFirefoxDriver();
            wd.Navigate().GoToUrl(this.BuildUrl());
            
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
            List<Dictionary<string, string>> collectedData = HTMLNodeParser.FeedHtml(wd.PageSource, "div", "list-row-v2", rawValues);

            foreach (Dictionary<string,string> entry in collectedData)
            {
                var titleAndDescription = DeepScrap(wd, entry["url"]);
                entry["title"] = titleAndDescription["title"];
                entry["description"] = titleAndDescription["description"];
            }
            
            List<HousingObject> databaseEntries = new List<HousingObject>();
            foreach (var entry in collectedData)
            {
                HousingObject obj = ToDbo(entry);
                databaseEntries.Add(obj);
            }

            databaseEntries = databaseEntries
                .GroupBy(entry => entry.url)
                .Select(g => g.First())
                .ToList();
            
            PWDatabaseInitializer.InsertMany(dbc, databaseEntries);
            wd.Close();
            return databaseEntries.ToArray();
        }

        private static Dictionary<string, string> DeepScrap(WebDriver wd, string url)
        {
            Logger.WriteHttpGetScrappers(url);
            wd.Navigate().GoToUrl(url);
            Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>> rawValues =
                new Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>>
                {
                    ["title"] = Tuple.Create("h1", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementClassName, "obj-header-text")),
                    ["description"] = Tuple.Create("div", new HTMLNodeParser.ParseOptions(HTMLNodeParser.ParserFlags.HtmlElementId, "collapsedTextBlock"))
                };
            List<Dictionary<string, string>> collectedData = HTMLNodeParser.FeedHtml(wd.PageSource, "div", "obj-cont", rawValues);
            return collectedData.First();
        }

        private static HousingObject ToDbo(Dictionary<string, string> insertable)
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
    }
}
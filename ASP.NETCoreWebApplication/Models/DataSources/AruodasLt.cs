using System;
using System.Collections.Generic;
using System.Linq;
using ASP.NETCoreWebApplication.Interactors;
using ASP.NETCoreWebApplication.Utils;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

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

    public class RangeDescriptor
    {
        public enum RoomRangeType
        {
            Exact, //exactly X rooms
            RangeOf //Rooms from X to Y, will throw InvalidDescriptorException if Y < X
        }

        private RoomRangeType _rangeType;
        private int min;
        private int max;
        private int exactly;

        public class InvalidDescriptorException : Exception
        {
            public InvalidDescriptorException()
            {
                
            }
        }

        public RangeDescriptor(int min, int max)
        {
            if (min > max)
            {
                throw new InvalidDescriptorException();
            }

            if (min < 1 || max < 1)
            {
                //logiskai niekada nebus maziau nei 1 kambario butas
                throw new InvalidDescriptorException();
            }

            this.min = min;
            this.max = max;
            this._rangeType = RoomRangeType.RangeOf;
        }

        public RangeDescriptor(int exactly)
        {
            if (exactly < 1)
            {
                throw new InvalidDescriptorException();
            }

            this.exactly = exactly;
            this._rangeType = RoomRangeType.Exact;
        }

        public bool MatchDescriptor(int rooms)
        {
            if (_rangeType == RoomRangeType.Exact)
            {
                return this.exactly == rooms;
            }
            else
            {
                return rooms >= this.min && rooms <= this.max;
            }
        }

        public RoomRangeType GetRangeType()
        {
            return this._rangeType;
        }

        public int GetMin()
        {
            return this.min;
        }

        public int GetMax()
        {
            return this.max;
        }

        public int GetExactly()
        {
            return this.exactly;
        }

    }

    public class RoomNumberDescriptor : RangeDescriptor
    {
        public RoomNumberDescriptor(int min, int max) : base(min, max)
        {
        }

        public RoomNumberDescriptor(int exactly) : base(exactly)
        {
        }
    }

    public class AreaDescriptor : RangeDescriptor
    {
        public AreaDescriptor(int min, int max) : base(min, max)
        {
        }

        public AreaDescriptor(int exactly) : base(exactly)
        {
        }
    }

    public enum Municipality
    {
        //TODO pull from API
    }

    public enum City
    {
        //TODO pull from API
    }

    public enum District
    {
        //TODO pull from API
    }
    public class AruodasLt
    {
        private readonly HousingType housingType;
        private readonly Municipality? municipality;
        private readonly City? city;
        private readonly District? distr;
        private readonly PriceRange priceRange;
        private readonly RoomNumberDescriptor rooms;
        private readonly AreaDescriptor area; //kvadratiniai metrai
        private readonly string? optionalSearch;
        public AruodasLt(HousingType type, Municipality? mun, City? city, District? district, RoomNumberDescriptor rooms, AreaDescriptor area, PriceRange priceRange, string? optionalSearchText)
        {
            //assign
            this.housingType = type;
            this.municipality = mun;
            this.city = city;
            this.distr = district;
            this.rooms = rooms;
            this.priceRange = priceRange;
            this.area = area;
            this.optionalSearch = optionalSearchText;
            //search
        }

        private string BuildUrl()
        {
            //?FRoomNumMin=10&FRoomNumMax=20 Kambariu skaicius nuo iki
            //?FAreaOverAllMin=20&FAreaOverAllMax=800 Kvadratiniai metrai
            
            //&FHouseState=full PILNAI IRENGTAS
            //&FHouseState=part DALINAI IRENGTAS
            //&FHouseState=noteq NEIRENGTAS
            //&FHouseState=n_finished NEPASTATYTAS
            //&FHouseState=foundation PAMATAI
            //?FHouseState=none Neatitinka jokiu kitu parametru
            
            //?search_text=lazdynai -> ieskos lazdynuose
            
            string location = "";

            switch (housingType)
            {
                case HousingType.BuyFlat:
                {
                    location = "https://www.aruodas.lt/butai/?";
                    break;
                }
                case HousingType.BuyHouse:
                {
                    location = "https://www.aruodas.lt/namai/?";
                    break;
                }
                case HousingType.RentFlat:
                {
                    location = "https://www.aruodas.lt/butu-nuoma/?";
                    break;
                }
                case HousingType.RentHouse:
                {
                    location = "https://www.aruodas.lt/namu-nuoma/";
                    break;
                }
                default:
                {
                    throw new InvalidLocationException();
                }
            }
            
            RoomNumberDescriptor.RoomRangeType rangeType = this.rooms.GetRangeType();
            if (rangeType == RoomNumberDescriptor.RoomRangeType.Exact)
            {
                //?FRoomNumMin=Exact&FRoomNumMax=Exact
                location += "FRoomNumMin=";
                location += this.rooms.GetExactly().ToString();
                location += "&";
                location += "FRoomNumMax=";
                location += this.rooms.GetExactly().ToString();
                location += "&";
            }
            else
            {
                location += "FRoomNumMin=";
                location += this.rooms.GetMin().ToString();
                location += "&";
                location += "FRoomNumMax=";
                location += this.rooms.GetMax().ToString();
                location += "&";
            }

            AreaDescriptor.RoomRangeType areaRangeType = this.area.GetRangeType();
            if (rangeType == AreaDescriptor.RoomRangeType.Exact)
            {
                location += "FAreaOverAllMin=";
                location += this.area.GetExactly().ToString();
                location += "&";
                location += "FAreaOverAllMax=";
                location += this.area.GetExactly().ToString();
                location += "&";
            }
            else
            {
                location += "FAreaOverAllMin=";
                location += this.rooms.GetMin().ToString();
                location += "&";
                location += "FAreaOverAllMax=";
                location += this.rooms.GetMax().ToString();
                location += "&";
            }

            if (optionalSearch != null)
            {
                location += "search_text=";
                location += optionalSearch;
            }

            //remove last & from URLs
            return location;
        }

        public string Scrap(int depth = 4)
        {
            
            if(depth < 1) throw new ArgumentException("depth cannot be zero or negative");
            //get html
            WebDriver wd = SeleniumScrapper.CreateFirefoxDriver();
            wd.Navigate().GoToUrl(this.BuildUrl());
            
            //parse XML type with XPath
            XPathParser aruodasParser = new XPathParser(
                "//tr[contains(@class, 'list-row')]",
                new List<string>
                {
                    "//div[contains(@class, 'obj-save')]",
                    "//div[contains(@class, 'obj-unsave')]",
                    "//div[contains(@class, 'icon-on-top-container')]",
                    "//td[contains(@class, 'list-remember')]"
                },
                new Dictionary<string, string>
                {
                    ["price"] = "*//span[contains(@class, 'list-item-price')]",
                    ["area"] = "*//td[contains(@class, 'list-AreaOverall')]",
                    ["rooms"] = "*//td[contains(@class, 'list-RoomNum')]",
                    ["floors"] = "*//td[contains(@class, 'list-Floors')]",
                    ["image"] = "*//img/@src"
                    //["url"] = "//a/@href"
                }
                );
            List<Dictionary<string, string>> collectedData = aruodasParser.FeedHTML(wd.PageSource);

            string rowResultHTML = collectedData.Select(entry => string.Join("\n", entry))
                .Aggregate((x, y) => x + "\n" + y);
            
            return rowResultHTML;
        }
    }
}
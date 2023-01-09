using System;
using System.Collections.Generic;
using ASP.NETCoreWebApplication.Infrastructure;
using ASP.NETCoreWebApplication.Models.Schemas;

namespace ASP.NETCoreWebApplication.Scrappers
{
    using ParseOptions = HTMLNodeParser.ParseOptions;
    using Selector = Tuple<string, HTMLNodeParser.ParseOptions>;
    using SelectorMap = Dictionary<string, Tuple<string, HTMLNodeParser.ParseOptions>>;
    using MappedValues = Dictionary<string, string>;
    using HtmlAttribute = HTMLNodeParser.ParserFlags;
    
    public enum FHouseState
    {
        Full, //IRENGTAS
        Part, //DALINAI IRENGTAS
        Noteq, //Neirengtas
        Nfinished, //Nebaigtas statyti
        Foundation, //Pamatai
        None //Neatitinka jokiu kitu parametru
    }
    
    public abstract class ScrapperBase
    {
        protected const HtmlAttribute ClassName = HtmlAttribute.HtmlElementClassName;
        protected const HtmlAttribute Hyperlink = HtmlAttribute.Hyperlink;
        protected const HtmlAttribute Image = HtmlAttribute.Image;
        protected const HtmlAttribute ElementId = HtmlAttribute.HtmlElementId;
        
        protected readonly SeleniumScrapper _seleniumScrapper;

        protected ScrapperBase(SeleniumScrapper seleniumScrapper)
        {
            _seleniumScrapper = seleniumScrapper;
        }

        protected abstract SelectorMap GetSelectors(string type);
        protected abstract string GetLinkFromType(string type);
        
        protected static string TrimStringValue(string value)
        {
            return value.Replace(" ", "").Replace("\n", "").Replace("\r", "");
        }

        public abstract IEnumerable<HousingObject> ScrapSearchResults(string url, string type, int depth = 4);
    }
}
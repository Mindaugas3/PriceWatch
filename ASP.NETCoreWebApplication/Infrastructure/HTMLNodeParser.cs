using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Utils
{
    public class HTMLNodeParser
    {

        public enum ParserFlags
        {
            Image,
            Hyperlink,
            Script,
            HtmlElementClassName,
            HtmlElementId
        }

        public class ParseOptions
        {
            private ParserFlags ParserFlags { get; }
            private string Attribute { get; }

            public ParseOptions(ParserFlags parserFlags, string attribName)
            {
                this.ParserFlags = parserFlags;
                this.Attribute = attribName;
            }

            public string GetAttributeName()
            {
                return this.Attribute;
            }

            public ParserFlags GetParserFlags()
            {
                return this.ParserFlags;
            }
        }
        public static List<Dictionary<string, string>> FeedHtml(string html, string listItemDescendants, string className, Dictionary<string, Tuple<string, ParseOptions>> itemsToChoose)
        {
            List<Dictionary<string, string>> aggregateData = new List<Dictionary<string, string>>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var descendants = htmlDocument.DocumentNode.Descendants(listItemDescendants)
                .Where(node => node.GetAttributeValue("class", "")
                .Contains(className));
            
            foreach (var childNode in descendants)
            {
                bool skipEntry = false;
                Dictionary<string, string> singleDataItem = new Dictionary<string, string>();
                //LIST ENTRY
                foreach (var key in itemsToChoose.Keys)
                {
                   string selectorTagName = itemsToChoose[key].Item1;
                   ParserFlags pf = itemsToChoose[key].Item2.GetParserFlags();
                   string selectorClassName = itemsToChoose[key].Item2.GetAttributeName();
                   string data = "";
                   switch (pf)
                   {
                       case ParserFlags.Hyperlink: 
                       {
                           data = childNode?.Descendants(selectorTagName)
                               ?.Where(node => node.GetAttributeValue("class", "").Contains(selectorClassName))?.FirstOrDefault()
                               ?.GetAttributeValue("href", "");
                           break;
                       }
                       case ParserFlags.Image:
                       {
                           data = childNode?.Descendants("img")
                               ?.Where(img => img.GetAttributeValue("class", "---none").Equals(selectorClassName))
                               ?.FirstOrDefault()
                               ?.GetAttributeValue("src", "");
                           break;
                       }
                       case ParserFlags.Script:
                       {

                           break;
                       }
                       case ParserFlags.HtmlElementId:
                       {
                           data = childNode?.Descendants(selectorTagName)
                               ?.Where(node => node.GetAttributeValue("id", "").Contains(selectorClassName))?.FirstOrDefault()
                               ?.InnerText;
                           break;
                       }
                       case ParserFlags.HtmlElementClassName:
                       {
                           data = childNode?.Descendants(selectorTagName)
                               ?.Where(node => node.GetAttributeValue("class", "").Contains(selectorClassName))?.FirstOrDefault()
                               ?.InnerText;
                           break;
                       }
                   }
                   if (data == null)
                   {
                       skipEntry = true;
                   }
                   singleDataItem[key] = data;
                }

                if (!skipEntry)
                {
                    aggregateData.Add(singleDataItem);
                }
            }
            return aggregateData;
        }
    }
}
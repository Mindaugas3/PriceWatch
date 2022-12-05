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
            private ParserFlags pf { get; }
            private string attribute { get; }

            public ParseOptions(ParserFlags pf, string attribName)
            {
                this.pf = pf;
                this.attribute = attribName;
            }

            public string getAttrName()
            {
                return this.attribute;
            }

            public ParserFlags getParserFlags()
            {
                return this.pf;
            }
        }
        public static List<Dictionary<string, string>> FeedHTML(string HTML, string ListItemDescendants, string className, Dictionary<String, Tuple<String, ParseOptions>> itemsToChoose)
        {
            List<Dictionary<string, string>> AggregateData = new List<Dictionary<string, string>>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(HTML);

            var descendants = htmlDocument.DocumentNode.Descendants(ListItemDescendants)
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
                   ParserFlags pf = itemsToChoose[key].Item2.getParserFlags();
                   string selectorClassName = itemsToChoose[key].Item2.getAttrName();
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
                    AggregateData.Add(singleDataItem);
                }
            }
            return AggregateData;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Infrastructure
{
    public class HTMLNodeParser
    {
        public class ParseOptions
        {
            public ParserFlags ParserFlags { get; }
            public string Attribute { get; }

            public ParseOptions(ParserFlags parserFlags, string attribName)
            {
                this.ParserFlags = parserFlags;
                this.Attribute = attribName;

            }
        }

        public enum ParserFlags
        {
            Image,
            Hyperlink,
            Script,
            HtmlElementClassName,
            HtmlElementId
        }

        public static List<Dictionary<string, string>> FeedHtml(string html, string listItemDescendants, string className, Dictionary<string, Tuple<string, ParseOptions>> itemsToChoose)
        {
            var htmlDocument = LoadHtmlDocument(html);
            var descendants = GetDescendants(htmlDocument, listItemDescendants, className);
            return descendants.Select(childNode => GetSingleDataItem(childNode, itemsToChoose)).Where(singleDataItem => singleDataItem.Count > 0).ToList();
        }

        private static HtmlDocument LoadHtmlDocument(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private static IEnumerable<HtmlNode> GetDescendants(HtmlDocument htmlDocument, string listItemDescendants, string className)
        {
            return htmlDocument.DocumentNode.Descendants(listItemDescendants)
                .Where(node => node.GetAttributeValue("class", "").Contains(className));
        }

        private static Dictionary<string, string> GetSingleDataItem(HtmlNode childNode, Dictionary<string, Tuple<string, ParseOptions>> itemsToChoose)
        {
            var singleDataItem = new Dictionary<string, string>();
            foreach (var key in itemsToChoose.Keys)
            {
                var selectorTagName = itemsToChoose[key].Item1;
                var parseOptions = itemsToChoose[key].Item2;
                var data = GetData(childNode, selectorTagName, parseOptions);
                if (data == null)
                {
                    break;
                }
                singleDataItem[key] = data;
            }
            return singleDataItem;
        }
        
        private static string GetData(HtmlNode childNode, string selectorTagName, ParseOptions parseOptions)
        {
            return parseOptions.ParserFlags switch
            {
                ParserFlags.Hyperlink => childNode?.Descendants(selectorTagName)
                    ?.FirstOrDefault()
                    ?.GetAttributeValue("href", ""),
                ParserFlags.Image => childNode?.Descendants("img")
                    ?.Where(img => img.GetAttributeValue("class", "---none").Equals(parseOptions.Attribute))
                    ?.FirstOrDefault()
                    ?.GetAttributeValue("src", ""),
                ParserFlags.Script => null,
                ParserFlags.HtmlElementId => childNode?.Descendants(selectorTagName)
                    ?.Where(node => node.GetAttributeValue("id", "").Contains(parseOptions.Attribute))
                    ?.FirstOrDefault()
                    ?.InnerText,
                ParserFlags.HtmlElementClassName => childNode?.Descendants(selectorTagName)
                    ?.Where(node => node.GetAttributeValue("class", "").Contains(parseOptions.Attribute))
                    ?.FirstOrDefault()
                    ?.InnerText,
                _ => null
            };
        }

    }
}
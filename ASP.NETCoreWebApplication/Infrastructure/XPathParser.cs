using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Utils
{
    public class XPathParser
    {
        private readonly string _mainXPathItem;
        private readonly List<string> _xPathRemovalList;
        private readonly Dictionary<string, string> _xPathDataKeys;
        
        public XPathParser(string mainXPathItem, List<string> xPathRemovalList,
            Dictionary<string, string> xPathDataKeys)
        {
            this._xPathDataKeys = xPathDataKeys;
            this._xPathRemovalList = xPathRemovalList;
            this._mainXPathItem = mainXPathItem;
        }
        
        public List<Dictionary<string, string>> FeedHtml(string html)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            
            var mainNodes = htmlDocument.DocumentNode.SelectNodes(this._mainXPathItem);
            Console.Write(mainNodes.Count);
            List<Dictionary<string, string>> aggregateData = new List<Dictionary<string, string>>();
            foreach (var childNode in mainNodes)
            {
                Console.Write(childNode.SelectNodes(this._xPathDataKeys["price"])?.FirstOrDefault()?.InnerHtml + "\n");
                this._xPathRemovalList.Select(x => childNode?.SelectNodes(x)?.ToList())
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .ToList()
                    .ForEach(nodeToRemove => nodeToRemove.Remove());

                Dictionary<string, string> keyedData = this._xPathDataKeys.Keys
                    .ToDictionary(x => x,
                    x => childNode.SelectNodes(this._xPathDataKeys[x])
                        ?.Where(y => y != null)
                        .Select(y => y.OriginalName == "img" ? y.Attributes["src"].Value : y.InnerHtml)
                        .ToArray()
                        .Aggregate((a, b) => a + ";" + b)
                );
                
                aggregateData.Add(keyedData);
            }
            return aggregateData;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Utils
{
    public class XPathParser
    {
        private readonly string MainXPathItem;
        private readonly List<string> XPathRemovalList;
        private readonly Dictionary<string, string> XPathDataKeys;
        
        public XPathParser(string mainXPathItem, List<string> xPathRemovalList,
            Dictionary<string, string> xPathDataKeys)
        {
            this.XPathDataKeys = xPathDataKeys;
            this.XPathRemovalList = xPathRemovalList;
            this.MainXPathItem = mainXPathItem;
        }
        
        public List<Dictionary<string, string>> FeedHTML(string HTML)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(HTML);
            
            var mainNodes = htmlDocument.DocumentNode.SelectNodes(this.MainXPathItem);
            Console.Write(mainNodes.Count);
            string rowResultHTML = "";
            List<Dictionary<string, string>> AggregateData = new List<Dictionary<string, string>>();
            foreach (var childNode in mainNodes)
            {
                Console.Write(childNode.SelectNodes(this.XPathDataKeys["price"])?.FirstOrDefault()?.InnerHtml + "\n");
                this.XPathRemovalList.Select(x => childNode?.SelectNodes(x)?.ToList())
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .ToList()
                    .ForEach(nodeToRemove => nodeToRemove.Remove());

                Dictionary<string, string> keyedData = this.XPathDataKeys.Keys
                    .ToDictionary(x => x,
                    x => childNode.SelectNodes(this.XPathDataKeys[x])
                        ?.Where(y => y != null)
                        .Select(y => y.OriginalName == "img" ? y.Attributes["src"].Value : y.InnerHtml)
                        .ToArray()
                        .Aggregate((a, b) => a + ";" + b)
                );
                
                AggregateData.Add(keyedData);
            }
            return AggregateData;
        }
    }
}
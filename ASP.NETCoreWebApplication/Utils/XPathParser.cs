using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Utils
{
    public class XPathParser
    {
        //THE BEATING HEART OF THE SCRAPPER
        
        //parent nodes
        private readonly string MainXPathItem; //which is list item from HTML
        //childNodes inside parentNodes
        private readonly List<string> XPathRemovalList; //which items contain extra information and can be deleted
        private readonly Dictionary<string, string> XPathDataKeys; //matching the exact data
        //example price -> "//div[contains(@class, 'price')]"
        
        public XPathParser(string mainXPathItem, List<string> xPathRemovalList,
            Dictionary<string, string> xPathDataKeys)
        {
            this.XPathDataKeys = xPathDataKeys;
            this.XPathRemovalList = xPathRemovalList;
            this.MainXPathItem = mainXPathItem;
        }
        
        // input --> html --> XPathParser --> Key/Value object --> output
        // keyFormat is for filtering out the keys we need in output object
        //will return Dictionary<string, dynamic> as obj
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
                Console.Write(childNode.SelectNodes(this.XPathDataKeys["price"]).FirstOrDefault().InnerHtml + "\n");
                //SELECT BY XPATH AND REMOVE FROM HTML NODE - LINQ
                this.XPathRemovalList.Select(x => childNode.SelectNodes(x)?.ToList())
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .ToList()
                    .ForEach(nodeToRemove => nodeToRemove.Remove());

                //returns things like { "Price", "100 000EUR" }
                Dictionary<string, string> keyedData = this.XPathDataKeys.Keys
                    .ToDictionary(x => x,
                    x => childNode.SelectNodes(this.XPathDataKeys[x])
                        .Where(y => y != null)
                        .Select(y => y.OriginalName == "img" ? y.Attributes["src"].Value : y.InnerHtml)
                        .ToArray()
                        .Aggregate((a, b) => a + ";" + b) //Data lists of the same selector are separated with semicolon
                );
                
                //Aggregate 
                AggregateData.Add(keyedData);
            }
            //parse XPath
            return AggregateData;
        }
    }
}
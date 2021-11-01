using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Utils
{
    public class HTMLNodeParser
    {
        //HTML - what you feed
        //ListItemDescendants / className - what you filter out (List)
        //itemsToChoose - what you filter out from
        public static List<Dictionary<string, string>> FeedHTML(string HTML, string ListItemDescendants, string className, Dictionary<String, Tuple<String, String>> itemsToChoose, string? imageClassName = null)
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
                   //DATA COLLECTION
                   string selectorTagName = itemsToChoose[key].Item1;
                   string selectorClassName = itemsToChoose[key].Item2;
                   string data = childNode?.Descendants(selectorTagName)
                       ?.Where(node => node.GetAttributeValue("class", "").Equals(selectorClassName))?.FirstOrDefault()
                       ?.InnerText?.Replace(" ", "").Replace("\n", "").Replace("\r", "");
                   //DATA ENTRY
                   if (data == null)
                   {
                       skipEntry = true;
                   }
                   singleDataItem[key] = data;
                }

                if (imageClassName != null)
                {
                    //image fetch logic
                    string imageSrc = childNode?.Descendants("img")
                        ?.Where(img => img.GetAttributeValue("class", "---none").Equals(imageClassName))
                        ?.FirstOrDefault()
                        ?.GetAttributeValue("src", "");
                    singleDataItem["image"] = imageSrc;
                }

                if (!skipEntry)
                {
                    AggregateData.Add(singleDataItem);
                }
            }
            //parse XPath
            return AggregateData;
        }
    }
}
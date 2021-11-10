using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace ASP.NETCoreWebApplication.Models.DataSources
{

    public enum ItemCategory
    {
        TV,
        ClothingBootsAccessories,
        
    }

    public static class ItemCategoryMapping
    {
        public static string Map(ItemCategory i)
        {
            switch (i)
            {
                case ItemCategory.TV:
                    return "televizoriai";
                case ItemCategory.ClothingBootsAccessories:
                    return "apranga-avalyne-galanterija";
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }
    }
    public class PiguLt
    {
        private static async void ScrapAsync(string itemCategory)
        {
            List<string> Links = new List<string>();
            List<string> Name = new List<string>();
            List<string> Price = new List<string>();
            List<string> Rating = new List<string>();
            var url = "https://pigu.lt/lt/"+itemCategory;
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("product-list all-products-visible clearfix product-list--equal-height")).ToList();

            var ProductListItems = ProductsHtml[0].Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("cover-link")).ToList();


            foreach (var ProductListItem in ProductListItems)
            {
                Links.Add(ProductListItem.GetAttributeValue("href", ""));                
            }

            foreach (string link in Links)
            {
                url = link;
                httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                Name.Add(htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
                try
                {
                    Price.Add(htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("product-price fl notranslate")).FirstOrDefault().InnerText);
                }
                catch(Exception)
                {
                    Price.Add("0");
                }
                try
                {
                    Rating.Add(htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("rating")).FirstOrDefault().InnerText);
                }
                catch(Exception)
                {
                    Rating.Add("?/5");
                }
                }

            for(int i=0; i<Links.Count(); i++)
            {
                Name[i] = Name[i].Replace("&quot", "'");
                Price[i] = Price[i].Replace(" ", "");
                Price[i] = Price[i].Replace("€", "");
                Console.WriteLine(i + ".  Name: " + Name[i]);
                Console.WriteLine("Price: " + (int) Convert.ToDouble(Price[i])/100);
                Console.WriteLine("Rating: " + Rating[i]);
                Console.WriteLine("Link: " + Links[i]);
                Console.WriteLine("___________________________________");
            }
            Console.ReadLine();
        }
    }
}
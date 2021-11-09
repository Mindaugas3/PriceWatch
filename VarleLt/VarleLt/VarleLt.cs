using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace VarleLt
{
    class VarleLt
    {
        static void Main(string[] args)
        {
            Scrap();


            Console.ReadLine();
        }

        private static async void Scrap()
        {
            List<string> Links = new List<string>();
            List<string> Name = new List<string>();
            List<string> Price = new List<string>();
            List<string> Rating = new List<string>();
            var url = "https://www.varle.lt/ismanieji-laikrodziai/";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("grid-items")).ToList();

            var ProductListItems = ProductsHtml[0].Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("img-container")).ToList();

            foreach (var link in ProductsHtml)
            {
                Console.WriteLine(link);
            }
            foreach (var ProductListItem in ProductListItems)
            {
                Links.Add(ProductListItem.GetAttributeValue("href", ""));
            }

            foreach (string link in Links)
            {
                url = "http://www.varle.lt" + link;
                httpClient = new HttpClient();
                Console.WriteLine(url);
                html = await httpClient.GetStringAsync(url);
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                Name.Add(htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
                Console.WriteLine(Name[0]);
                try
                {
                    Price.Add(htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Equals("price-div")).FirstOrDefault().InnerText);
                }
                catch (Exception)
                {
                    Price.Add("0");
                }
                try
                {
                    Rating.Add(htmlDocument.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("counter")).FirstOrDefault().InnerText);
                }
                catch (Exception)
                {
                    Rating.Add("?/5");
                }
            }

            for (int i = 0; i < Links.Count(); i++)
            {
                Name[i] = Name[i].Replace("&quot", "'");
                Price[i] = Price[i].Replace(" ", "");
                Price[i] = Price[i].Replace("€", "");
                Name[i] = Regex.Replace(Name[i], @"\s+", " ");
                Console.WriteLine(i + ".  Name: " + Name[i]);
                Console.WriteLine("Price: " + Convert.ToDouble(Price[i]) / 100);
                Rating[i] = Regex.Replace(Rating[i], @"\s+", " ");
                int index = Rating[i].IndexOf("(");
                if(index >= 0)
                {
                    Rating[i] = Rating[i].Substring(0, index);
                }
                Console.WriteLine("Rating: " + Rating[i]);
                Console.WriteLine("Link: " + Links[i]);
                Console.WriteLine("___________________________________");
            }
            Console.ReadLine();
        }
    }
}

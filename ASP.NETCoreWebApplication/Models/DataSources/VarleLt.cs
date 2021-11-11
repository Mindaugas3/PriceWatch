using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VarleLt
{
    class VarleLt
    {
        static void Main(string[] args)
        {          
            List<string> categories = new List<string>();
            List<string> subCategories = new List<string>();
            GetCategories(categories).Wait();
            foreach(string cat in categories)
            {
                GetSubCategories(cat, subCategories).Wait();
            }
            foreach (var cat in subCategories)
            {
                Scrap(cat);
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
        private static async Task GetSubCategories(string category, List<string>subcat)
        {
            Console.WriteLine("Getting subcategories from: " + category);
            var url = "https://www.varle.lt/" + category;
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html); 

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
              .Where(node => node.GetAttributeValue("class", "")
              .Equals("grid-items")).ToList();

            var Griditems = htmlDocument.DocumentNode.Descendants("a")
              .Where(node => node.GetAttributeValue("class", "")
              .Equals("title-category")).ToList();
            if (Griditems.Count() == 0)
            {
                Console.WriteLine("No sub categories found, adding the category as a sub one.");
                subcat.Add(category);
            }
            else
            {
                foreach (var v in Griditems)
                {
                    subcat.Add(v.GetAttributeValue("href", ""));
                    Console.WriteLine("Kategorija:   " + category + "   ||   SubKategorija:   " + v.GetAttributeValue("href", "").ToString());
                }
            }
            Console.WriteLine();
           
        }
        private static async Task GetCategories(List<string> cat)
        {
            var url = "https://www.varle.lt/visos";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html); //landing-page
            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Equals("categories-new")).ToList();

                var Griditems = htmlDocument.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Equals("grid")).ToList();

            foreach (var v in Griditems)
            {
                cat.AddRange(v.Descendants("a").Select(node => node.GetAttributeValue("href", String.Empty)).ToList());
            }            
            foreach(string str in cat)
            {
                Console.WriteLine(str);
            }
        }
        private static async void Scrap(string Categories)
        {
            Console.WriteLine("Starting:" + Categories);
            List<string> Links = new List<string>();
            List<string> Name = new List<string>();
            List<string> Price = new List<string>();
            List<string> Rating = new List<string>();
            List<string> Pages = new List<string>();
            var url = "https://www.varle.lt/" + Categories;
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

            var pages = ProductsHtml[0].Descendants("div")
                 .Where(node => node.GetAttributeValue("class", "")
                 .Equals("pagination")).ToList();

            var pp = pages[0].Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("page")).ToList();

            foreach (var p in pp)
            {
                Console.WriteLine(p.InnerHtml);
            }
            foreach (var ProductListItem in ProductListItems)
            {
                Links.Add(ProductListItem.GetAttributeValue("href", ""));
            }
         
            foreach (string link in Links)
            {
                url = "http://www.varle.lt" + link;
                httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                Name.Add(htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
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
                Name[i] = Name[i].Replace("&nbsp;", "");
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
                Console.WriteLine("Category: " + Categories);
                Console.WriteLine("___________________________________");
            }
            Console.WriteLine("Ënding: " + Categories);
        }
    }
}

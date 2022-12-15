using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ASP.NETCoreWebApplication.Infrastructure;
using ASP.NETCoreWebApplication.Models;
using ASP.NETCoreWebApplication.Models.Repositories;
using ASP.NETCoreWebApplication.Models.Schemas;
using HtmlAgilityPack;
using Category = ASP.NETCoreWebApplication.Scrappers.Category;

namespace ASP.NETCoreWebApplication.Scrappers
{

    class VarleLt
    {
        public class ItemsObject
        {
            public ItemsObject(string category, string title, string url, string price, string description)
            {
                this.category = category;
                this.title = title;
                this.url = url;
                this.price = price;
                this.description = description;
            }

            public string category;
            public string title;
            public string url;
            public string price;
            public string description;
        }
        private static List<ItemsObject> Items = new List<ItemsObject>();
        private static List<string> Links = new List<string>();
        private static List<string> Name = new List<string>();
        private static List<string> Price = new List<string>();
        private static List<string> Rating = new List<string>();
        private static List<string> categories = new List<string>();
        private static List<string> subCategories = new List<string>();
        private static List<string> CategoriesForDB = new List<string>();
        public static async Task<List<ItemObject>> VarleLT(PriceWatchContext dbc)
        {
            Category CategoryList = new Category();
            
            //subCategories.Add("televizoriai/");
            Console.WriteLine("Getting categories");
            GetCategories(categories).Wait();
            Console.WriteLine("Getting subCategories");
            foreach(string cat in categories.ToList())
            {
                if (cat.Equals("/turizmas/") || cat.Equals("/apple/") || cat.Equals("/ismanieji-namai/") || cat.Equals("/mi-shop/") || cat.Equals("/esporto-zaidimu-gaming-iranga/") || cat.Equals("/loreal/") || cat.Equals("/jura/") || cat.Equals("/samsung/") || cat.Equals(" / sokoladas - saldumynai / "))
                {
                }
                else
                {
                    GetSubCategories(cat, subCategories, CategoryList).Wait();
                }
            }
            foreach (var cat in subCategories.Take(1))
            {
                Console.WriteLine("Rasta galine kategorija:  " + cat);
                // GetLastPage(cat);
                try
                {
                    Scrap2(Links, cat).Wait();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                
            }

            Console.WriteLine("Done.");
            PrintData();
            List<ItemObject> databaseEntries = new List<ItemObject>();
            foreach (ItemsObject I in Items)
            {
                ItemObject obj = DBObjects(I);
                databaseEntries.Add(obj);
            }

            return databaseEntries;
        }
        private static void PrintData()
        {
            for (int i = 0; i < Links.Count(); i++)
            {
                Rating[i] = Regex.Replace(Rating[i], @"\s+", " ");
                int index = Rating[i].IndexOf("(");
                if (index >= 0)
                {
                    Rating[i] = Rating[i].Substring(0, index);
                }

                Name[i] = Name[i].Replace("&quot", "'");
                Price[i] = Price[i].Replace(" ", "");
                Price[i] = Price[i].Replace("€", "");
                Console.WriteLine(i + ".  Name: " + Name[i]);
                Console.WriteLine("Price: " + Price[i]);
                Console.WriteLine("Rating: " + Rating[i]);
                Console.WriteLine("Category: " + CategoriesForDB[i]);
                Logger.WriteHttpGetScrappers(Links[i]);
                Console.WriteLine("___________________________________");
                Items.Add(new ItemsObject(CategoriesForDB[i], Name[i], Links[i], Price[i], Rating[i]));
            }
        }
        private static async Task GetSubCategories(string category, List<string>subcat, Category c)
        {
          
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
            if (Griditems.Count() == 0) // Check if the category has subcategories
            {
                subcat.Add(category);
            }
            else
            {
                foreach (var v in Griditems)
                {
                    subcat.Add(v.GetAttributeValue("href", ""));
                    string name = v.GetAttributeValue("href", "").ToString();
                    c.name = v.GetAttributeValue("href", "").ToString();
                    c.href = "https://www.varle.lt" + v.GetAttributeValue("href", "").ToString();
                 
                }
            }
           
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
        }
        private static async void GetLastPage(string Categories)
        {



            try
            {
                var url = "https://www.varle.lt" + Categories;
                var httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);


                var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("ajax-container")).ToList();
                if (ProductsHtml == null || ProductsHtml.Count() == 0)
                {
                    Console.WriteLine("Error finding pages for  " + Categories);
                }
                else
                {
                    var pages = ProductsHtml[0].Descendants("li")
                         .Where(node => node.GetAttributeValue("class", "")
                         .Equals("for-desktop ")).ToList();
                    String PageHref = "";
                    if (pages == null || pages.Count() == 0)
                    {
                        PageHref = "?p=1";
                    }
                    else
                    {
                        var lastPage = pages.LastOrDefault().InnerText;
                        PageHref = "?p=" + lastPage.Replace(" ", "");
                    }
                    Console.WriteLine(Categories + " has " + PageHref + " pages");
                }
            }
            catch
            {
                Console.WriteLine("Error with  " + Categories + "  LastPage gathering");
            }

        }
        public static async Task Scrap2(List<string> Links, string url)
        {
            Console.WriteLine("Getting info from: " + url);
            url = "https://www.varle.lt/" + url;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")

                .Equals("ajax-container")).ToList();
            if (ProductsHtml == null || ProductsHtml.Count == 0)
            {
                Console.WriteLine("Error getting info from" + url);
            }
            else
            {

                var ProductListItems = ProductsHtml[0].Descendants("div")
                   .Where(node => node.GetAttributeValue("class", "")
                   .Equals("img-container")).ToList();


                foreach (var ProductListItem in ProductListItems)
                {
                    var nodes = ProductListItem.SelectNodes("a[@href]");
                    foreach (var node in nodes)
                    {
                        Links.Add((node.Attributes["href"].Value));
                    }
                }
                Console.WriteLine("Success. Gatthering info.");
                foreach (string link in Links)
                {
                    Logger.WriteHttpGetScrappers(link);
                    url = "https://www.varle.lt"+link;
                    httpClient = new HttpClient();
                    html = await httpClient.GetStringAsync(url);
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    CategoriesForDB.Add(url);
                    try
                    {
                        Name.Add(htmlDocument.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("title")).FirstOrDefault().InnerText);
                    }
                    catch (Exception)
                    {
                        Name.Add(link);
                    }
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



            }
        }
        public static ItemObject DBObjects(ItemsObject I)
        {
            double value;
            I.price.Replace(",", ".");
            double.TryParse(I.price, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            ItemObject dbObject = new ItemObject
            {
                Source_id = 2,
                category = I.category,
                title = I.title,
                url = I.url,
                price = Convert.ToSingle(value),
                shipping = Convert.ToSingle(0),
                shippingDuration = 0,
                Currency_id = 0,
                returns = "",
                weight = Convert.ToSingle(0),
                description = I.description,
                img = null

            };
            return dbObject;
        }
    }
}

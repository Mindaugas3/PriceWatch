using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ASP.NETCoreWebApplication.Interactors;
using ASP.NETCoreWebApplication.Utils;
using System.Globalization;

namespace ASP.NETCoreWebApplication.Models.DataSources
{
    public class Category
    {
        public string name;
        public string sourceID = "https://www.pigu.lt/";
        public string href;
    }

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
    class PiguLt
    {
        private static List<ItemsObject> Items = new List<ItemsObject>();
        private static List<string> SubCategoryURL = new List<string>();
        private static List<string> Links = new List<string>();
        private static List<string> CategoryURL = new List<string>();
        private static List<string> Name = new List<string>();
        private static List<string> Price = new List<string>();
        private static List<string> Rating = new List<string>();
        private static List<string> CategoryForDB = new List<string>();
        private static List<string> PictureURL = new List<string>();
        static async Task PiguLT(string[] args, PriceWatchContext dbc)
        {

            await GetCategories(CategoryURL);
            int timer = 0;
            foreach (var v in CategoryURL)
            {
                Console.WriteLine(timer);
                timer++;
                try
                {
                    //Console.WriteLine("Getting info about " + v);
                    await GetSubCategories(v);
                }
                catch (Exception)
                {
                    SubCategoryURL.Add(v);
                }
            }

            Console.WriteLine("Found " + SubCategoryURL.Count() + "  subcategories");

            int count = 0;
            foreach (var v in SubCategoryURL.Take(5))
            {
                count++;
                Console.WriteLine(count);
                Console.WriteLine("SubcategoryURL: " + v);
                try
                {

                    GetHtmlAsync(Links, v).Wait();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("429 too many requests");
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Invalid URI at  " + v);
                    Console.WriteLine(e.Message);
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Done.");
            PrintData();
            List<ItemObject> databaseEntries = new List<ItemObject>();
            foreach(ItemsObject I in Items)
            {
                ItemObject obj = DBObjects(I);
                databaseEntries.Add(obj);
            }
            
            PWDatabaseInitializer.InsertItems(dbc, databaseEntries);
            Console.ReadLine();
        }
        private static void PrintData()
        {
            for (int i = 0; i < Links.Count(); i++)
            {
                Name[i] = Name[i].Replace("&quot", "'");
                Price[i] = Price[i].Replace(" ", "");
                Price[i] = Price[i].Replace("€", "");
                Console.WriteLine(i + ".  Name: " + Name[i]);
                Console.WriteLine("Price: " + Convert.ToDouble(Price[i]) / 100);

                Rating[i] = Regex.Replace(Rating[i], @"\s+", " ");
                Console.WriteLine("Rating: " + Rating[i]);

                int index = CategoryForDB[i].LastIndexOf("/");
                CategoryForDB[i] = CategoryForDB[i].Substring(0, CategoryForDB[i].LastIndexOf("/", CategoryForDB[i].LastIndexOf("/") - 1));
                CategoryForDB[i] = CategoryForDB[i].Substring(CategoryForDB[i].LastIndexOf("/") + 1);
                CategoryForDB[i] = CategoryForDB[i].Replace("-", " ");
                Console.WriteLine("Category: " + CategoryForDB[i]);

                Console.WriteLine("Link: " + Links[i]);
                Console.WriteLine("Img URL: " + PictureURL[i]);
                Console.WriteLine("___________________________________");

                // category title url price description img
                Items.Add(new ItemsObject(CategoryForDB[i],Name[i],Links[i],Price[i],Rating[i]));
            }
        }

        private static async Task GetHtmlAsync(List<string> Links, string url)
        {
            Console.WriteLine("Getting info from: " + url);


            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("product-list all-products-visible clearfix product-list--equal-height")).ToList();
            if (ProductsHtml == null || ProductsHtml.Count == 0)
            {
                Console.WriteLine("Error getting info from" + url);
            }
            else
            {

                var ProductListItems = ProductsHtml[0].Descendants("a")
                   .Where(node => node.GetAttributeValue("class", "")
                   .Equals("cover-link")).ToList();


                foreach (var ProductListItem in ProductListItems)
                {
                    Links.Add(ProductListItem.GetAttributeValue("href", ""));
                }
                Console.WriteLine("Success. Gatthering info.");
                foreach (string link in Links)
                {
                    url = link;
                    httpClient = new HttpClient();
                    html = await httpClient.GetStringAsync(url);
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    CategoryForDB.Add(url);
                    try
                    {
                        Name.Add(htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
                    }
                    catch (Exception)
                    {
                        Name.Add(link);
                    }
                    try
                    {
                        Price.Add(htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("product-price fl notranslate")).FirstOrDefault().InnerText);
                    }
                    catch (Exception)
                    {
                        Price.Add("0");
                    }
                    try
                    {

                        Rating.Add(htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("rating")).FirstOrDefault().InnerText);

                    }
                    catch (Exception)
                    {
                        Rating.Add("?/5");
                    }
                    try
                    {
                        var ImageURL = htmlDocument.DocumentNode.Descendants("div")
                  .Where(node => node.GetAttributeValue("class", "")
                  .Equals("cl img_side")).ToList();
                        string temp = "";
                        foreach (var v in ImageURL)
                        {
                            temp = v.InnerHtml;
                        }
                        int pFrom = temp.IndexOf("cfsrc = ") + "cfsrc = ".Length;
                        int pTo = temp.IndexOf("alt=") - 1;
                        String result = temp.Substring(pFrom + 3, pTo - pFrom - 4);
                        //<img data-cfsrc="https://lt1.pigugroup.eu/colours/459/308/13/45930813/lassie-zieminis-lauko-komplektas-raiku-juodas-723751_xsmall.jpg" alt='' style="display:none;visibility:hidden;"><noscript><img src='https://lt1.pigugroup.eu/colours/459/308/13/45930813/lassie-zieminis-lauko-komplektas-raiku-juodas-723751_xsmall.jpg' alt=''></noscript>
                        //Name.Add(htmlDocument.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
                        PictureURL.Add(result);
                    }
                    catch (Exception e)
                    {
                        PictureURL.Add("Unknown Picture URL");
                        Console.WriteLine(e.Message);
                    }

                }



            }
        }

        private static async Task GetCategories(List<string> CategoryURL)
        {
            var url = "https://pigu.lt/lt/katalogas";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);


            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("category-listing")).ToList();

            var ProductListItems = ProductsHtml[0].Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("subcategory-list")).ToList();

            foreach (var v in ProductListItems)
            {
                Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
                Match match;
                for (match = regex.Match(v.InnerHtml); match.Success; match = match.NextMatch())
                {
                    foreach (Group group in match.Groups)
                    {
                        CategoryURL.Add(group.ToString());
                    }
                }
            }
            CategoryURL.RemoveAll(x => x.Contains("href="));
        }
        private static async Task GetSubCategories(string link)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(link);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Equals("category-list all-categories-visible")).ToList();

            var ProductsListItems = ProductsHtml[0].Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("category-list-item-wrap all-categories-visible")).ToList();

            foreach (var v in ProductsListItems)
            {
                SubCategoryURL.Add(v.SelectSingleNode("a").Attributes["href"].Value);
            }
        }
        public static ItemObject DBObjects(ItemsObject I)
        {
            double value;
            I.price.Replace(",",".");
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

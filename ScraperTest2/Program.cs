using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScraperTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }

        private static async void GetHtmlAsync()
        {
            var url = "https://www.ebay.com/sch/i.html?_nkw=nintendo+switch&_in_kw=1&_ex_kw=&_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=94040&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=200";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ProductsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            var ProductListItems = ProductsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            Console.WriteLine(ProductListItems.Count() + " Listings");
            Console.WriteLine();

            foreach (var ProductListItem in ProductListItems)
            {
                //Listing ID
                Console.WriteLine("Listing ID: " + ProductListItem.GetAttributeValue("listingid", ""));

                //Product Name
                Console.WriteLine("Product Name: " + ProductListItem.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvtitle")).FirstOrDefault().InnerText.Replace("New listing", String.Empty).Replace("\n", String.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty)
                    );

                //Product Price
                Console.WriteLine("Price: " + 
                    Regex.Match(
                        ProductListItem.Descendants("li")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("lvprice prc")).FirstOrDefault().InnerText
                    ,@"\d+.\d+")
                    );


                //Listing Type
                Console.WriteLine("# of Bids or Buy it Now: " + ProductListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvformat")).FirstOrDefault().InnerText.Replace("\n", String.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty)
                    );


                //Product URL
                Console.WriteLine("Product URL: " + 
                        ProductListItem.Descendants("a")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("vip")).FirstOrDefault().GetAttributeValue("href", "")
                        );

                Console.WriteLine();
            }

        }
    }
}
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace WebScrapperVezba
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("What would you like to search for: ");
                string s = Console.ReadLine();
                GetHtmlAsync(s);
            }
            //GetHtmlAsync();
            
        }

        private static async void GetHtmlAsync(string userInput)
        {
            userInput.Replace(" ", "+");
            string url = $"https://www.ebay.com/sch/i.html?_from=R40&_nkw= {userInput} &_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sargn=-1%26saslc%3D1&_salic=1&_sop=13&_dmd=1&_ipg=50&_fosrp=1".Replace(" ","");

            HttpClient httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            //parse data
            HtmlDocument htmlDocumentation = new HtmlDocument();
            htmlDocumentation.LoadHtml(html);

            //targeting what u want to scrap
            var ProductsHtml = htmlDocumentation.DocumentNode.Descendants("ul")
                .Where(x => x.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            //search for li that contains id="item"
            var productListItems = ProductsHtml[0].Descendants("li")
                .Where(x => x.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            //grab data from each item
            foreach (var productListItem in productListItems)
            {
                //id
                Console.WriteLine(productListItem.GetAttributeValue("listingid", ""));

                //name
                Console.WriteLine(productListItem.Descendants("h3")
                    .Where(x => x.GetAttributeValue("class", "")
                    .Equals("lvtitle"))
                    .FirstOrDefault().InnerText.Trim('\r', '\n', '\t'));

                //price lvprice prc
                Console.WriteLine(
                    Regex.Match(productListItem.Descendants("li")
                    .Where(x => x.GetAttributeValue("class", "")
                    .Equals("lvprice prc"))
                    .FirstOrDefault().InnerText.Trim('\r','\n','\t')
                    ,@"\d+.\d+"));

                //link to the product
                Console.WriteLine(productListItem.Descendants("a")
                    .FirstOrDefault().GetAttributeValue("href","")+"\n");

            }

        }
    }
}

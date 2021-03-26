using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseDNS databaseDNS = new DatabaseDNS();
            string siteName = "https://www.dns-shop.ru";
            string city = "Барнаул";
            string cityId = "49bc7ffa-ddec-11dc-8709-00151716f9f5";
            int maxProductsOnPage = 18;

            Dictionary<string, string> categories = databaseDNS.GetActualCategoriesFromBase();
            SearcherDNS searcherDNS = new SearcherDNS();
            searcherDNS.InitSearcher(siteName);
            searcherDNS.SetCity(cityId);

            foreach (KeyValuePair<string, string> category in categories)
            {
                int counter = 0;
                while (counter < 3)
                {
                    try
                    {
                        var productData = searcherDNS.GetPricesFromCategory(category.Key, maxProductsOnPage);
                        foreach (Product product in productData)
                        {
                            databaseDNS.AddPrice(link: product.Link, name: product.Name, category: category.Value, priceText: product.Price, city: city, site: siteName);;
                        }
  
                        databaseDNS.UpdateStructureUrls(category.Key);
                        counter = 3;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.StackTrace + " || " + DateTime.Now + " || " + e.Message);
                        searcherDNS.InitSearcher(siteName);
                        searcherDNS.SetCity(city);
                        counter++;
                    }
                }              
            }
        }
    }
}

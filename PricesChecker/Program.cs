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

            Dictionary<string, string> categories = databaseDNS.GetActualCategoriesFromBase();
            SearcherDNS searcherDNS = new SearcherDNS();
            searcherDNS.InitSearcher(siteName);
            searcherDNS.SetCity(city);

            foreach (KeyValuePair<string, string> category in categories)
            {
                int counter = 0;
                while (counter < 3)
                {
                    try
                    {
                        var productData = searcherDNS.GetPricesFromCategory(category.Key);
                        foreach (Product product in productData)
                        {
                            databaseDNS.AddPrice(product.Link, product.Name, category.Value, product.Price, city, siteName);
                        }
                        databaseDNS.UpdateStructureUrls(category.Key);
                        counter = 3;
                    }
                    catch
                    {
                        searcherDNS.InitSearcher(siteName);
                        searcherDNS.SetCity(city);
                        counter++;
                    }
                }

                
            }
        }
    }
}

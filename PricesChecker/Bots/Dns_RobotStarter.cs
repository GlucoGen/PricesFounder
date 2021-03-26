using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesChecker
{
    class DnsRobotStarter
    {

        private string siteName;
        private string city;
        private string cityId; 
        private int maxProductsOnPage;

        public DnsRobotStarter(string _site, string _city, string _cityId, int max)
        {

            siteName = _site;
            city = _city;
            cityId = _cityId;
            maxProductsOnPage = max;
        }
        public void startDnsRobot()
        {
            DatabaseDNS databaseDNS = new DatabaseDNS();
            Dictionary<string, string> categories = databaseDNS.GetActualCategoriesFromBase();
            SearcherDNS searcherDNS = new SearcherDNS();
            searcherDNS.InitSearcher(siteName);
            searcherDNS.SetCity(cityId);

            foreach (KeyValuePair<string, string> category in categories)
            {

                for (int counter = 0;  counter < 3; counter++)
                {
                    try
                    {
                        var productData = searcherDNS.GetPricesFromCategory(category.Key, maxProductsOnPage);
                        foreach (Product product in productData)
                        {
                            databaseDNS.AddPrice(link: product.Link, name: product.Name, category: category.Value, priceText: product.Price, city: city, site: siteName); ;
                        }

                        databaseDNS.UpdateStructureUrls(category.Key);
                        break;
                    }
                    catch (Exception e)
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

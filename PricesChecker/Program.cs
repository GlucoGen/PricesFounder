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
            SQL sql = new SQL();
            string siteName = "https://www.dns-shop.ru";
            string city = "Барнаул";

            Dictionary<string, string> goods = sql.GetActualGoodsFromBase();
            Searcher search = new Searcher();
            search.InitSearcher(siteName);
            search.SetCity(city);
            List<Product> productData = new List<Product>();

            foreach (KeyValuePair<string, string> Link in goods)
            {
                int counter = 0;
                while (counter < 3)
                {
                    try
                    {
                        productData = search.GetPricesFromCategory(Link.Key);
                        foreach (Product pd in productData)
                        {
                            sql.AddPrice(pd.Link, pd.Name, Link.Value, pd.Price, city, siteName);
                        }
                        sql.UpdateStructureUrls(Link.Key);
                        counter = 3;
                    }
                    catch
                    {
                        search.InitSearcher(siteName);
                        search.SetCity(city);
                        counter++;
                    }
                }

                
            }
        }
    }
}

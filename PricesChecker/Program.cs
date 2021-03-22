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

            foreach (KeyValuePair<string, string> Link in goods)
            {
                search.GoToUrl(Link.Key);
                search.ExpandCategory();
                Dictionary<string, string> productData = search.GetPricesFromCategory();
                foreach (KeyValuePair<string, string> pd in productData)
                {
                    sql.AddPrice(Link.Key, pd.Key, Link.Value, pd.Value,city, siteName);                    
                }
                sql.UpdateStructureUrls(Link.Key);
            }
        }
    }
}

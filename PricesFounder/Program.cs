using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesFounder
{
    class Program
    {
        static void Main(string[] args)
        {
            Searcher searchDNS = new Searcher();
            searchDNS.InitSearcher("https://www.dns-shop.ru");

            var dnsStucture = searchDNS.GetSiteStructure();
            SQL sql = new SQL();
            sql.InsertToBase(dnsStucture);


            Console.ReadKey();
        }
    }
}

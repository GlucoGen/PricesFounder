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
            Searcher searchDNS = new Searcher("https://www.dns-shop.ru");

            searchDNS.KillSearcher();
            Console.ReadKey();
        }
    }
}

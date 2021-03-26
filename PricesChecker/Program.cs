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
            DnsRobotStarter dnsBot = new DnsRobotStarter(_site: "https://www.dns-shop.ru", _city: "Барнаул", _cityId: "49bc7ffa-ddec-11dc-8709-00151716f9f5", max: 18);
            dnsBot.startDnsRobot();
        }
    }   
}

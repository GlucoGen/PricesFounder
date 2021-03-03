using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PricesFounder
{
    class Searcher
    {
        private string SiteUrl;
        private IWebDriver Driver;
        private string ChromeDriverPath = @"D:\Chromedriver\88";
        Dictionary<string, string> SiteStructure = new Dictionary<string, string>();

        public void InitSearcher(string url)
        {
            KillSearcher();
            //Убиваем Хром

            SiteUrl = url;

            //запускаем Хром
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(ChromeDriverPath);
            driverService.HideCommandPromptWindow = true;

            TimeSpan timeout = new TimeSpan(0, 5, 0);

            Driver = new ChromeDriver(driverService, options, timeout);
            Driver.Navigate().GoToUrl(SiteUrl);

            //WaitElement("//input[@placeholder='Поиск по сайту111']", 10);
        }


        protected void KillSearcher()
        {
            try
            {
                Driver.Close();
            }
            catch (Exception e)
            {
                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }

                foreach (var process in Process.GetProcessesByName("chrome"))
                {
                    process.Kill();
                }
            }

        }

        protected bool WaitElement(string xpath, int timeout)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                IWebElement element = wait.Until(d => d.FindElements(By.XPath(xpath)).FirstOrDefault());
                return true;
            }

            catch (Exception e)
            {
                return false;
                //throw new Exception("Элемент не найден в течение " + timeout + " секунд: " + xpath);
            }
        }

        protected void Click(string xpath, int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                Driver.FindElement(By.XPath(xpath)).Click();
            }
            else throw new Exception("Не удалось кликнуть. Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        protected string GetText(string xpath, int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                return Driver.FindElement(By.XPath(xpath)).Text;
            }
            else throw new Exception("Не удалось считать текст. Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        protected Dictionary<string, string> GetDnsStucture()
        {
            Dictionary<string, string> structure = new Dictionary<string, string>();

            bool firstMenu = WaitElement("//div[@class='menu-desktop__root']", 10);
            if (firstMenu)
            {
                var menu = Driver.FindElements(By.XPath("//div[@class='menu-desktop__root']"));
                if (menu.Count > 0)
                {
                    for (int i = 9; i < menu.Count; i++)
                    {
                        InitSearcher(SiteUrl);
                        WaitElement("//div[@class='menu-desktop__root']",10);
                        IWebElement element = Driver.FindElement(By.XPath("//div[@class='menu-desktop__root'][" + (i + 1) + "]//div[@class='menu-desktop__root-info']/a"));
                        string href = element.GetAttribute("href");
                        string category = element.Text;
                        Dictionary<string, string> structureTmp = GetRecursiveStructure(href, category, SiteUrl);
                        foreach (KeyValuePair<string, string> kvp in structureTmp)
                        {
                            if (!structureTmp.ContainsKey(kvp.Key)) structure.Add(kvp.Key, kvp.Value);
                        }
                        try
                        {
                            Driver.Url = SiteUrl;
                        }
                        catch (OpenQA.Selenium.WebDriverException e)
                        {
                            InitSearcher(SiteUrl);
                        }
                    }
                }

                return structure;
            }
            else throw new Exception("Не удалось найти меню сайта. Элемент не найден в течение " + 10 + " секунд: " + "//span[@class='catalog-spoiler']");
        }

        protected Dictionary<string, string> GetRecursiveStructure(string link, string category, string prevUrl)
        {
            int i = 0;
            Dictionary<string, string> structureResult = new Dictionary<string, string>();
            try
            {
                Driver.Url = link;
            }
            catch (OpenQA.Selenium.WebDriverException e)
            {
                InitSearcher(link);
            }

            WaitElement("//div[@class='subcategory']", 1);
            var subs = Driver.FindElements(By.XPath("//div[@class='subcategory']/a"));
            if (subs.Count > 0)
            {
                for (; i < subs.Count; i++)
                {
                    
                    WaitElement("//div[@class='subcategory']/a[" + (i + 1) + "]", 1);
                    IWebElement element;

                    try
                    {
                        element = Driver.FindElement(By.XPath("//div[@class='subcategory']/a[" + (i + 1) + "]"));
                    }
                    catch (Exception)
                    {
                        try
                        {
                            Driver.Url = link;
                        }
                        catch (OpenQA.Selenium.WebDriverException e)
                        {
                            InitSearcher(link);
                        }
                        WaitElement("//div[@class='subcategory']", 1);
                        subs = Driver.FindElements(By.XPath("//div[@class='subcategory']/a"));
                        element = Driver.FindElement(By.XPath("//div[@class='subcategory']/a[" + (i + 1) + "]"));
                    }

                    string href = element.GetAttribute("href");
                    Dictionary<string, string> structureTMP =  GetRecursiveStructure(href, element.Text, link);
                    foreach (KeyValuePair<string, string> kvp in structureTMP)
                    {
                        if(!structureResult.ContainsKey(kvp.Key)) structureResult.Add(kvp.Key, kvp.Value);
                    }
                }

                try
                {
                    Driver.Url = prevUrl;
                }
                catch (OpenQA.Selenium.WebDriverException e)
                {
                    InitSearcher(link);
                }
                //Driver.Navigate().Back();
                //Thread.Sleep(2000);
                return structureResult;
            }
            else
            {
                if (link == "https://www.dns-shop.ru/catalog/4fe024233ecb7fd7/aksessuary-k-plitam/")
                { 
                    int a = 2; 
                }
                try
                {
                    Driver.Url = prevUrl;
                }
                catch (OpenQA.Selenium.WebDriverException e)
                {
                    InitSearcher(link);
                }
                //Driver.Navigate().Back();
                //Thread.Sleep(2000);
                Dictionary<string, string> structure = new Dictionary<string, string>
                {
                    { category, link }
                };
                return structure;
            }
        }

        public Dictionary<string, string> GetSiteStructure()
        {
            if (SiteUrl.IndexOf("dns-shop") > 0)
            {
                GetDnsStucture();
                return null;
            }
            return null;
        }


    }
}


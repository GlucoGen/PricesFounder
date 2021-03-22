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

namespace PricesChecker
{
    class Searcher
    {
        private string SiteUrl;
        private IWebDriver Driver;
        private string ChromeDriverPath = @"D:\Chromedriver\88";
        Dictionary<string, string> Prices = new Dictionary<string, string>();
        int shortTimeout = 5;

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
        }


        protected void KillSearcher()
        {
            try
            {
                Driver.Close();
            }
            catch (Exception)
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

            catch (Exception)
            {
                return false;
            }
        }

        protected void Click(string xpath, int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                try
                {
                    Driver.FindElement(By.XPath(xpath)).Click();
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(2000);
                    Driver.FindElement(By.XPath(xpath)).Click();
                }
                return;
            }
            throw new Exception("Не удалось кликнуть. Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        public void GoToUrl(string link)
        {
            //Driver.Url = link;
            Driver.Navigate().GoToUrl(link);
        }

        protected string GetText(string xpath, int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                return Driver.FindElement(By.XPath(xpath)).Text;
            }

            throw new Exception("Не удалось считать текст. Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        public Dictionary<string, string> GetPricesFromCategory()
        {
            Dictionary<string, string> productData = new Dictionary<string, string>();         
            int i = 1;
    
            string nameTemplate = "(//div[@data-id='product'])[@NAME@]/a/span";
            string priceTemplate = "((//div[@data-id='product'])[@NAME@]//div[contains(@class,'product-buy__price')])[last()]";
            string allItems = "//div[@data-id='product']";

            //надо вынести проверку числа в отдельный метод, который повторно вызывает разворачивание в случае неравенства
            if (WaitElement(allItems, shortTimeout))
            {
                int countItems = CheckCount();
                var totalItems = Driver.FindElements(By.XPath(allItems));

                if (totalItems.Count == countItems)
                {
                    string curElement = nameTemplate.Replace("@NAME@", "" + i);

                    while (WaitElement(curElement, shortTimeout))
                    {
                        string curPrice = priceTemplate.Replace("@NAME@", "" + i);

                        IWebElement NameElement = Driver.FindElement(By.XPath(curElement));
                        string name = NameElement.Text;

                        IWebElement PriceElement = Driver.FindElement(By.XPath(curPrice));
                        string price = PriceElement.Text;

                        productData.Add(name, price);

                        i++;
                        curElement = nameTemplate.Replace("@NAME@", "" + i);
                    }


                    return productData;
                }
                return null;
            }

            return null;
        }

        protected int CheckCount()
        {
            string count = "//span[@data-role='items-count']";
            if (WaitElement(count, shortTimeout))
            {
               string countText = GetText(count, shortTimeout);
               string productsCount = countText.Split(' ')[0];
               int countGoods = Convert.ToInt32(productsCount);

               return countGoods;

            }
            return 0;
        }

            
            public void SetCity(string city)
        {
            string chooseCity = "//a[text()='Выбрать другой']";

            if (WaitElement(chooseCity, 10))
            {
                SetInputCity(chooseCity,city);
            }
        }

        protected void SetInputCity(string chooseCity, string city)
        {
            string inputCity = "//input[@data-role='search-city']";
            Click(chooseCity, shortTimeout);
            Thread.Sleep(1000);
            if (WaitElement(chooseCity, shortTimeout))
            {
                IWebElement element = Driver.FindElement(By.XPath(inputCity));
                element.SendKeys(city);
                element.SendKeys(Keys.Enter);
                Thread.Sleep(5000);
            }
        }
        public void ExpandCategory()
        {
            string showMoreButton = "//button[@data-role='show-more-btn']";

            while (WaitElement(showMoreButton, shortTimeout))
            {
                Click(showMoreButton,10);
                Thread.Sleep(2000);
            }
        }
    }
}

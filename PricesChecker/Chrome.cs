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
    class Chrome
    {
        private IWebDriver Driver;
        private string ChromeDriverPath = @"D:\Chromedriver\88";
        Dictionary<string, string> Prices = new Dictionary<string, string>();
        int shortTimeout = 5;
        int longTimeout = 120;

        public Chrome(string url)
        {
            KillSearcher();
            //Убиваем Хром

            //запускаем Хром
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(ChromeDriverPath);
            driverService.HideCommandPromptWindow = true;
            TimeSpan timeout = new TimeSpan(0, 5, 0);

            Driver = new ChromeDriver(driverService, options, timeout);
            Driver.Navigate().GoToUrl(url);
            Driver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(0, 15, 0);
        }

        public void KillSearcher()
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

        public bool WaitElement(string xpath, int timeout)
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

        public void Click(string xpath, int timeout)
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

        public string GetText(string xpath, int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                return Driver.FindElement(By.XPath(xpath)).Text;
            }

            throw new Exception("Не удалось считать текст. Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        public IWebElement FindElement(string xpath)
        {
            return Driver.FindElement(By.XPath(xpath));           
        }
        public void Refresh()
        {
            Driver.Navigate().Refresh();
        }

        public void UpdateCurentLink()
        {
            string currentLink = Driver.Url;
            Driver.Navigate().GoToUrl(currentLink);
        }

        public void Sleep(int seconds)
        {
            Thread.Sleep(seconds*1000);
        }


    }
}

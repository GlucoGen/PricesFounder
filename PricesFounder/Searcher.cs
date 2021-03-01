using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesFounder
{
    class Searcher
    {
        private string SiteName;
        private IWebDriver Driver;
        private string ChromeDriverPath = @"D:\Chromedriver\88";
        public Searcher(string name)
        {
            KillSearcher();
            //Убиваем Хром

            SiteName = name;

            //запускаем Хром
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(ChromeDriverPath);
            driverService.HideCommandPromptWindow = true;

            TimeSpan timeout = new TimeSpan(0, 5, 0);

            Driver = new ChromeDriver(driverService, options, timeout);
            Driver.Navigate().GoToUrl(SiteName);

            WaitElement("//input[@placeholder='Поиск по сайту111']", 10);
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


    }
}


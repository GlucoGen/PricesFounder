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

        public Chrome(string url)
        {
            KillChrome();

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

        public void KillChrome()
        {
            if (Driver != null)
            {
                Driver.Close();
            }
            
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
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

        public void Click(string xpath)
        {
            int timeout = 0;
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

        public string GetText(string xpath)
        {
            int timeout = 0;
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

        public IWebElement FindElement(string xpath,int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                return Driver.FindElement(By.XPath(xpath));
            }
            throw new Exception("Не удалось получить за "+timeout+"секунд элемент: " + xpath);
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

        public string GetAttribute(string xpath, string attributeName,int timeout)
        {
            if (WaitElement(xpath, timeout))
            {
                return Driver.FindElement(By.XPath(xpath)).GetAttribute(attributeName);
            }
           
            throw new Exception("Не удалось взять атрибут "+attributeName+". Элемент не найден в течение " + timeout + " секунд: " + xpath);
        }

        public int CountElements(string xpath)
        {
            var elements = Driver.FindElements(By.XPath(xpath));
            return elements.Count;
        }

        public void executeJS(string js)
        {
            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)Driver;
            scriptExecutor.ExecuteScript(js);
        }

    }
}

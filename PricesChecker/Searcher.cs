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

    //структура товара - имя, ссылка, цена
    struct Product
    {
        public string Name;
        public string Link;
        public string Price;
    }

    class Searcher
    {
        private string SiteUrl;
        Chrome chrome;
        Dictionary<string, string> Prices = new Dictionary<string, string>();
        int shortTimeout = 5;
        int longTimeout = 120;

        //инициализация
        public void InitSearcher(string url)
        {
            SiteUrl = url;
            chrome = new Chrome(SiteUrl);        
        }

        //метод получения всех товаров со всех страниц, перелистывает их по очереди
        public List<Product> GetPricesFromCategory(string link)
        {
            List<Product> productData = new List<Product>();
            string pageTamplate = "//a[@class='pagination-widget__page-link' and text()='@PAGE@']";
            int j = 1;
            int counter = 0;

           chrome.GoToUrl(link);

            while (counter < 3)
            {
                try
                {
                    string currentPage = pageTamplate.Replace("@PAGE@", j.ToString());
                    while (chrome.WaitElement(currentPage, 5))
                    {
                        chrome.Click(currentPage, 5);
                        chrome.Sleep(2);
                        List<Product> structureTMP = GetAllPricesAndNames();
                        productData.AddRange(structureTMP);
                        counter = 0;
                        j++;
                        currentPage = pageTamplate.Replace("@PAGE@", j.ToString());
                    }
                    if (j == 1)
                    {
                        List<Product> structureTMP = GetAllPricesAndNames();
                        productData.AddRange(structureTMP);
                    }
                    counter = 3;
                }
                catch
                {
                    chrome.UpdateCurentLink();
                    chrome.Sleep(2);
                    counter++;
                }

            }
            return productData;
        }

        //метод получения всех товаров с одной страницы
        public List<Product> GetAllPricesAndNames()
        {
            int i = 1;
            List <Product> products = new List<Product>();

            string nameTemplate = "(//div[@data-id='product'])[@NAME@]/a/span";
            string hrefTemplate = "(//div[@data-id='product'])[@NAME@]/a";
            string priceTemplate = "((//div[@data-id='product'])[@NAME@]//div[contains(@class,'product-buy__price')])[last()]";
            string curElement = nameTemplate.Replace("@NAME@", i.ToString());
            string curHref = hrefTemplate.Replace("@NAME@", i.ToString());

            while (chrome.WaitElement(curElement, shortTimeout))
            {
                string curPrice = priceTemplate.Replace("@NAME@", i.ToString());

                IWebElement NameElement = chrome.FindElement(curElement);
                string name = NameElement.Text;

                chrome.WaitElement(curPrice, longTimeout);
                IWebElement PriceElement = chrome.FindElement(curPrice);
                string price = PriceElement.Text;
                if (price.Trim().Length == 0)
                {
                    chrome.Refresh();
                    chrome.Sleep(2);
                    chrome.WaitElement(curPrice, longTimeout);
                    PriceElement = chrome.FindElement(curPrice);
                    price = PriceElement.Text;
                }

                chrome.WaitElement(curHref, shortTimeout);
                IWebElement hrefElement = chrome.FindElement(curHref);
                string href= hrefElement.GetAttribute("href");

                Product curProduct = new Product();
                curProduct.Name = name;
                curProduct.Link = href;
                curProduct.Price = price;

                if (!products.Contains(curProduct)) products.Add(curProduct);
                //if (!productData.ContainsKey(name)) productData.Add(name, price);

                i++;
                curElement = nameTemplate.Replace("@NAME@", i.ToString());
                curHref = hrefTemplate.Replace("@NAME@", i.ToString());
            }


            return products;
        }
  

        //метод установки текущего города, нажимает на Выбрать другой    
        public void SetCity(string city)
        {
            string chooseCity = "//a[text()='Выбрать другой']";        

            if (chrome.WaitElement(chooseCity, 10))
            {
                SetInputCity(chooseCity,city);
            }
        }

        //метод установки текущего города, выбирает нужный город 
        protected void SetInputCity(string chooseCity, string city)
        {
            string inputCity = "//input[@data-role='search-city']";
            chrome.Click(chooseCity, shortTimeout);
            chrome.Sleep(5);
            if (chrome.WaitElement(inputCity, shortTimeout))
            {
                IWebElement element = chrome.FindElement(inputCity);
                element.SendKeys(city);
                element.SendKeys(Keys.Enter);
                chrome.Sleep(5);
            }
        }
    }
}

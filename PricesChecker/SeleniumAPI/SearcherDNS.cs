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

        public Product(string name, string link, string price)
        {
            Name = name;
            Link = link;
            Price = price;
        }
    }

    class SearcherDNS
    {
        private string SiteUrl;
        Chrome chrome;
        int shortTimeout = 5;
        int longTimeout = 120;

        //инициализация
        public void InitSearcher(string url)
        {
            SiteUrl = url;
            chrome = new Chrome(SiteUrl);        
        }

        //метод получения всех товаров со всех страниц, перелистывает их по очереди
        public List<Product> GetPricesFromCategory(string link,int maxElementsOnPage)
        {
            List<Product> productData = new List<Product>();
            string pageTamplate = "//a[@class='pagination-widget__page-link' and text()='@PAGE@']";
            string firstNameXPath = "(//div[@data-id='product'])[1]";
            string newFistNameXPathTemplate = "(//div[@data-id='product'])[1][not(@data-product='@NAME@')]";
            string totalPagesXPath = "//ul[@class='pagination-widget__pages']/li[last()]";
            string totalProductsInCategoryXPath = "//span[@data-role='items-count']";
            int j = 0;
            int counter = 0;
            string firstName = "";
            int totalPages = 0;

            chrome.GoToUrl(link);

            string textCount = chrome.GetText(totalProductsInCategoryXPath,shortTimeout).Split(' ')[0];
            if (!Int32.TryParse(textCount, out int totalProducts)) throw new Exception("Не удалось взять количество продуктов в категории  :" + totalProductsInCategoryXPath);

            if (totalProducts > maxElementsOnPage)
            {
                j = 1;
                totalPages = Convert.ToInt32(chrome.GetAttribute(totalPagesXPath, "data-page-number", shortTimeout));
            }
            //if (chrome.WaitElement(totalPagesXPath,shortTimeout)) totalPages = Convert.ToInt32(chrome.GetAttribute(totalPagesXPath, "data-page-number", shortTimeout));

          while (counter < 3) 
            {
                try
                {
                    string currentPage = pageTamplate.Replace("@PAGE@", j.ToString());

                    while ((j>0) && (j<= totalPages))
                    {
                        if (j != 1) firstName = chrome.GetAttribute(firstNameXPath, "data-product", shortTimeout);

  
                        string fistNameXPath = newFistNameXPathTemplate.Replace("@NAME@", firstName);
                       

                        chrome.Click(currentPage, shortTimeout);


                        if (!chrome.WaitElement(fistNameXPath, shortTimeout))
                        {
                            throw new Exception("Не удалось взять первый товар на странице: " + fistNameXPath);
                        }

                        List<Product> structureTMP = GetPricesFromOnePage();
                        productData.AddRange(structureTMP);

                        counter = 0;
                        j++;
                       
                        currentPage = pageTamplate.Replace("@PAGE@", j.ToString());
                        totalPages = Convert.ToInt32(chrome.GetAttribute(totalPagesXPath, "data-page-number",shortTimeout));
                    } 

                    if (j == 0)
                    {
                        List<Product> structureTMP = GetPricesFromOnePage();
                        productData.AddRange(structureTMP);
                    }
                    counter = 3;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace+" || "+DateTime.Now+" || "+e.Message);
                    chrome.Refresh();
                    //chrome.UpdateCurentLink();
                    counter++;
                }
            }

            if (productData.Count != totalProducts)
            {
                throw new Exception("Не удалось взять все продукты из категории по ссылке: " + link + " Взято " + productData.Count + " из " + totalProducts + " товаров");
            }

             return productData;
        }

        //метод получения всех товаров с одной страницы
        public List<Product> GetPricesFromOnePage()
        {
            int i = 1;
            List <Product> products = new List<Product>();

            string nameTemplate = "(//div[@data-id='product'])[@NAME@]/a/span";
            string hrefTemplate = "(//div[@data-id='product'])[@NAME@]/a";
            string priceTemplate = "((//div[@data-id='product'])[@NAME@]//div[contains(@class,'product-buy__price') and text()])[last()]";
            string currentElement = nameTemplate.Replace("@NAME@", i.ToString());
            string currentHref = hrefTemplate.Replace("@NAME@", i.ToString());
            string totalProductsXpath = "(//div[@data-id='product'])";

            chrome.WaitElement(totalProductsXpath, shortTimeout);
            int totalPages = chrome.CountElements(totalProductsXpath);


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (i <= totalPages)
            {

                string name = chrome.GetText(currentElement, shortTimeout * 2);
                string currentPrice = priceTemplate.Replace("@NAME@", i.ToString());

                string price = chrome.GetText(currentPrice, shortTimeout * 2);
                if (price.Length == 0)
                {
                    chrome.Sleep(1);
                    price = chrome.GetText(currentPrice, shortTimeout * 2);
                }
                string href= chrome.GetAttribute(currentHref,"href", shortTimeout * 2);

                Product curProduct = new Product(name, href, price);

                if (!products.Contains(curProduct)) products.Add(curProduct);

                i++;
                currentElement = nameTemplate.Replace("@NAME@", i.ToString());
                currentHref = hrefTemplate.Replace("@NAME@", i.ToString());
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            Console.WriteLine("Время парсинга страницы = " + elapsedTime);

            return products;
        }
  

        //метод установки текущего города, нажимает на Выбрать другой    
        public void SetCity(string cityId)
        {
            string js = "setCity('" + cityId + "')";
            chrome.executeJS(js);
            chrome.Sleep(1);
            chrome.Refresh();
        }


    }
}

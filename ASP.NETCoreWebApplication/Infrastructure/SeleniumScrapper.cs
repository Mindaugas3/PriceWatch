using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace ASP.NETCoreWebApplication.Interactors
{
    internal static class SeleniumScrapper
    {
        private static RemoteWebDriver _webDriver;
        private static readonly List<string> CssSelectors = new List<string>
        {
            "div > table > tbody > tr",
            "tr[class]",
            "tr[class='odd']",
            "*[class='odd']",
            "tr[class*='e']"
        };
        
        public static WebDriver CreateFirefoxDriver()
        {
            var options = new FirefoxOptions();
            options.AddArgument("--headless");
            return new FirefoxDriver(options);
        }

        public static WebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            return new ChromeDriver(options);
        }
    }
}

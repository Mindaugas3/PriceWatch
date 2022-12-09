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

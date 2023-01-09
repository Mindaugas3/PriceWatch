using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace ASP.NETCoreWebApplication.Infrastructure
{
    public class SeleniumScrapper
    {
        private readonly WebDriver _webDriver;

        internal SeleniumScrapper()
        {
            this._webDriver = CreateFirefoxDriver();
        }
        private static WebDriver CreateFirefoxDriver()
        {
            var options = new FirefoxOptions();
            options.AddArgument("--headless");
            return new FirefoxDriver(options);
        }

        public WebDriver GetWebDriver()
        {
            return this._webDriver;
        }

        ~SeleniumScrapper()
        {
            this._webDriver.Close();
        }
    }
}

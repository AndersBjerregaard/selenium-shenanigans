using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace selenium_shenanigans_classlib;

public class Driver
{
   public void Foo()
   {
      var options = new FirefoxOptions();

      var uri = new System.Uri("http://selenium-hub:4444");

      var driver = new RemoteWebDriver(uri, options);

      driver.Url = "https://www.google.com";
      driver.FindElement(By.Name("q")).SendKeys("webdriver" + Keys.Return);

      driver.Quit();
   }
}
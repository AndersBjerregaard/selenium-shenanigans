using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

Console.WriteLine("Starting WebDriver...");

var options = new FirefoxOptions();

var uri = new System.Uri("http://localhost:4444");

var driver = new RemoteWebDriver(uri, options);

Console.WriteLine("Running Selenium script...");

driver.Url = "https://www.google.com";
driver.FindElement(By.Name("q")).SendKeys("webdriver" + Keys.Return);
Console.WriteLine(driver.Title);

Console.WriteLine("COMPLETE! Closing...");

driver.Quit();
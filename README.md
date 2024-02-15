# selenium-shenanigans

## Selenium WebDriver NuGet

### Package

[NuGet source](https://www.nuget.org/packages/Selenium.WebDriver)

```shell
dotnet add package Selenium.WebDriver --version 4.17.0
```

### Basic Usage

```csharp
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

var driver = new ChromeDriver();

driver.Url = "https://www.google.com";
driver.FindElement(By.Name("q")).SendKeys("webdriver" + Keys.Return);
Console.WriteLine(driver.Title);

driver.Quit();
```
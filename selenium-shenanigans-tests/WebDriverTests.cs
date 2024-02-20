using System.Collections;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace selenium_shenanigans_tests;

public class WebDriverTests : IClassFixture<WebDriverFixture>
{
    private readonly WebDriverFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public WebDriverTests(WebDriverFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [ClassData(typeof(InitialTestClassData))]
    public void InitialTest(DriverOptions driverOptions)
    {
        RemoteWebDriver? driver = null;
        try
        {
            var uri = _fixture.WebDriverUri;

            driver = new RemoteWebDriver(uri, driverOptions);

            driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");

            var title = driver.Title;
            Assert.Equal("Web form", title);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            var textBox = wait.Until(webDriver =>
            {
                var e = webDriver.FindElement(By.Name("my-text"));
                if (e.Displayed)
                    return e;
                return null;
            });
            
            var submitButton = driver.FindElement(By.TagName("button"));
            
            textBox?.SendKeys("Selenium");
            submitButton.Click();
            
            var message = driver.FindElement(By.Id("message"));
            var value = message.Text;
            Assert.Equal("Received!", value);
        }
        catch (Exception e)
        {
            _testOutputHelper.WriteLine(e.ToString());
            throw;
        }
        finally
        {
            if (driver is not null)
                driver.Quit();
        }
    }

    [Fact]
    public async Task ParallelTests()
    {
        var task1 = Task.Run(() => Foo(new FirefoxOptions()));
        var task2 = Task.Run(() => Foo(new ChromeOptions()));
        var task3 = Task.Run(() => Foo(new EdgeOptions()));

        await Task.WhenAll(task1, task2, task3);
        
        _testOutputHelper.WriteLine("All WebDrivers complete.");
    }

    private void Foo(DriverOptions driverOptions)
    {
        
        RemoteWebDriver? driver = null;
        try
        {
            var uri = _fixture.WebDriverUri;

            driver = new RemoteWebDriver(uri, driverOptions);

            driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/web-form.html");

            var title = driver.Title;
            Assert.Equal("Web form", title);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            var textBox = wait.Until(webDriver =>
            {
                var e = webDriver.FindElement(By.Name("my-text"));
                return e.Displayed ? e : null;
            });
            
            var submitButton = driver.FindElement(By.TagName("button"));
            
            textBox?.SendKeys("Selenium");
            submitButton.Click();
            
            var message = driver.FindElement(By.Id("message"));
            var value = message.Text;
            Assert.Equal("Received!", value);
            
            _testOutputHelper.WriteLine(driverOptions.BrowserName + " WebDriver finished without errors.");
        }
        catch (Exception e)
        {
            _testOutputHelper.WriteLine(e.ToString());
            throw;
        }
        finally
        {
            driver?.Quit();
        }
    }
}

public class InitialTestClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [new FirefoxOptions()];
        yield return [new EdgeOptions()];
        yield return [new ChromeOptions()];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class WebDriverFixture : IDisposable
{
    public WebDriverFixture()
    {
        string? env = Environment.GetEnvironmentVariable("GRID_URI");

        if (string.IsNullOrWhiteSpace(env))
        {
            Debug.WriteLine("Environment variable 'GRID_URI' was unset." +
                              " Defaulting Selenium Gird Hub uri to localhost...");
            WebDriverUri = new Uri(@"http://localhost:4444");
        }
        else
        {
            Debug.WriteLine($"Environment variable 'GRID_URI' loaded as: {env}");
            WebDriverUri = new Uri($"http://{env}:4444");
        }
    }

    public void Dispose() => WebDriverUri = null;

    public Uri? WebDriverUri { get; private set; }
}
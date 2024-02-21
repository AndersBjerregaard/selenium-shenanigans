using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace se_fd_tests;

public class WebDriverTests : IClassFixture<WebDriverFixture>
{
    private readonly WebDriverFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public WebDriverTests(WebDriverFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task ParallelTests()
    {
        DriverOptions[] driverOptions = GetWebDriverOptions();
        Task[] tests = new Task[driverOptions.Length];
        
        for (var i = 0; i < driverOptions.Length; i++)
        {
            var driverOption = driverOptions[i];
            Task task = Task.Run(() => GenericTest(driverOption));
            tests[i] = task;
        }

        await Task.WhenAll(tests);
        
        _testOutputHelper.WriteLine("All WebDrivers complete.");
    }

    [Fact]
    public async Task ConfigTest()
    {
        var options = new FirefoxOptions();
        options.AddAdditionalOption("se:name", "Simple Test Name");
        options.AddAdditionalOption("se:sampleMetadata", "Sample metadata value");
        options.AddAdditionalOption("se:recordVideo", "true");
        // options.AddAdditionalOption("timeouts", "{ \"implicit\": 3000, \"pageLoad\": 3000, \"script\": 3000 }");
        // options.AddAdditionalOption("moz:shutdownTimeout", "3000");

        RemoteWebDriver? driver = null;
        try
        {
            driver = new RemoteWebDriver(_fixture.WebDriverUri, options);
        
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

    private DriverOptions[] GetWebDriverOptions()
    {
        return [new FirefoxOptions(), new ChromeOptions(), new EdgeOptions()];
    }

    private void GenericTest(DriverOptions driverOptions)
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
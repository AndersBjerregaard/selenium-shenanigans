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

## Selenium Scripts .Net Test Application

Through some testing. I believe that the Selenium Grid Hub is not ready for
session connections, when starting the .net app from the docker compose.
A manual workaround I got working was waiting for startup, and executing the
following command from this project's root directory:
```shell
docker run --rm -ti --network selenium-grid -e GRID_URI=selenium-hub hub-node-execution-test-scripts
```
I believe that running a healthcheck periodically on the hub, will result in a
proper execution pattern ([source](https://github.com/SeleniumHQ/docker-selenium?tab=readme-ov-file#adding-a-healthcheck-to-the-grid)):
```shell
#!/bin/bash
# wait-for-grid.sh

set -e
url="http://localhost:4444/wd/hub/status"
wait_interval_in_seconds=1
max_wait_time_in_seconds=30
end_time=$((SECONDS + max_wait_time_in_seconds))
time_left=$max_wait_time_in_seconds

while [ $SECONDS -lt $end_time ]; do
    response=$(curl -sL "$url" | jq -r '.value.ready')
    if [ -n "$response"  ]  && [ "$response" ]; then
        echo "Selenium Grid is up - executing tests"
        break
    else
        echo "Waiting for the Grid. Sleeping for $wait_interval_in_seconds second(s). $time_left seconds left until timeout."
        sleep $wait_interval_in_seconds
        time_left=$((time_left - wait_interval_in_seconds))
    fi
done

if [ $SECONDS -ge $end_time ]; then
    echo "Timeout: The Grid was not started within $max_wait_time_in_seconds seconds."
    exit 1
fi
```
*Will require `jq` installed via `apt-get`, else the script will keep printing `Waiting` without completing the execution.*

Article describing using health check shell scripts in docker:
https://docs.docker.com/compose/startup-order/

Note: If needed, replace `localhost` and `4444` for the correct values in your environment. Also, this script is polling indefinitely, you might want to tweak it and establish a timeout.

Let's say that the normal command to execute your tests is `mvn clean test`. Here is a way to use the above script and execute your tests:
```shell
$ ./wait-for-grid.sh mvn clean test
```

Like this, the script will poll until the Grid is ready, and then your tests will start.
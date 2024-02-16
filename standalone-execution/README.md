# Network Configuration

By default, the Selenium Grid (hub) server is set to bind its port to 4444, specifically to the Docker Daemon IP.
Which means that it's impossible to have other containers attempt to communicate with the Selenium Grid server
without going through the host system's network.
To bypass this; pass the environment variable `host` with the value of `0.0.0.0` to the grid server.
Or configure it through a `config.toml` file under a `[server]` section. Example:
```toml
[server]
host = "0.0.0.0"
```
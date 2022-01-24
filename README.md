# Mail2Gotify

Simple stand-alone service that hosts an STMP server, receives mail and redirects them to a Gotify instance.


## Description

This application will let you redirect mail to your gotify instance. It can help aggregate notifications from services or systems that don't support Gotify.


## Usage


Simply fill up the necessary configuration variables, host the service and expose the configured port. Create a Gotify Application if necessary and point your email notifier to the host and port, using the Gotify App name as the authentication user and the Gotify App token as the authentication password.

## <a id="configuration-1">Configuration</a>

Configuring the service is done either the appsettings.json found at "/Mail2Gotify", or with associated environment variables (great for docker). Here's a list and small description of available configuration variables:

Variable | Default | Description|
---|---|---
Logging:LogLevel:Default | Debug | Default .NET logging level.
Logging:LogLevel:Microsoft | Warning | Default .NET Microsoft logging level.
Logging:LogLevel:Microsoft.Hosting.Lifetime | Information |  Default .NET Microsoft Hosting logging level.
Services:Mail2Gotify:HostAddress | | The address on which this service resides.
Services:Mail2Gotify:HostPort | 587 | The port that this service will use to receive mail through SMTP.
Services:Mail2Gotify:CacheDirectory | | The directory to use for persistent caaching (bind with docker for permanency).
Services:Gotify:ServiceUri | | Gotify's service uri.
Services:Gotify:Header | X-Gotify-Key | Gotify's authentication header to use (the default is typically correct).
Services:Certificate:Name | Mail2Gotify | The name of the X502 certificate used for the SMTP server's TLS encryption (TLS 1.1 or 1.2 is mandatory).
Services:Certificate:Password |  | The password used for the X502 certificate.


# Docker

Here's how I configure my installation :

## docker-compose.yaml
```yaml
version: "3"

volumes:
  cache:

services:
  mail2gotify:
    container_name: mail2gotify
    entrypoint:
      - dotnet
      - Mail2Gotify.dll
    environment:
      - Services:Mail2Gotify:HostAddress=localhost
      - Services:Mail2Gotify:CacheDirectory=/app/cache
      - Services:Gotify:ServiceUri=https://your-gotify-instance
      - Certificate:Password=randompassword
      - TZ=America/Toronto
    image: mattmckenzy/mail2gotify:latest      
    volumes:
      - cache:/app/cache
      - /etc/localtime:/etc/localtime:ro
      - /etc/timezone:/etc/timezone:ro
    restart: always
    ports:
      - "587:587"
    network_mode: "host"
```

# Donate

If you appreciate my work and feel like helping me realize other projects, you can donate at <a href="https://paypal.me/MattMckenzy">https://paypal.me/MattMckenzy</a>!

# Mail2Gotify

Simple stand-alone service that hosts an STMP server, receives mail and redirects them to a Gotify instance.


## Description

This application will let you redirect mail to your gotify instance. It can help aggregate notifications from services or systems that don't support Gotify.


## Usage


Simply fill up the necessary configuration variables, host the service and expose the configured port. 

TLS 1.2 or 1.3 is mandatory and Mail2Gotify will need a certificate to function. You can choose to point the "CertLocation" variable to a valid PEM or PKCS7 certificate, or leave it empty to have Mail2Gotify cerate a self-signed one. Keep in mind, however, that self-signed certificates might not be accepted by certain applications, such as pfSense's notifications service. For PEM certificate types, it can also support encrypted or seperate key file certificates if the configuration is properly filled out. 

Create a Gotify Application if necessary and point your email notifier to the host and port, using the Gotify App name as the authentication user and the Gotify App token as the authentication password. You can also append a dash and priority number to the application name, i.e. "pfSense-6", to change the priority level of the Gotify notification. Default priority is 5.

## <a id="configuration-1">Configuration</a>

Configuring the service is done either the appsettings.json found at "/Mail2Gotify", or with associated environment variables (great for docker). Here's a list and small description of available configuration variables:

Variable | Default | Description|
---|---|---
Logging:LogLevel:Default | Debug | Default .NET logging level.
Logging:LogLevel:Microsoft | Warning | Default .NET Microsoft logging level.
Logging:LogLevel:Microsoft.Hosting.Lifetime | Information |  Default .NET Microsoft Hosting logging level.
Services:Gotify:ServiceUri | | Gotify's service uri.
Services:Gotify:Header | X-Gotify-Key | Gotify's authentication header to use (the default is typically correct).
Services:Mail2Gotify:HostAddress | | The address on which this service resides.
Services:Mail2Gotify:HostPort | 587 | The port that this service will use to receive mail through SMTP.
Services:Mail2Gotify:CacheDirectory | | The directory to use for persistent caaching (bind with docker for permanency).
Services:Mail2Gotify:CertLocation | | The location for the certificate used for TLS encryption. If left empty, Mail2Gotify will create a self-signed certificate using the "Services:Certificate:Name" and "Services:Certificate:Password" variables shown below.
Services:Mail2Gotify:KeyLocation | | The location of the PEM key file certificate used for TLS encryption. Only used when CertType is "PEM".
Services:Mail2Gotify:CertPassword | | The password of the PEM file if it is encrypted. Only used when CertType is "PEM".
Services:Mail2Gotify:CertType | PEM | The type pf certificate used for TLS encryption. Either "PEM" or "PKCS7".
Services:SelfSignedCertificate:Name | Mail2Gotify | The name used for the creation of the self-signed certificate used for the SMTP server's TLS encryption.
Services:SelfSignedCertificate:Password |  | The password used for the creation of the self-signed certificate.


## Docker

Here's how I configure my installation :

### docker-compose.yaml
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
      - Services:Mail2Gotify:CertLocation=/app/certs/privkey.pfx
      - Services:Gotify:ServiceUri=https://gotify.exampledomain.ca
      - TZ=America/Toronto
    image: mattmckenzy/mail2gotify:latest
    volumes:
      - cache:/app/cache
      - /etc/localtime:/etc/localtime:ro
      - /etc/timezone:/etc/timezone:ro
      - /mnt/docker/storage/volumes/swag_data/_data/etc/letsencrypt/live/exampledomain.ca:/app/certs
    restart: always
    ports:
      - "587:587"
    network_mode: "host"
```

## Release Notes

## 1.1.0

- Added configuration options to support new certificate types.

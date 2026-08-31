[![](https://img.shields.io/nuget/v/soenneker.cloudflare.validators.request.functions.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.validators.request.functions/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.validators.request.functions/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.validators.request.functions/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.cloudflare.validators.request.functions.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.cloudflare.validators.request.functions/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cloudflare.validators.request.functions/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cloudflare.validators.request.functions/actions/workflows/codeql.yml)

# Soenneker.Cloudflare.Validators.Request.Functions

Validates the client certificate Azure App Service forwards to an isolated-worker Azure Function against packaged Cloudflare origin-pull certificate fingerprints.

## Installation

```bash
dotnet add package Soenneker.Cloudflare.Validators.Request.Functions
```

## Registration

```csharp
using Soenneker.Cloudflare.Validators.Request.Functions.Registrars;

services.AddCloudflareRequestValidatorAsSingleton();
```

The package supplies `cloudflareorigincerts.txt` as a build resource. Set `Cloudflare:RequestValidatorLog` to `true` only when certificate-match debug logging is useful.

## Usage

```csharp
using Soenneker.Cloudflare.Validators.Request.Functions.Abstract;

bool fromCloudflare = await validator.IsFromCloudflare(
    request,
    functionContext.CancellationToken);

if (!fromCloudflare)
    return request.CreateResponse(HttpStatusCode.Forbidden);
```

`IsFromCloudflare` reads the first `X-ARR-ClientCert` value, decodes the certificate, calculates SHA-256 over its DER bytes, and compares that fingerprint case-insensitively with the packaged set. Missing, malformed Base64, and invalid certificate data return `false`.

## Trust boundary

`X-ARR-ClientCert` is meaningful only when Azure App Service is the trusted TLS terminator and supplies the header from an authenticated client certificate. Ensure Authenticated Origin Pulls is enabled, App Service requires client certificates, and the Function cannot be reached through an ingress path that lets callers forge the header or bypass certificate enforcement.

This validator performs exact fingerprint matching; it does not build a certificate chain. Cloudflare certificate rotation therefore requires an updated fingerprint package. `Validate(string)` exposes the same comparison for callers that already have a hexadecimal SHA-256 fingerprint.

For request enforcement, `Soenneker.Cloudflare.Middlewares.Require.Functions` provides middleware built on this validator.

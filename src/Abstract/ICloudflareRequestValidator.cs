using Microsoft.Azure.Functions.Worker.Http;
using Soenneker.Validators.Validator.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.Validators.Request.Functions.Abstract;

/// <summary>
/// A validator for Azure Functions that verifies incoming requests originate from Cloudflare by checking the client certificate thumbprint.
/// </summary>
public interface ICloudflareRequestValidator : IValidator, IDisposable, IAsyncDisposable
{
    ValueTask<bool> IsFromCloudflare(HttpRequestData req, CancellationToken cancellationToken = default);
}

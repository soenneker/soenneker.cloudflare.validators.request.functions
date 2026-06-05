using Microsoft.Azure.Functions.Worker.Http;
using Soenneker.Validators.Validator.Abstract;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.Validators.Request.Functions.Abstract;

/// <summary>
/// A validator for Azure Functions that verifies incoming requests originate from Cloudflare by checking the client certificate thumbprint.
/// </summary>
public interface ICloudflareRequestValidator : IValidator, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Executes the is from cloudflare operation.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<bool> IsFromCloudflare(HttpRequestData req, CancellationToken cancellationToken = default);
}
using Microsoft.Azure.Functions.Worker.Http;
using Soenneker.Validators.Validator.Abstract;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cloudflare.Validators.Request.Functions.Abstract;

/// <summary>
/// Validates Azure Functions requests using the client certificate forwarded by Azure App Service.
/// </summary>
public interface ICloudflareRequestValidator : IValidator, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Compares the SHA-256 fingerprint of the certificate in <c>X-ARR-ClientCert</c> with the packaged Cloudflare fingerprints.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<bool> IsFromCloudflare(HttpRequestData req, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares a SHA-256 certificate fingerprint with the packaged Cloudflare certificate fingerprints.
    /// </summary>
    [Pure]
    ValueTask<bool> Validate(string thumbprint, CancellationToken cancellationToken = default);
}

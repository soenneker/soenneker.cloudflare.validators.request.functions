using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cloudflare.Validators.Request.Functions.Abstract;
using Soenneker.Validators.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.File.Abstract;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Spans.Readonly.Bytes;
using Soenneker.Utils.Paths.Resources;

namespace Soenneker.Cloudflare.Validators.Request.Functions;

/// <inheritdoc cref="ICloudflareRequestValidator"/>
public sealed class CloudflareRequestValidator : Validator, ICloudflareRequestValidator
{
    private readonly AsyncSingleton<HashSet<string>> _thumbprintsSet;
    private readonly IFileUtil _fileUtil;

    private readonly bool _log;

    public CloudflareRequestValidator(ILogger<CloudflareRequestValidator> logger, IFileUtil fileUtil, IConfiguration configuration) : base(logger)
    {
        _fileUtil = fileUtil;
        _log = configuration.GetValue<bool>("Cloudflare:RequestValidatorLog");

        _thumbprintsSet = new AsyncSingleton<HashSet<string>>(CreateThumbprintsSet);
    }

    private async ValueTask<HashSet<string>> CreateThumbprintsSet(CancellationToken token)
    {
        string path = await ResourcesPathUtil.GetResourceFilePath("cloudflareorigincerts.txt").NoSync();

        return await _fileUtil.ReadToHashSet(path, StringComparer.OrdinalIgnoreCase, cancellationToken: token).NoSync();
    }

    public async ValueTask<bool> IsFromCloudflare(HttpRequestData req, CancellationToken cancellationToken = default)
    {
        // Azure App Service forwards the client certificate as base64 in X-ARR-ClientCert
        if (!req.Headers.TryGetValues("X-ARR-ClientCert", out IEnumerable<string>? values))
        {
            return false;
        }

        string? b64 = values.FirstOrDefault();

        if (b64.IsNullOrWhiteSpace())
            return false;

        byte[] raw;

        try
        {
            raw = b64.ToBytesFromBase64();
        }
        catch
        {
            return false;
        }

        using X509Certificate2 cert = X509CertificateLoader.LoadCertificate(raw);
        ReadOnlySpan<byte> data = cert.RawData;

        return await Validate(data.ToSha256Hex(), cancellationToken).NoSync();
    }

    public async ValueTask<bool> Validate(string thumbprint, CancellationToken cancellationToken = default)
    {
        if (thumbprint.IsNullOrWhiteSpace())
        {
            if (_log)
                Logger.LogDebug("Thumbprint was null or whitespace");

            return false;
        }

        if ((await _thumbprintsSet.Get(cancellationToken).NoSync()).Contains(thumbprint))
        {
            if (_log)
                Logger.LogDebug("Incoming certificate thumbprint ({incoming}) is not a current Cloudflare certificate thumbprint", thumbprint);

            return true;
        }

        return false;
    }

    public ValueTask DisposeAsync()
    {
        return _thumbprintsSet.DisposeAsync();
    }

    public void Dispose()
    {
        _thumbprintsSet.Dispose();
    }
}
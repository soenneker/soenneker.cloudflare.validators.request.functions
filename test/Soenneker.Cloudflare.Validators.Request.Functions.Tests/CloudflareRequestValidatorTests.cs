using Soenneker.Cloudflare.Validators.Request.Functions.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cloudflare.Validators.Request.Functions.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloudflareRequestValidatorTests : HostedUnitTest
{
    private readonly ICloudflareRequestValidator _validator;

    public CloudflareRequestValidatorTests(Host host) : base(host)
    {
        _validator = Resolve<ICloudflareRequestValidator>(true);
    }

    [Test]
    public void Default()
    {

    }
}

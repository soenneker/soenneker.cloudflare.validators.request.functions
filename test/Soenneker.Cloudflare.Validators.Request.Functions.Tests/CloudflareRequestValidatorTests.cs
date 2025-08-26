using Soenneker.Cloudflare.Validators.Request.Functions.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cloudflare.Validators.Request.Functions.Tests;

[Collection("Collection")]
public sealed class CloudflareRequestValidatorTests : FixturedUnitTest
{
    private readonly ICloudflareRequestValidator _validator;

    public CloudflareRequestValidatorTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _validator = Resolve<ICloudflareRequestValidator>(true);
    }

    [Fact]
    public void Default()
    {

    }
}

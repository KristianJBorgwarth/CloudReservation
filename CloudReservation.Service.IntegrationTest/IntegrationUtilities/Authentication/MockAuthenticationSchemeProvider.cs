using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CloudReservation.IntegrationTest.IntegrationUtilities.Authentication;

internal class MockAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    public MockAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {
    }

    public override Task<AuthenticationScheme> GetSchemeAsync(string name)
    {
        var scheme = new AuthenticationScheme("Test", "Test", typeof(MockAuthenticationHandler));

        return Task.FromResult(scheme);
    }
}
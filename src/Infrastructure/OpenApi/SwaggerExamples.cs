using CleanTib.Application.Identity.Tokens;
using CleanTib.Shared.Multitenancy;
using NSwag.Examples;

namespace CleanTib.Infrastructure.OpenApi;

public class TokenExample : IExampleProvider<TokenRequest>
{
    public TokenRequest GetExample()
    {
        return new TokenRequest(MultitenancyConstants.Root.EmailAddress, MultitenancyConstants.DefaultPassword);
    }
}
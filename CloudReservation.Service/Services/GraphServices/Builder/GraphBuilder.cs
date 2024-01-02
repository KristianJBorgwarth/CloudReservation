using Azure.Core;
using Azure.Identity;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using Microsoft.Graph;

namespace CloudReservation.Service.Services.GraphServices.Builder;

public class GraphBuilder : IGraphBuilder
{
    private readonly ClientSecretCredential _clientSecretCredential;
    private readonly string[] _scopes;
    
    public GraphBuilder(IConfigurationWrapper configurationWrapper)
    {
        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        _clientSecretCredential = new ClientSecretCredential(
            configurationWrapper.GetMsGraphConfiguration<string>(ConfigurationType.TenantId),
            configurationWrapper.GetMsGraphConfiguration<string>(ConfigurationType.ClientId),
            configurationWrapper.GetMsGraphConfiguration<string>(ConfigurationType.ClientSecret),
            options);
        _scopes = new[] { "https://graph.microsoft.com/.default" };
    }
    public GraphServiceClient Build()
    {
        var accessToken = _clientSecretCredential.GetToken(new TokenRequestContext(_scopes)).Token;
        
        return new GraphServiceClient(_clientSecretCredential, _scopes);
    }
}
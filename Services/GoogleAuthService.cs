using DeathByAIBackend.Interfaces;
using DeathByAIBackend.Models;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;

namespace DeathByAIBackend.Services;

public class GoogleAuthService(IConfiguration configuration) : IOAuthProvider
{
    private readonly string _clientId = configuration["Google:ClientId"]
                                        ?? throw new ArgumentNullException($"Google:ClientId", "Google Client ID is not configured.");
    private readonly string _clientSecret = configuration["Google:ClientSecret"]
                                            ?? throw new ArgumentNullException($"Google:ClientSecret", "Google Client Secret is not configured.");
    private readonly string _redirectUri = configuration["Google:RedirectUri"]
                                           ?? throw new ArgumentNullException($"Google:RedirectUri", "Google Redirect URI is not configured.");

    public Task<Uri> GetAuthUrl()
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            },
            Scopes = ["email", "profile"]
        });

        return Task.FromResult(flow.CreateAuthorizationCodeRequest(_redirectUri).Build());
    }

    public async Task<AuthenticatedUser> GetAuthenticatedUser(string code)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            }
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync("me", code, _redirectUri, CancellationToken.None);

        var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

        var authenticatedUser = new AuthenticatedUser(payload.Name, payload.Email, payload.Picture);

        return authenticatedUser;
    }
}
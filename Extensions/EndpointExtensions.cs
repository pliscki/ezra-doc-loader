using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Ezra.Scribe.Core.CloudStorage;
using Ezra.Scribe.Core.CloudStorage.GoogleDrive;
using Google.Apis.Auth.OAuth2.Flows;

namespace Ezra.Scribe.Extensions;

public static class EndpointExtensions
{
    public static void MapGoogleDriveEndpoints(this WebApplication app)
    {
        app.MapPost("/drive/list-files", async (string userId, ICloudStorageService storageService, CancellationToken cancellationToken) =>
        {
            var files = await storageService.ListFilesAsync(string.Empty, userId, cancellationToken);
            return Results.Ok(files);
        })
        .WithName("ListGoogleDriveFiles")
        .WithOpenApi();
    }

    public static void MapTestEndpoints(this WebApplication app)
    {
        app.MapGet("/signin-google-initiate", (
            string userId,
            IOptions<GoogleDriveSettings> googleDriveSettingsOptions,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.BadRequest("Missing userId in query string.");

            var settings = googleDriveSettingsOptions.Value;
            var clientId = settings.ClientId;
            var redirectUri = settings.RedirectUrl;
            var scopes = new[]
            {
                "https://www.googleapis.com/auth/drive.readonly",
                "https://www.googleapis.com/auth/userinfo.email"
            };
            var scope = string.Join(" ", scopes);
            var oauthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={Uri.EscapeDataString(clientId)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&access_type=offline&prompt=consent&state={Uri.EscapeDataString(userId)}";
            return Results.Ok(new { redirectUrl = oauthUrl });
        })
        .WithName("InitiateGoogleSignin")
        .WithOpenApi();

        app.MapGet("/signin-google", async (string code, string state, IOptions<GoogleDriveSettings> googleDriveSettingsOptions, IUserTokenStore tokenStore, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(code))
                return Results.BadRequest("Missing code in query string.");

            var settings = googleDriveSettingsOptions.Value;
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                {
                    ClientId = settings.ClientId,
                    ClientSecret = settings.ClientSecret
                }
            };
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            var token = await flow.ExchangeCodeForTokenAsync(
                userId: string.Empty,
                code: code,
                redirectUri: settings.RedirectUrl,
                taskCancellationToken: cancellationToken
            ).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token.RefreshToken))
                return Results.BadRequest("Failed to retrieve refresh token.");

            await tokenStore.StoreRefreshTokenAsync("localdev", token.RefreshToken, cancellationToken);
            return Results.Ok("Refresh token stored successfully.");
        })
        .WithName("GoogleSigninCallback")
        .WithOpenApi();
    }
}

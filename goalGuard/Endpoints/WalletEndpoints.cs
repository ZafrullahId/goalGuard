using goalGuard.Contracts;

namespace goalGuard.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wallet").WithTags("Wallet");

        group.MapPost("/{userId}/sign-challenge", (string userId, SignChallengeRequest request) =>
        {
            return Results.StatusCode(501);
        })
        .WithSummary("Sign Challenge")
        .WithDescription("Owner-proof challenge + signature.");

        group.MapGet("/{userId}/summary", (string userId) =>
        {
            return Results.StatusCode(501);
        })
        .WithSummary("Wallet Summary")
        .WithDescription("Balances + recent transactions.");
    }
}

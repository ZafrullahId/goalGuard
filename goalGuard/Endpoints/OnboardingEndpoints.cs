using goalGuard.Contracts;
using goalGuard.Services;

namespace goalGuard.Endpoints;

public static class OnboardingEndpoints
{
    public static void MapOnboardingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/onboarding").WithTags("Onboarding");

        group.MapPost("/users", async (CreateUserRequest request, IOnboardingService onboardingService, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return Results.BadRequest(new { Error = "FirstName, Email, and PhoneNumber are required." });
            }

            var result = await onboardingService.CreateUserAsync(request, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { Error = result.ErrorMessage });
            }

            return Results.Created($"/api/onboarding/{result.UserId}/status", new CreateUserResponse(result.UserId.GetValueOrDefault(), result.BmoniUserId, "UserCreated"));
        })
        .WithSummary("Create BMONI User")
        .WithDescription("Creates a new user on the BMONI platform.");

        group.MapGet("/{userId}/status", (string userId) =>
        {
            return Results.StatusCode(501);
        })
        .WithSummary("Check Onboarding Status")
        .WithDescription("Check the status of user onboarding.");

        group.MapPost("/{userId}/wallet-challenge", async (Guid userId, IOnboardingService onboardingService, CancellationToken ct) =>
        {
            var result = await onboardingService.RequestOwnerProofChallengeAsync(userId, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { Error = result.ErrorMessage });
            }

            return Results.Ok(new 
            { 
                ChallengeId = result.ChallengeId, 
                Message = result.Message, 
                ExpiresAt = result.ExpiresAt 
            });
        })
        .WithSummary("Request Owner Proof Challenge")
        .WithDescription("Requests a challenge to prove ownership of the smart wallet.");

        group.MapPost("/{userId}/start-nigeria", (string userId) =>
        {
            return Results.StatusCode(501);
        })
        .WithSummary("Activate NGN Rail")
        .WithDescription("Activates the Nigeria rail for the user.");

        group.MapPost("/{userId:guid}/start-nigeria", async (Guid userId, IOnboardingService onboardingService, CancellationToken ct) =>
        {
            var result = await onboardingService.StartNigeriaOnboardingAsync(userId, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { Error = result.ErrorMessage });
            }

            return Results.Ok(new { Message = result.Message });
        })
        .WithSummary("Activate Nigeria Rail")
        .WithDescription("Submits sandbox KYC (BVN) and activates the NGN rail for a user's wallet.");

        group.MapGet("/{userId:guid}/status", async (Guid userId, IOnboardingService onboardingService, CancellationToken ct) =>
        {
            var result = await onboardingService.GetOnboardingStatusAsync(userId, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { Error = result.ErrorMessage });
            }

            return Results.Ok(result.Status);
        })
        .WithSummary("Get Onboarding Status")
        .WithDescription("Checks BMONI's onboarding status for a user, including KYC and rail activation.");

        group.MapPost("/{userId:guid}/wallet", async (Guid userId, IOnboardingService onboardingService, CancellationToken ct) =>
        {
            var result = await onboardingService.CreateManagedWalletAsync(userId, ct);

            if (!result.Success)
            {
                return Results.BadRequest(new { Error = result.ErrorMessage });
            }

            return Results.Ok(new { SmartWalletAddress = result.SmartWalletAddress });
        })
        .WithSummary("Create Managed Smart Wallet")
        .WithDescription("Signs the owner-proof challenge and creates the user's managed smart wallet.");
    }
}

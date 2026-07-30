using goalGuard.Contracts.Bmoni;
using Refit;

namespace goalGuard.Http;

public interface IBmoniApi
{
    [Post("/v1/users")]
    Task<ApiResponse<CreateUserResponse>> CreateUserAsync([Body] CreateUserRequest request);

    [Post("/v1/users/{userId}/smart-wallets/owner-proof-challenges")]
    Task<ApiResponse<OwnerProofChallengeResponse>> RequestOwnerProofChallengeAsync(string userId, [Body] OwnerProofChallengeRequest request);

    [Get("/v1/users/{userId}/smart-wallets/account/wallets")]
    Task<WalletsResponse> GetWalletsAsync(string userId);

    [Get("/v1/users/{userId}/smart-wallets/account/balances")]
    Task<BalancesResponse> GetBalancesAsync(string userId);

    [Get("/v1/users/{userId}/smart-wallets/account/transactions")]
    Task<TransactionsResponse> GetTransactionsAsync(string userId);

    [Post("/v1/users/{userId}/onboarding/start-nigeria")]
    Task<ApiResponse<StartNigeriaResponse>> StartNigeriaOnboardingAsync(string userId, [Body] StartNigeriaRequest request);

    [Get("/v1/users/{userId}/onboarding/status")]
    Task<ApiResponse<OnboardingStatusResponse>> GetOnboardingStatusAsync(string userId);

    [Post("/v1/users/{userId}/smart-wallets/create-managed")]
    Task<ApiResponse<CreateManagedWalletResponse>> CreateManagedWalletAsync(string userId, [Body] CreateManagedWalletRequest request);
}

using System.Text.Json.Serialization;

namespace goalGuard.Contracts;

public record CreateUserRequest(
    string FirstName, 
    string LastName, 
    string? MiddleName, 
    string Email, 
    string PhoneNumber);
public record CreateUserResponse(Guid UserId, string? BmoniUserId, string Status);
public record OnboardingStatusResponse(string? AnchorStatus,
    string? BridgeStatus,
    string? MoneriumStatus,
    string? PaytrieStatus,
    string? EtherfuseStatus);
public record StartNigeriaResult(bool Success, string? ErrorMessage = null, string? Message = null);

public record OnboardingStatusResult(bool Success, string? ErrorMessage = null, OnboardingStatusResponse? Status = null);

public record SignChallengeRequest(string Challenge);
public record SignChallengeResponse(string Signature);
public record WalletSummaryResponse(string UserId, decimal Balance, IEnumerable<object> RecentTransactions);

public record CreateManagedWalletResult(bool Success, string? ErrorMessage = null, string? SmartWalletAddress = null);

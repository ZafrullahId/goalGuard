using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace goalGuard.Contracts.Bmoni;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email,
    string PhoneNumber,
    string? EmployeeId = null,
    string? IdentityId = null,
    string? Bvn = null,
    decimal? MonthlySalary = null,
    string? EmployerName = null,
    string? Occupation = null,
    string? AddressStreet = null,
    string? AddressCity = null,
    string? AddressState = null,
    string? AddressCountry = null,
    string? AddressPostalCode = null);
public record CreateUserResponse([property: JsonPropertyName("user")] BmoniUser User);

public record BmoniUser(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("bmoniUserId")] string BmoniUserId,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string? LastName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("phoneNumber")] string PhoneNumber,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
);

public record OnboardingStatusResponse(
    [property: JsonPropertyName("anchorStatus")] string? AnchorStatus,
    [property: JsonPropertyName("bridgeStatus")] string? BridgeStatus,
    [property: JsonPropertyName("moneriumStatus")] string? MoneriumStatus,
    [property: JsonPropertyName("paytrieStatus")] string? PaytrieStatus,
    [property: JsonPropertyName("etherfuseStatus")] string? EtherfuseStatus
);
public record StartNigeriaRequest(
    [property: JsonPropertyName("bvn")] string Bvn,
    [property: JsonPropertyName("ngnWalletAddress")] string NgnWalletAddress,
    [property: JsonPropertyName("ngnWalletIndex")] int NgnWalletIndex
);

public record StartNigeriaResponse(
    [property: JsonPropertyName("message")] string Message
);



public record OwnerProofChallengeRequest(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("userOwnerAddress")] string UserOwnerAddress);

public record OwnerProofChallengeResponse(
    [property: JsonPropertyName("challengeId")] string ChallengeId,
    [property: JsonPropertyName("groupId")] string GroupId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("expiresAt")] string ExpiresAt);

public record CreateManagedWalletRequest(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("userOwnerAddress")] string UserOwnerAddress,
    [property: JsonPropertyName("ownerProofChallengeId")] string OwnerProofChallengeId,
    [property: JsonPropertyName("ownerProofSignature")] string OwnerProofSignature
);

public record CreateManagedWalletResponse(
    [property: JsonPropertyName("id")] string SmartWalletId,
    [property: JsonPropertyName("walletAddress")] string SmartWalletAddress
);

public record SmartWalletResponse(string WalletId, string Address);

public record WalletsResponse(IEnumerable<SmartWalletResponse> Wallets);
public record BalancesResponse(decimal Available, decimal Pending);
public record TransactionsResponse(IEnumerable<object> Transactions);

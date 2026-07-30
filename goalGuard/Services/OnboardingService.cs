using goalGuard.Contracts;
using goalGuard.Data;
using goalGuard.Entity;
using goalGuard.Http;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Refit;
using System.Text;

namespace goalGuard.Services;

public record CreateUserResult(bool Success, string? ErrorMessage = null, Guid? UserId = null, string? BmoniUserId = null);
public record ChallengeResult(bool Success, string? ErrorMessage = null, string? ChallengeId = null, string? Message = null, string? ExpiresAt = null);

public interface IOnboardingService
{
    Task<CreateUserResult> CreateUserAsync(Contracts.CreateUserRequest request, CancellationToken ct = default);
    Task<CreateManagedWalletResult> CreateManagedWalletAsync(Guid localUserId, CancellationToken ct = default);
    Task<ChallengeResult> RequestOwnerProofChallengeAsync(Guid localUserId, CancellationToken ct = default);
    Task<OnboardingStatusResult> GetOnboardingStatusAsync(Guid localUserId, CancellationToken ct = default);
    Task<StartNigeriaResult> StartNigeriaOnboardingAsync(Guid localUserId, CancellationToken ct = default);
    string SignChallenge(string privateKeyHex, string challengeMessage);
}

public class OnboardingService : IOnboardingService
{
    private const string SandboxTestBvn = "22222222222";
    private readonly GoalGuardDbContext _dbContext;
    private readonly IBmoniApi _bmoniApi;
    private readonly ILogger<OnboardingService> _logger;

    public OnboardingService(GoalGuardDbContext dbContext, IBmoniApi bmoniApi, ILogger<OnboardingService> logger)
    {
        _dbContext = dbContext;
        _bmoniApi = bmoniApi;
        _logger = logger;
    }

    public async Task<CreateUserResult> CreateUserAsync(Contracts.CreateUserRequest request, CancellationToken ct = default)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email, ct))
        {
            return new CreateUserResult(Success: false, ErrorMessage: "A user with this email already exists.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            OnboardingStatus = OnboardingStatus.NotStarted
        };

        _dbContext.Users.Add(user);

        var bmoniRequest = new Contracts.Bmoni.CreateUserRequest(
            FirstName: request.FirstName,
            LastName: request.LastName,
            MiddleName: request.MiddleName,
            Email: request.Email,
            PhoneNumber: request.PhoneNumber
        );

        var bmoniResponse = await _bmoniApi.CreateUserAsync(bmoniRequest);
        if (bmoniResponse == null || !bmoniResponse.IsSuccessStatusCode || bmoniResponse.Content == null)
        {
            _logger.LogError("Failed to create user in Bmoni. Response: {Response}", bmoniResponse?.Error);
            return new CreateUserResult(Success: false, ErrorMessage: "Failed to create user in Bmoni.");
        }

        user.BmoniUserId = bmoniResponse.Content.User.BmoniUserId;
        user.OnboardingStatus = OnboardingStatus.UserCreated;
        await _dbContext.SaveChangesAsync(ct);

        return new CreateUserResult(Success: true, UserId: user.Id, BmoniUserId: user.BmoniUserId);
    }

    public async Task<ChallengeResult> RequestOwnerProofChallengeAsync(Guid localUserId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == localUserId, ct);

        if (user == null || user.OnboardingStatus < OnboardingStatus.UserCreated || string.IsNullOrWhiteSpace(user.BmoniUserId))
        {
            return new ChallengeResult(false, "User not found or not in correct status.");
        }

        if (string.IsNullOrWhiteSpace(user.OwnerAddress) || string.IsNullOrWhiteSpace(user.OwnerPrivateKey))
        {
            var ecKey = EthECKey.GenerateKey();
            user.OwnerPrivateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            user.OwnerAddress = ecKey.GetPublicAddress();

            await _dbContext.SaveChangesAsync(ct);
        }

        var request = new Contracts.Bmoni.OwnerProofChallengeRequest("CNGN", user.OwnerAddress);
        var response = await _bmoniApi.RequestOwnerProofChallengeAsync(user.BmoniUserId, request);

        if (response == null || !response.IsSuccessStatusCode || response.Content == null)
        {
            _logger.LogError("Failed to request owner proof challenge: {Response}", response?.Error);
            return new ChallengeResult(Success: false, ErrorMessage: "Failed to request owner proof challenge.");
        }

        user.PendingChallengeId = response.Content.ChallengeId;
        user.PendingChallengeMessage = response.Content.Message;
        user.PendingChallengeExpiresAt = response.Content.ExpiresAt;
        user.OnboardingStatus = OnboardingStatus.ChallengeRequested;
        user.OnboardingStatus = OnboardingStatus.ChallengeRequested;
        await _dbContext.SaveChangesAsync(ct);

        return new ChallengeResult(true, null, response.Content.ChallengeId, response.Content.Message, response.Content.ExpiresAt);

    }
    public async Task<StartNigeriaResult> StartNigeriaOnboardingAsync(Guid localUserId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == localUserId, ct);
        if (user is null)
        {
            return new StartNigeriaResult(Success: false, ErrorMessage: "User not found.");
        }

        if (string.IsNullOrWhiteSpace(user.SmartWalletAddress))
        {
            return new StartNigeriaResult(Success: false, ErrorMessage: "Wallet must be created before activating the Nigeria rail.");
        }

        var bmoniRequest = new Contracts.Bmoni.StartNigeriaRequest(
            Bvn: SandboxTestBvn,
            NgnWalletAddress: user.SmartWalletAddress,
            NgnWalletIndex: 1 // TODO: confirm correct index value with BMONI if this fails
        );

        var bmoniResponse = await _bmoniApi.StartNigeriaOnboardingAsync(user.BmoniUserId!, bmoniRequest);
        if (bmoniResponse == null || !bmoniResponse.IsSuccessStatusCode || bmoniResponse.Content == null)
        {
            _logger.LogError("Failed to start Nigeria onboarding. Response: {Response}", bmoniResponse?.Error);
            return new StartNigeriaResult(Success: false, ErrorMessage: bmoniResponse?.Error?.Message ?? "Failed to start Nigeria onboarding.");
        }

        user.OnboardingStatus = OnboardingStatus.RailActive;
        await _dbContext.SaveChangesAsync(ct);

        return new StartNigeriaResult(Success: true, Message: "Succesfully Activated the Nigerian rail");
    }
    public async Task<OnboardingStatusResult> GetOnboardingStatusAsync(Guid localUserId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == localUserId, ct);
        if (user is null || string.IsNullOrWhiteSpace(user.BmoniUserId))
        {
            return new OnboardingStatusResult(Success: false, ErrorMessage: "User not found or not yet created in Bmoni.");
        }

        var bmoniResponse = await _bmoniApi.GetOnboardingStatusAsync(user.BmoniUserId);
        if (bmoniResponse == null || !bmoniResponse.IsSuccessStatusCode || bmoniResponse.Content == null)
        {
            _logger.LogError("Failed to get onboarding status. Response: {Response}", bmoniResponse?.Error);
            return new OnboardingStatusResult(Success: false, ErrorMessage: "Failed to retrieve onboarding status.");
        }

        return new OnboardingStatusResult(Success: true, Status: new(
            AnchorStatus: bmoniResponse.Content.AnchorStatus,
            BridgeStatus: bmoniResponse.Content.BridgeStatus,
            MoneriumStatus: bmoniResponse.Content.MoneriumStatus,
            PaytrieStatus: bmoniResponse.Content.PaytrieStatus,
            EtherfuseStatus: bmoniResponse.Content.EtherfuseStatus
        ));
    }
    public async Task<CreateManagedWalletResult> CreateManagedWalletAsync(Guid localUserId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == localUserId, ct);
        if (user is null || string.IsNullOrWhiteSpace(user.BmoniUserId))
        {
            return new CreateManagedWalletResult(Success: false, ErrorMessage: "User not found or not yet created in Bmoni.");
        }

        if (string.IsNullOrWhiteSpace(user.OwnerAddress) || string.IsNullOrWhiteSpace(user.OwnerPrivateKey))
        {
            return new CreateManagedWalletResult(Success: false, ErrorMessage: "Owner keypair not found.");
        }

        if (string.IsNullOrWhiteSpace(user.PendingChallengeId) || string.IsNullOrWhiteSpace(user.PendingChallengeMessage))
        {
            return new CreateManagedWalletResult(Success: false, ErrorMessage: "No pending challenge found. Request a challenge first.");
        }

        if (user.PendingChallengeExpiresAt != null && DateTime.Parse(user.PendingChallengeExpiresAt) < DateTime.UtcNow)
        {
            return new CreateManagedWalletResult(Success: false, ErrorMessage: "Challenge has expired. Request a new one.");
        }

        var signature = SignChallenge(user.OwnerPrivateKey, user.PendingChallengeMessage);

        var bmoniRequest = new Contracts.Bmoni.CreateManagedWalletRequest(
            Currency: "CNGN",
            UserOwnerAddress: user.OwnerAddress,
            OwnerProofChallengeId: user.PendingChallengeId,
            OwnerProofSignature: signature
        );

        var bmoniResponse = await _bmoniApi.CreateManagedWalletAsync(user.BmoniUserId, bmoniRequest);
        if (bmoniResponse == null || !bmoniResponse.IsSuccessStatusCode || bmoniResponse.Content == null)
        {
            _logger.LogError("Failed to create managed wallet. Response: {Response}", bmoniResponse?.Error?.Message);
            return new CreateManagedWalletResult(Success: false, ErrorMessage: bmoniResponse?.Error?.Message ?? "Failed to create managed wallet.");
        }

        user.SmartWalletId = bmoniResponse.Content.SmartWalletId;
        user.SmartWalletAddress = bmoniResponse.Content.SmartWalletAddress;
        user.PendingChallengeId = null;
        user.PendingChallengeMessage = null;
        user.PendingChallengeExpiresAt = null;
        user.OnboardingStatus = OnboardingStatus.WalletCreated;
        await _dbContext.SaveChangesAsync(ct);

        return new CreateManagedWalletResult(Success: true, SmartWalletAddress: user.SmartWalletAddress);
    }
    public string SignChallenge(string privateKeyHex, string challengeMessage)
    {
        var signer = new EthereumMessageSigner();
        return signer.EncodeUTF8AndSign(challengeMessage, new EthECKey(privateKeyHex));
    }
}

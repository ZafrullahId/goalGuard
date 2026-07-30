namespace goalGuard.Entity
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PendingChallengeId { get; set; } = string.Empty;
        public string? PendingChallengeMessage { get; set; } = string.Empty;
        public string? PendingChallengeExpiresAt { get; set; } = string.Empty;

        // --- BMONI identity ---
        public string? BmoniUserId { get; set; }          // from POST /v1/users

        // --- Smart wallet ownership (from owner-proof-challenge + create-managed) ---
        public string? OwnerAddress { get; set; }          // 0x... EVM address
        public string? OwnerPrivateKey { get; set; }        // sandbox only - see note below
        public string? SmartWalletId { get; set; }          // from create-managed response
        public string? SmartWalletAddress { get; set; }     // from create-managed response

        // --- KYC / rail activation ---
        public string? Bvn { get; set; }                    // sandbox test BVN: 22222222222
        public bool NigeriaRailActive { get; set; } = false;

        // --- Funding ---
        public bool WalletFunded { get; set; } = false;

        // --- Progress tracking ---
        public OnboardingStatus OnboardingStatus { get; set; } = OnboardingStatus.NotStarted;

        // --- Audit ---
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum OnboardingStatus
    {
        NotStarted = 0,
        UserCreated = 1,          // POST /v1/users succeeded
        ChallengeRequested = 2,   // owner-proof-challenge received, awaiting signature
        WalletCreated = 3,        // create-managed succeeded
        KycSubmitted = 4,         // BVN submitted
        KycApproved = 5,          // GET onboarding/status confirms approval
        RailActive = 6,           // start-nigeria succeeded
        Funded = 7,               // test funds confirmed via balances endpoint
        Ready = 8                 // fully onboarded, usable for transfers/evaluate
    }
}

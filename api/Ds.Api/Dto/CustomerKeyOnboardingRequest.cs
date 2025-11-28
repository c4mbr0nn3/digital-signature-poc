using Ds.Core.Entities;

namespace Ds.Api.Dto;

public record CustomerKeyOnboardingRequest
{
    public required string PublicKey { get; set; } // Base64 encoded
    public required string EncryptedPrivateKey { get; set; } // Base64 encoded
    public required string Salt { get; set; } // Base64 encoded
    public required string Iv { get; set; } // Base64 encoded

    public CustomerKey ToEntity(int customerId)
    {
        return new CustomerKey
        {
            CustomerId = customerId,
            PublicKey = Convert.FromBase64String(PublicKey),
            EncryptedPrivateKey = Convert.FromBase64String(EncryptedPrivateKey),
            PrivateKeySalt = Convert.FromBase64String(Salt),
            Iv = Convert.FromBase64String(Iv),
        };
    }
}
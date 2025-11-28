namespace Ds.Api.Dto;

public class CustomerActiveKeyResponse
{
    public int Id { get; set; }
    public string EncryptedPrivateKey { get; set; } // Base64 encoded
    public string Salt { get; set; } // Base64 encoded
    public string Iv { get; set; } // Base64 encoded
}
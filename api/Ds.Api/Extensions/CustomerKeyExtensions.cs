using System.Security.Cryptography;
using Ds.Api.Dto;
using Ds.Core.Entities;

namespace Ds.Api.Extensions;

public static class CustomerKeyExtensions
{
    extension(CustomerKey ck)
    {
        public CustomerActiveKeyResponse ToCustomerActiveKeyResponse() =>
            new()
            {
                Id = ck.Id,
                EncryptedPrivateKey = Convert.ToBase64String(ck.EncryptedPrivateKey),
                Salt = Convert.ToBase64String(ck.PrivateKeySalt),
                Iv = Convert.ToBase64String(ck.Iv),
            };

        public bool VerifyData(byte[] data, byte[] signature)
        {
            // verify the signature using the ECDSA public key
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportSubjectPublicKeyInfo(ck.PublicKey, out _);
            return ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }
    }
}
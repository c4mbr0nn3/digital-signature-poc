using Ds.Api.Dto;
using Ds.Core.Entities;

namespace Ds.Api.Extensions;

public static class CustomerKeyExtensions
{
    public static CustomerActiveKeyResponse ToCustomerActiveKeyResponse(this CustomerKey ck) =>
        new()
        {
            EncryptedPrivateKey = Convert.ToBase64String(ck.EncryptedPrivateKey),
            Salt = Convert.ToBase64String(ck.PrivateKeySalt),
            Iv = Convert.ToBase64String(ck.Iv),
        };
}
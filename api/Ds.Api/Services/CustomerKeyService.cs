using Ds.Api.Dto;
using Ds.Api.Extensions;
using Ds.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ds.Api.Services;

public interface ICustomerKeyService
{
    Task<CustomerActiveKeyResponse?> GetActiveUserKey();
}

public class CustomerKeyService(AppDbContext db) : ICustomerKeyService
{
    public async Task<CustomerActiveKeyResponse?> GetActiveUserKey()
    {
        var customerId = await db.Customers.Select(x => x.Id).FirstOrDefaultAsync();
        if (customerId == 0) throw new Exception("Customer not found");

        var activeKey = await db.CustomerKeys
            .Where(ck => ck.CustomerId == customerId && ck.IsActive)
            .Select(ck => ck.ToCustomerActiveKeyResponse())
            .FirstOrDefaultAsync();

        return activeKey;
    }
}
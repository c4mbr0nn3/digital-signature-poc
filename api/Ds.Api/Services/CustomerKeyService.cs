using Ds.Api.Dto;
using Ds.Api.Extensions;
using Ds.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ds.Api.Services;

public interface ICustomerKeyService
{
    Task<CustomerActiveKeyResponse?> GetActiveUserKey();
    Task<int> OnboardCustomerKey(CustomerKeyOnboardingRequest request);
    Task RotateCustomerKey(CustomerKeyOnboardingRequest request);
}

public class CustomerKeyService(AppDbContext db) : ICustomerKeyService
{
    public async Task<CustomerActiveKeyResponse?> GetActiveUserKey()
    {
        var customerId = await db.Customers.Select(x => x.Id).FirstOrDefaultAsync();
        if (customerId == 0) throw new Exception("Customer not found");

        var activeKey = await db.CustomerKeys
            .Where(ck => ck.CustomerId == customerId && ck.SupersededAt == null)
            .Select(ck => ck.ToCustomerActiveKeyResponse())
            .FirstOrDefaultAsync();

        return activeKey;
    }

    public async Task<int> OnboardCustomerKey(CustomerKeyOnboardingRequest request)
    {
        var customer = await db.Customers.FirstOrDefaultAsync();
        if (customer == null) throw new Exception("Customer not found");
        var customerId = customer.Id;

        var anyKeys = await db.CustomerKeys.AnyAsync(ck => ck.CustomerId == customerId);
        if (anyKeys) throw new Exception("Customer already has a key onboarded");
        var customerKey = request.ToEntity(customerId);
        await db.CustomerKeys.AddAsync(customerKey);
        await db.SaveChangesAsync();

        customer.ActiveKeyId = customerKey.Id;
        await db.SaveChangesAsync();
        return customerKey.Id;
    }

    public async Task RotateCustomerKey(CustomerKeyOnboardingRequest request)
    {
        var customer = await db.Customers.FirstOrDefaultAsync();
        if (customer == null) throw new Exception("Customer not found");
        var customerId = customer.Id;

        var activeKey = await db.CustomerKeys
            .FirstOrDefaultAsync(ck => ck.CustomerId == customerId && ck.SupersededAt == null);
        if (activeKey == null) throw new Exception("No active key found to rotate");

        activeKey.SupersededAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var newCustomerKey = request.ToEntity(customerId);
        await db.CustomerKeys.AddAsync(newCustomerKey);
        await db.SaveChangesAsync();

        customer.ActiveKeyId = newCustomerKey.Id;
        await db.SaveChangesAsync();
    }
}
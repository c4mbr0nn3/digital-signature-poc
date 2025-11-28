using Bogus;
using Ds.Api.Dto;
using Ds.Api.Extensions;
using Ds.Core.Entities;
using Ds.Core.Enumerations;
using Ds.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ds.Api.Services;

public interface ITradeRecommendationService
{
    Task<List<TradeProposalDetails>> GetTradesRecommendations();
    Task<TradeProposalCreateResponse> CreateRandomTradeRecommendation();
    Task SignTradeRecommendation(int id, TradeSignRequest request);
}

public class TradeRecommendationService(AppDbContext db) : ITradeRecommendationService
{
    public async Task<List<TradeProposalDetails>> GetTradesRecommendations()
    {
        var customerId = await db.Customers.Select(x => x.Id).FirstOrDefaultAsync();
        if (customerId == 0) throw new Exception("Customer not found");

        var tradeRecommendation = await db.TradeRecommendations
            .Where(tr => tr.CustomerId == customerId)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();

        var result = tradeRecommendation
            .Select(tr => tr.ToTradeProposalDetails())
            .ToList();

        return result;
    }

    public async Task<TradeProposalCreateResponse> CreateRandomTradeRecommendation()
    {
        var customerId = await db.Customers.Select(x => x.Id).FirstOrDefaultAsync();
        if (customerId == 0) throw new Exception("Customer not found");

        var tradesFaker = new Faker<TradeData>()
            .RuleFor(td => td.Isin, f => f.Isin())
            .RuleFor(td => td.Currency, _ => "EUR")
            .RuleFor(td => td.Quantity, f => f.Random.Int(1, 100))
            .RuleFor(td => td.Price, f => f.Random.Decimal(0.01m, 100.00m));

        var metadataFaker = new Faker<Metadata>()
            .RuleFor(
                m => m.Trades,
                f => tradesFaker.Generate(f.Random.Int(1, 10)).ToList());

        var tradeRecommendationFaker = new Faker<TradeRecommendation>()
            .RuleFor(tr => tr.CustomerId, _ => 1)
            .RuleFor(tr => tr.Metadata, metadataFaker.Generate());

        var tradeRecommendation = tradeRecommendationFaker.Generate();

        await db.TradeRecommendations.AddAsync(tradeRecommendation);
        await db.SaveChangesAsync();

        return tradeRecommendation.ToTradeProposalCreateResponse();
    }

    public async Task SignTradeRecommendation(int id, TradeSignRequest request)
    {
        var customer = await db.Customers.FirstOrDefaultAsync();
        if (customer == null) throw new Exception("Customer not found");
        if (customer.ActiveKeyId == null) throw new Exception("Customer has no active signing key");
        var customerId = customer.Id;

        var tradeRecommendation = await db.TradeRecommendations
            .FirstOrDefaultAsync(tr => tr.Id == id && tr.CustomerId == customerId);
        if (tradeRecommendation == null) throw new Exception("Trade recommendation not found");

        if (!TryGetSigningKey(request.SigningKeyId, customer.ActiveKeyId.Value, out var signingKey))
        {
            throw new Exception("Invalid signing key");
        }

        // this seems redundant but is to satisfy the nullable warning
        if (signingKey == null)
        {
            throw new Exception("Signing key not found");
        }

        // TODO: implement signature verification logic here

        // after verification, go on to update the trade recommendation
        tradeRecommendation.Sign(request);
    }

    private bool TryGetSigningKey(int signingKeyId, int customerActiveKey, out CustomerKey? signingKey)
    {
        if (signingKeyId != customerActiveKey)
        {
            signingKey = null;
            return false;
        }

        signingKey = db.CustomerKeys.FirstOrDefault(ck => ck.Id == signingKeyId);
        return signingKey != null;
    }
}
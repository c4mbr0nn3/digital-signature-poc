using Bogus;
using Ds.Api.Dto;
using Ds.Api.Extensions;
using Ds.Core.Entities;
using Ds.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ds.Api.Services;

public interface ITradeRecommendationService
{
    Task<TradeProposalCreateResponse> CreateRandomTradeRecommendation();
}

public class TradeRecommendationService(AppDbContext db) : ITradeRecommendationService
{
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

        return tradeRecommendation.ToDto();
    }
}
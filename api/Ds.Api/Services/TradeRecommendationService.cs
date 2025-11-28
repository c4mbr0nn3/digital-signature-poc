using System.Text;
using Bogus;
using Ds.Api.Dto;
using Ds.Api.Extensions;
using Ds.Core.Entities;
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

        var canonicalString = MakeCanonicalStringV1(request, tradeRecommendation);
        var isValidSignature = signingKey.VerifyData(canonicalString.ToUTF8Bytes(), request.SignatureBytes);
        if (!isValidSignature)
        {
            throw new Exception("Invalid signature");
        }

        tradeRecommendation.Sign(request);
        await db.SaveChangesAsync();
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

    /// <summary>
    /// Create the canonical string for signing version V1
    /// TRADEv1|{trade_id}|{action}|{signed_at}|{metadata_hash}
    /// </summary>
    /// <param name="request"></param>
    /// <param name="tradeRecommendation"></param>
    /// <returns></returns>
    private static string MakeCanonicalStringV1(TradeSignRequest request, TradeRecommendation tradeRecommendation)
    {
        const string prefix = "TRADEv1";
        const char separator = '|';
        var tradeId = tradeRecommendation.Id.ToString();
        var action = request.SignedAction;
        var signedAt = request.SignedAt.ToString();
        var metadataHash = tradeRecommendation.MetadataSha256;
        var builder = new StringBuilder();
        builder.Append(prefix).Append(separator);
        builder.Append(tradeId).Append(separator);
        builder.Append(action).Append(separator);
        builder.Append(signedAt).Append(separator);
        builder.Append(metadataHash);
        return builder.ToString();
    }
}
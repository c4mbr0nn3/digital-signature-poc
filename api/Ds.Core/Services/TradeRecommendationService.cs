using Ds.Core.Entities;

namespace Ds.Core.Services;


public interface ITradeRecommendationService
{
    TradeRecommendation CreateRandomTradeRecommendation();
}

public class TradeRecommendationService : ITradeRecommendationService
{
    public TradeRecommendation CreateRandomTradeRecommendation()
    {

        throw new NotImplementedException();
    }
}
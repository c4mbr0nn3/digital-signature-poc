namespace Ds.Core.Enumerations;

public enum TradeStatus
{
    Pending = 1,
    Signed = 2
}

public static class TradeStatusExtensions
{
    public static string ToLabel(this TradeStatus status)
    {
        return status switch
        {
            TradeStatus.Pending => "pending",
            TradeStatus.Signed => "signed",
            _ => "unknown"
        };
    }
}
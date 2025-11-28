namespace Ds.Core.Enumerations;

public enum SignAction
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3
}

public static class SignActionExtensions
{
    public static string ToLabel(this SignAction action)
    {
        return action switch
        {
            SignAction.Pending => "pending",
            SignAction.Accepted => "accepted",
            SignAction.Rejected => "rejected",
            _ => "unknown"
        };
    }
}
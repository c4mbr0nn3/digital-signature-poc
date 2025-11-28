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

public static class SignActionParser
{
    public static SignAction Parse(string action)
    {
        return action.ToLower() switch
        {
            "pending" => SignAction.Pending,
            "accepted" => SignAction.Accepted,
            "rejected" => SignAction.Rejected,
            _ => throw new ArgumentException("Invalid sign action", nameof(action))
        };
    }
}
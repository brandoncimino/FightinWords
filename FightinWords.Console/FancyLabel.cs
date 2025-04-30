using Spectre.Console;

namespace FightinWords.Console;

public record FancyLabel(string Name, string Icon)
{
    public Style? Style { get; init; }

    public static readonly FancyLabel Error     = new("Error", "❌");
    public static readonly FancyLabel Rejection = new("Rejection", "👎");

    public const string ErrorDefault     = "❌";
    public const string RejectionDefault = "👎";
    public const string SuccessDefault   = "✅";
    public const string StaleWordDefault = "🔁";
}

public interface IConfigOwner<TValue>
{
}

public interface IConicStyle
{
    public static abstract string? Emoji { get; }
}
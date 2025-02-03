using FightinWords.Submissions;

namespace FightinWords.Console;

public enum UnknownCommandBehavior
{
    /// <summary>
    /// Entering the <see cref="CommandInterceptor{COMMAND}.Prefix"/> followed by an unknown command results in a <see cref="Failure"/>. 
    /// </summary>
    Reject,

    /// <summary>
    /// Entering the <see cref="CommandInterceptor{COMMAND}.Prefix"/> followed by an unknown command returns <c>null</c>, i.e. "no command".
    /// </summary>
    Ignore
}
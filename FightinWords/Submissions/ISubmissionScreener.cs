using OneOf;

namespace FightinWords.Submissions;

/// <summary>
/// Checks if a given <typeparamref name="INPUT"/> is even <see cref="Legality.Legal"/>.
///
/// This is in contrast to <see cref="WordLookup.IWordLookup"/>, which checks if a <i><see cref="Legality.Legal"/></i> string is also a word.
/// </summary>
public interface ISubmissionScreener<in INPUT, SUCCESS>
{
    public OneOf<SUCCESS, Failure> ScreenInput(INPUT rawSubmission);
}

public delegate OneOf<SUCCESS, Failure> Screener<in INPUT, SUCCESS>(INPUT rawSubmission);
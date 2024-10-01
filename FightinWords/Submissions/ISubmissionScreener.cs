using System.Collections.Immutable;
using OneOf;

namespace FightinWords.Submissions;

/// <summary>
/// Checks if a given <typeparamref name="INPUT"/> is even <see cref="Legality.Legal"/>.
///
/// This is in contrast to <see cref="WordLookup.IWordLookup"/>, which checks if a <i><see cref="Legality.Legal"/></i> string is also a word.
/// </summary>
public interface ISubmissionScreener<in INPUT, SUCCESS, FAILURE>
{
    public OneOf<SUCCESS, FAILURE> ScreenInput(INPUT rawSubmission);
}
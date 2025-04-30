using FightinWords.WordLookup;

namespace FightinWords.Console.Rendering;

public sealed class LetterPoolDisplay
{
    public static LetterPoolDisplay Create(
        SharedResources sharedResources,
        LetterSorting   initialSort = LetterSorting.Phonological
    )
    {
        var pool = new LetterPoolDisplay(sharedResources);
        pool.Sort(initialSort);
        return pool;
    }

    public enum LetterSorting
    {
        AsOriginallyGiven,
        Alphabetical,
        Phonological,
        Random,
    }

    private static readonly int PossibleLettersSortings = Enum.GetValues<LetterSorting>()
                                                              .Distinct()
                                                              .Count();

    private readonly SharedResources _sharedResources;
    private readonly Grapheme[]      _currentDisplay;
    public           IList<Grapheme> CurrentDisplay => _currentDisplay.AsReadOnly();
    public           LetterSorting   CurrentSorting { get; private set; }

    private readonly Comparison<Grapheme> _alphabetical;
    private readonly Comparison<Grapheme> _phonological;

    private LetterPoolDisplay(SharedResources sharedResources)
    {
        _sharedResources = sharedResources;
        _currentDisplay  = _sharedResources.GamePlan.ProgenitorPool.ToArray();

        // Sorts by the `language`'s culture
        _alphabetical = (a, b) => _sharedResources.GamePlan.Language.CultureInfo()
                                                  .CompareInfo
                                                  .Compare(a.Source, b.Source);

        // Sorts by `Phonology` -> `_alphabetical`
        _phonological = (a, b) =>
        {
            // ⚠ Enum only contains a `.CompareTo(object?)` method, which would cause the enum to boxed for absolutely no reason! 
            int aPhonology = (int)a.GetPhonology(_sharedResources.GamePlan.Language);
            int bPhonology = (int)b.GetPhonology(_sharedResources.GamePlan.Language);

            var relationship = aPhonology.CompareTo(bPhonology);
            return relationship == 0 ? _alphabetical(a, b) : relationship;
        };
    }

    /// <summary>
    /// Sorts the <see cref="_currentDisplay"/> based on the given <see cref="LetterSorting"/>.
    /// </summary>
    /// <param name="newSorting"></param>
    /// <returns><c>true</c> if we were able to apply the <paramref name="newSorting"/> <i>(either because it differed from the <see cref="CurrentSorting"/> or, in the case of <see cref="LetterSorting.Random"/>, it was not idempotent)</i></returns>
    /// <exception cref="ArgumentOutOfRangeException">You've given me a junk <see cref="LetterSorting"/></exception>
    /// <remarks>
    /// <ul>
    /// <li>After this method, <see cref="CurrentSorting"/> should <b><i>always</i></b> equal <paramref name="newSorting"/>.</li>
    /// <li>Just because this method returned <c>true</c> doesn't mean that the <see cref="_currentDisplay"/> changed, because the <paramref name="newSorting"/>
    /// may have produced the same result.
    /// For example, <c>['a', 'b', 'c']</c> is sorted both <see cref="LetterSorting.Alphabetical"/>ly and <see cref="LetterSorting.Phonological"/>ly.</li>
    /// </ul>
    /// </remarks>
    public bool Sort(LetterSorting newSorting)
    {
        if (newSorting == CurrentSorting && newSorting is not LetterSorting.Random)
        {
            return false;
        }

        switch (newSorting)
        {
            case LetterSorting.Random:
                _sharedResources.Random.Shuffle(_currentDisplay);
                break;
            case LetterSorting.Alphabetical:
                _currentDisplay.AsSpan().Sort(_alphabetical);
                break;
            case LetterSorting.Phonological:
                _currentDisplay.AsSpan().Sort(_phonological);
                break;
            case LetterSorting.AsOriginallyGiven:
                _sharedResources.GamePlan.ProgenitorPool.Letters.Values.CopyTo(_currentDisplay);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newSorting), newSorting, null);
        }

        CurrentSorting = newSorting;
        return true;
    }

    /// <summary>
    /// <see cref="Sort"/>s using the next-highest <see cref="LetterSorting"/>, looping around after the last one.
    /// </summary>
    /// <returns><c>true</c> if we were able to apply the next <see cref="LetterSorting"/> <i>(which should always be the case, since this should always result in a change to the <see cref="CurrentSorting"/>)</i></returns>
    public bool SortNext()
    {
        var nextSorting = ((int)CurrentSorting + 1) % PossibleLettersSortings;
        return Sort((LetterSorting)nextSorting);
    }
}
using FightinWords.WordLookup;

namespace FightinWords.Console;

public sealed class LetterPoolDisplay
{
    public static LetterPoolDisplay Create(
        SharedResources sharedResources,
        LetterSorting   initialSort = LetterSorting.Phonological
    )
    {
        return new LetterPoolDisplay(sharedResources).Sort(initialSort);
    }

    public enum LetterSorting
    {
        AsOriginallyGiven,
        Alphabetical,
        Phonological,
        Random,
    }

    private readonly SharedResources _sharedResources;
    private readonly Grapheme[]      _currentDisplay;
    public           IList<Grapheme> CurrentDisplay => _currentDisplay.AsReadOnly();
    private          LetterSorting   _currentSorting = LetterSorting.AsOriginallyGiven;

    private readonly Comparison<Grapheme> _alphabetical;
    private readonly Comparison<Grapheme> _phonological;

    public LetterPoolDisplay(SharedResources sharedResources)
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

    public LetterPoolDisplay Sort(LetterSorting newStyle)
    {
        if (newStyle == _currentSorting && newStyle is not LetterSorting.Random)
        {
            return this;
        }

        switch (newStyle)
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
                throw new ArgumentOutOfRangeException(nameof(newStyle), newStyle, null);
        }

        _currentSorting = newStyle;
        return this;
    }
}
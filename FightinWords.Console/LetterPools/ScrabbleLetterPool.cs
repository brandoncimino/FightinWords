using System.Text;
using FightinWords.Scoring;
using FightinWords.WordLookup;

namespace FightinWords.Console.LetterPools;

public sealed record ScrabbleLetterPool(Language Language, int PoolSize) : ILetterPool
{
    public Word LockIn(Random random)
    {
        Preconditions.Require(PoolSize > 0);

        var scrabbleTiles = CreateScrabbleTiles(Language);
        var chosenTiles   = scrabbleTiles.DrawX(random, PoolSize);
        return new Word(chosenTiles);
    }

    private static TileBag<Grapheme> CreateScrabbleTiles(Language language)
    {
        var scrabbleBox = Enumerable.Range('a', 'z' - 'a' + 1)
                                    .Select(c => new Rune(c))
                                    .ToDictionary(Grapheme.Parse, c => ScrabbleScorer.GetLetterCount(c, language));
        return new TileBag<Grapheme>(scrabbleBox);
    }
}
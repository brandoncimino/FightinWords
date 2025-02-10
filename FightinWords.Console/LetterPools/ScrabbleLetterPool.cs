using System.Text;
using FightinWords.Scoring;
using FightinWords.WordLookup;

namespace FightinWords.Console.LetterPools;

public sealed class ScrabbleLetterPool : RandomPoolBase<TileBag<Grapheme>>
{
    public override TileBag<Grapheme> CreateNewState()
    {
        return CreateScrabbleTiles(Language);
    }

    public override Grapheme GetRandomLetter(Random random, TileBag<Grapheme> tileBag) => tileBag.DrawOne(random);

    public override Grapheme ExchangeForVowel(Random random, TileBag<Grapheme> tileBag, Grapheme toBeExchanged)
    {
        var vowel = tileBag.DrawOne(random, it => it.GetPhonology(Language) is Phonology.Vowel or Phonology.Semivowel);
        tileBag.Add(toBeExchanged);
        return vowel;
    }

    private static TileBag<Grapheme> CreateScrabbleTiles(Language language)
    {
        var scrabbleBox = Enumerable.Range('a', 'z' - 'a' + 1)
                                    .Select(c => new Rune(c))
                                    .ToDictionary(Grapheme.Parse, c => ScrabbleScorer.GetLetterCount(c, language));
        return new TileBag<Grapheme>(scrabbleBox);
    }
}
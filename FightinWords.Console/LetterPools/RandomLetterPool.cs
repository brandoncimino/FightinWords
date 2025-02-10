using OneOf.Types;

namespace FightinWords.Console.LetterPools;

/// <summary>
/// A <see cref="ILetterPool"/> that randomly picks a <i>(lowercase)</i> letter from <c>a</c> to <c>z</c> with equal weight.
/// </summary>
public sealed class RandomLetterPool : RandomPoolBase<None>
{
    public override None CreateNewState() => default;

    public override Grapheme GetRandomLetter(Random random, None _)
    {
        var randomChar     = (char)random.Next('a', 'z' + 1);
        var randomGrapheme = Grapheme.Parse(randomChar);
        return randomGrapheme;
    }

    public override Grapheme ExchangeForVowel(Random random, None _, Grapheme toBeExchanged)
    {
        var vowelChar = Vowels[random.Next(Vowels.Length)];
        return Grapheme.Parse(vowelChar);
    }
}
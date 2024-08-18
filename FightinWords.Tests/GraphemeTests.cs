using System.Collections.Immutable;
using System.Text;

namespace FightinWords;

public class GraphemeTests
{
    [Test]
    public void CharLetters_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Letters))] char c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.ToString());
    }

    [Test]
    public void GetAsciiDirect()
    {
        var ascii = SingleCharData.AsciiChars;
        Console.WriteLine(ascii);
    }
    
    [Test]
    public void CharNumbers_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Numbers))] char c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.ToString());
    }
    
    [Test]
    public void CharSymbols_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Symbols))] char c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.ToString());
    }
    
    private static void Assert_GraphemeSource(Grapheme actual, string expectedSource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedSource);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsDefault, Is.False);
            Assert.That(actual.LengthInChars, Is.EqualTo(expectedSource.Length));
            Assert.That(actual.LengthInRunes, Is.EqualTo(expectedSource.EnumerateRunes().Count()));
            Assert.That(actual.Source.Any(char.IsWhiteSpace), Is.False);
            Assert.That(actual.Source.Any(char.IsControl), Is.False);
            Assert.That(actual.Source.EnumerateRunes().Any(Rune.IsWhiteSpace), Is.False);
            Assert.That(actual.Source.EnumerateRunes().Any(Rune.IsControl), Is.False);
            Assert.That(actual.Source, Is.EqualTo(expectedSource));
        });
    }
}
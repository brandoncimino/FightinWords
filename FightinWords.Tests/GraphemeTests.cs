using System.Text;

namespace FightinWords;

public class GraphemeTests
{
    [Test]
    public void CharLetters_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Letters))] CharData c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.Value.ToString());
    }

    [Test]
    public void CharNumbers_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Numbers))] CharData c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.Value.ToString());
    }

    [Test]
    public void CharSymbols_AreValid([ValueSource(typeof(SingleCharData), nameof(SingleCharData.Symbols))] CharData c)
    {
        Assert_GraphemeSource(Grapheme.Parse(c), c.Value.ToString());
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
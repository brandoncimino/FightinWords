using System.Collections.Immutable;
using System.Text;
using FightinWords.Data;
using FluentAssertions;
using FluentAssertions.Execution;

namespace FightinWords;

/// <summary>
/// TODO: Boy this stuff is both: A) a mess, and B) absurd overkill 
/// </summary>
public class LetterTests
{
    #region Data

    private static IEnumerable<TestData.StringData> GenerateCombiningCharacterStrings()
    {
        var random = TestHelpers.CreateRandom();

        Rune[] baseRunes      = [new('a'), SingleRuneData.Rainbow.Rune, new('1')];
        var    diacritics     = SingleCharData.Diacritics_Combining;
        var    diacriticCount = Enumerable.Range(1, 3).ToImmutableArray();
        var generated = CartesianProduct.Of(
            baseRunes,
            diacriticCount
        ).Select(
            data =>
            {
                var sb = new StringBuilder();
                sb.Append(data.a);
                for (int i = 0; i < data.b; i++)
                {
                    sb.Append(diacritics[random.Next(diacritics.Length)]);
                }

                return sb.ToString();
            }
        );

        return
        [
            ..generated,
            $"n{Diacritic.Tilde.Combining}" /* decomposed enye */,
            SingleCharData.LowerEnye /* composed enye */,
            SingleCharData.PileOfDiacritics,
        ];
    }

    #endregion

    [Test]
    public void CombiningCharactersTest(
        [ValueSource(nameof(GenerateCombiningCharacterStrings))]
        TestData.StringData stringWithCombiningCharacters
    )
    {
        Assert_GraphemeSource(Grapheme.Parse(stringWithCombiningCharacters.Content), stringWithCombiningCharacters.Content);
    }
    
    [Test]
    public void TrimCombiningCharacters([ValueSource(typeof(Diacritic), nameof(Diacritic.All))] Diacritic diacritic)
    {
        TestHelpers.AssertEquals(Grapheme.TrimCombiningCharacters($"a{diacritic.Combining}").ToString(), "a");
        TestHelpers.AssertEquals(Grapheme.TrimCombiningCharacters(diacritic.Combining.ToString()).ToString(), "");
    }
    
    [Test]
    public void FromString_Invalid(
        [ValueSource(typeof(TestData), nameof(TestData.GetInvalidGraphemeClusters))]
        TestData.StringData graphemeCluster
    )
    {
        Assert.Multiple(() =>
        {
            TestHelpers.AssertEquals(Grapheme.IsGrapheme(graphemeCluster.Content), false);
            Assert.That(() => Grapheme.Parse(graphemeCluster.Content), Throws.ArgumentException);
        });
    }

    [Test]
    public void FromString_Valid([ValueSource(typeof(TestData), nameof(TestData.GetValidGraphemeClusters))] TestData.StringData cluster)
    {
        Assert_GraphemeSource(Grapheme.Parse(cluster.Content), cluster.Content);
    }
    
    #region Assertions

    private static void Assert_GraphemeSource(Grapheme actual, string expectedSource)
    {
        expectedSource = expectedSource.Normalize(NormalizationForm.FormC);
        var label = $"[Grapheme: `{actual}`, Expected: `{expectedSource}`]";
        Assert.Multiple(() =>
        {
            TestHelpers.AssertEquals(actual.IsDefault, false, label);
            TestHelpers.AssertEquals(actual.LengthInChars, expectedSource.Length,label);
            TestHelpers.AssertEquals(actual.LengthInRunes, expectedSource.EnumerateRunes().Count(), label);
            TestHelpers.AssertEquals(actual.Source.LengthInTextElements(), 1, label);
            TestHelpers.AssertEquals(actual.Source.Any(char.IsWhiteSpace), false, label);
            TestHelpers.AssertEquals(actual.Source.Any(char.IsControl), false, label);
            TestHelpers.AssertEquals(actual.Source.EnumerateRunes().Any(Rune.IsWhiteSpace), false, label);
            TestHelpers.AssertEquals(actual.Source.EnumerateRunes().Any(Rune.IsControl), false, label);
            TestHelpers.AssertEquals(actual.Source.IsNormalized(), true, label);
            TestHelpers.AssertEquals(actual.Source, expectedSource, label);
        });
    }

    #endregion
}
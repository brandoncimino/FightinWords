using System.Collections.Immutable;
using System.Diagnostics;
using FightinWords.Console.LetterPools;

namespace FightinWords.LetterPools;

public class TileBagTests
{
    [Test]
    public void EmptyBagTest()
    {
        var emptyBag = new TileBag<char>(ImmutableDictionary<char, int>.Empty);

        TestHelpers.AssertEquals(emptyBag.TotalTiles, 0);
        Assert.Throws<InvalidOperationException>(() => emptyBag.DrawOne(Random.Shared));
        Assert.Throws<ArgumentException>(() => emptyBag.DrawX(Random.Shared, 1));
    }

    private static ImmutableDictionary<char, int> ValidTileCounts => ImmutableDictionary.CreateRange(
        new Dictionary<char, int>()
        {
            ['a'] = 0,
            ['b'] = 1,
            ['c'] = 5,
        });

    public static IEnumerable<ImmutableDictionary<char, int>> AllValidTileCounts() => [ValidTileCounts];

    #region Helpers

    private static TileBag<T> CreateBag<T>(IDictionary<T, int> tileCounts)
    {
        var expectedTotalTiles = tileCounts.Values.Sum();
        var tileBag            = new TileBag<T>(tileCounts);
        TestHelpers.AssertEquals(tileBag.TotalTiles, expectedTotalTiles);
        return tileBag;
    }

    private static ValueArray<T> FlattenTiles<T>(IDictionary<T, int> tileCounts)
    {
        var flattened = tileCounts
                        .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
                        .ToValueArray();

        Debug.Assert(flattened.Count() == tileCounts.Values.Sum());

        return flattened;
    }

    #endregion

    [Test]
    public void DrawX(
        [ValueSource(nameof(AllValidTileCounts))] ImmutableDictionary<char, int> tileCounts,
        [Values(2)]                               int                            x
    )
    {
        var tileBag                = CreateBag(tileCounts);
        var expectedTotalTiles     = tileCounts.Values.Sum();
        var expectedFlattenedTiles = FlattenTiles(tileCounts);

        Preconditions.Require(expectedTotalTiles >= x);
        TestHelpers.AssertEquals(tileBag.TotalTiles, expectedTotalTiles);

        var drawn = tileBag.DrawX(Random.Shared, x);
        TestHelpers.AssertEquals(tileBag.TotalTiles, expectedTotalTiles - x);
        TestHelpers.AssertEquals(drawn.Count(),      x);

        var expectedAfterDraw = expectedFlattenedTiles.Values.RemoveRange(drawn).ToValueArray();

        var actualAfterDraw = tileBag.GetTileCounts();
        TestHelpers.AssertEquals(FlattenTiles(actualAfterDraw), expectedAfterDraw);
    }

    [Test]
    public void DrawX_AllTiles([ValueSource(nameof(AllValidTileCounts))] ImmutableDictionary<char, int> tileCounts)
    {
        var tileBag = new TileBag<char>(tileCounts);
        var drawn   = tileBag.DrawX(Random.Shared, tileCounts.Values.Sum());

        TestHelpers.AssertEquals(drawn.Count(), tileCounts.Values.Sum());

        foreach (var (tile, expectedTileCount) in tileCounts)
        {
            var actualTileCount = drawn.Count(it => it == tile);
            TestHelpers.AssertEquals(actualTileCount, expectedTileCount);
        }
    }

    [Test]
    public void DrawX_TooManyTiles([ValueSource(nameof(AllValidTileCounts))] ImmutableDictionary<char, int> tileCounts)
    {
        var tileBag            = new TileBag<char>(tileCounts);
        var originalTotalTiles = tileBag.TotalTiles;
        Assert.That(() => tileBag.DrawX(Random.Shared, tileBag.TotalTiles + 1), Throws.Exception);
        TestHelpers.AssertEquals(tileBag.TotalTiles, originalTotalTiles);
        Assert.That(tileBag.TotalTiles, Is.EqualTo(originalTotalTiles));
    }
}
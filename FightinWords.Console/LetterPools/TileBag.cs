using System.Collections.Immutable;
using System.Diagnostics;
using JetBrains.Annotations;

namespace FightinWords.Console.LetterPools;

/// <summary>
/// Contains a bunch of <see cref="T"/>s where each <see cref="T"/> can be duplicated.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TileBag<T> where T : notnull
{
    private readonly Dictionary<T, int> _tileCounts;
    public           int                TotalTiles { get; private set; }

    /// <returns>an <b>immutable snapshot</b> of my <see cref="_tileCounts"/></returns>
    public ImmutableDictionary<T, int> GetTileCounts() => _tileCounts.ToImmutableDictionary();

    public TileBag(IEnumerable<KeyValuePair<T, int>> tileCounts)
    {
        this._tileCounts = tileCounts
                           .Where(kvp => kvp.Value switch
                           {
                               < 0 => throw new ArgumentOutOfRangeException(
                                   $"You cannot have a negative number of tiles: {kvp}!"),
                               > 0 => true,
                               0   => false
                           })
                           .ToDictionary();

        this.TotalTiles = _tileCounts.Values.Sum();
    }

    private void ThrowIfEmpty()
    {
        if (TotalTiles <= 0)
        {
            throw new InvalidOperationException("Can't draw a tile from an empty bag!");
        }
    }

    private T DrawByIndex(int tileIndex, Func<T, bool> predicate)
    {
        ThrowIfEmpty();
        Preconditions.Require(tileIndex >= 0 && tileIndex < TotalTiles);

        var tile = GetKeyByWeight(_tileCounts, tileIndex, predicate);
        _tileCounts[tile] -= 1;
        TotalTiles        -= 1;
        return tile;
    }

    public T DrawByTile(T tile)
    {
        _tileCounts[tile] -= 1;
        return tile;
    }

    public TileBag<T> Add(T tile)
    {
        if (_tileCounts.TryAdd(tile, 1) is false)
        {
            _tileCounts[tile] += 1;
        }

        return this;
    }

    public T DrawOne(Random random)
    {
        return DrawByIndex(random.Next(TotalTiles), static _ => true);
    }

    public T DrawOne(Random random, Func<T, bool> predicate)
    {
        var totalMatchingPredicate = _tileCounts
                                     .Where(kvp => predicate(kvp.Key))
                                     .Sum(kvp => kvp.Value);
        var tileIndex = random.Next(totalMatchingPredicate);
        return DrawByIndex(tileIndex, predicate);
    }

    public ValueArray<T> DrawX(Random random, [NonNegativeValue] int drawAmount)
    {
        if (drawAmount == 0)
        {
            return ValueArray<T>.Empty;
        }

        Preconditions.Require(drawAmount <= TotalTiles, true,
            $"Cannot draw {drawAmount} tiles because I only contain {TotalTiles}!");

        var arrayBuilder = ImmutableArray.CreateBuilder<T>(drawAmount);
        for (int i = 0; i < drawAmount; i++)
        {
            arrayBuilder.Add(DrawOne(random));
        }

        return arrayBuilder.MoveToImmutable();
    }

    private static K GetKeyByWeight<K>(IDictionary<K, int> weightedBag, int weight, Func<K, bool> predicate)
    {
        Debug.Assert(weight >= 0);
        var weightLeft = weight;

        foreach (var (k, w) in weightedBag)
        {
            if (predicate(k) is false)
            {
                continue;
            }

            if (weightLeft < w)
            {
                return k;
            }

            weightLeft -= w;
        }

        throw new ArgumentOutOfRangeException(nameof(weight), weight,
            $"Greater than the total weight in the bag, {weightedBag.Where(kvp => predicate(kvp.Key)).Sum(kvp => kvp.Value)}!");
    }
}
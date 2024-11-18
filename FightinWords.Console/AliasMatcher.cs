using System.Collections.Frozen;
using System.Collections.Immutable;
using OneOf;

namespace FightinWords.Console;

/// <summary>
/// Picks the best match for a given string from a list of options with aliases.
/// </summary>
public sealed partial class AliasMatcher
{
    /// <summary>
    /// How well a <see cref="KnownAlias"/> matched a given <see cref="string"/>.
    /// </summary>
    public enum Matchiness
    {
        Exact,
        Partial
    }

    private readonly FrozenDictionary<string, KnownAlias> _knownAliases;

    public AliasMatcher(IEnumerable<KnownAlias> knownAliases)
    {
        _knownAliases = knownAliases.ToFrozenDictionary(it => it.CanonicalName.ToLowerInvariant());
        // Make sure that NONE of the names overlap

        var duplicates = _knownAliases.SelectMany(it => AllNames(it.Value))
                                      .GroupBy(it => it.ToLowerInvariant())
                                      .Where(it => it.Count() > 1)
                                      .ToImmutableList();

        if (duplicates.IsEmpty is false)
        {
            throw new ArgumentException($"Some of the names are duplicated: {string.Join(", ", duplicates)}");
        }
    }

    private IEnumerable<string> AllNames(KnownAlias knownAlias)
    {
        return [knownAlias.CanonicalName, ..knownAlias.Aliases];
    }

    public OneOf<KnownAlias, string> FindMatch(ReadOnlySpan<char> candidate)
    {
        List<KnownAlias>? partialMatches = null;
        foreach (var knownAlias in _knownAliases.Values)
        {
            var matchiness = knownAlias.CheckMatchiness(candidate);
            switch (matchiness)
            {
                case Matchiness.Exact:
                    // If we found an EXACT match, return immediately
                    return knownAlias;
                case Matchiness.Partial:
                    partialMatches ??= [];
                    partialMatches.Add(knownAlias);
                    break;
                default:
                    continue;
            }
        }

        return partialMatches switch
        {
            null or []      => $"Didn't match to any of my known aliases.",
            [var onlyMatch] => onlyMatch,
            _               => $"Ambiguous; it matched all of: {string.Join(", ", partialMatches)}"
        };
    }
}
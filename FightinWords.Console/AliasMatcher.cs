using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
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

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class AliasAttribute(string alias) : Attribute
    {
        public string Alias { get; } = alias;
    }

    private readonly FrozenDictionary<string, KnownAlias> _knownAliases;

    public ImmutableArray<KnownAlias> KnownAliases => _knownAliases.Values;

    #region Constructors & Factories

    private AliasMatcher(FrozenDictionary<string, KnownAlias> knownAliases)
    {
        _knownAliases = knownAliases;
    }

    public AliasMatcher(IEnumerable<KnownAlias> knownAliases) : this(
        knownAliases.ToFrozenDictionary(it => it.CanonicalName.ToLowerInvariant()))
    {
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

    public static AliasMatcher Create(IDictionary<string, IList<string>> aliases)
    {
        return new AliasMatcher(
            aliases.Select(it => new KnownAlias(it.Key, it.Value))
        );
    }

    public static AliasMatcher ForEnum<ENUM>() where ENUM : struct, Enum
    {
        return Create(CreateEnumAliases<ENUM>());
    }

    private static Dictionary<string, IList<string>> CreateEnumAliases<ENUM>() where ENUM : struct, Enum
    {
        var allNames = Enum.GetNames<ENUM>();
        var aliases  = new Dictionary<string, IList<string>>();

        foreach (var name in allNames)
        {
            var canonicalValue = Enum.Parse<ENUM>(name);

            var canonicalName = canonicalValue.ToString();

            if (name == canonicalName)
            {
                aliases.Add(name, []);
            }
            else
            {
                aliases[canonicalName].Add(name);
            }

            var aliasAttributes = typeof(ENUM)
                                  .GetField(name)!
                                  .GetCustomAttributes<AliasAttribute>()
                                  .Select(it => it.Alias);

            foreach (var alias in aliasAttributes)
            {
                aliases[canonicalName].Add(alias);
            }
        }

        return aliases;
    }

    #endregion

    private static IEnumerable<string> AllNames(KnownAlias knownAlias)
    {
        return [knownAlias.CanonicalName, ..knownAlias.Aliases];
    }

    public OneOf<KnownAlias, Failure> FindMatch(ReadOnlySpan<char> candidate)
    {
        ImmutableArray<KnownAlias>.Builder? partialMatches = null;
        // List<KnownAlias>? partialMatches = null;
        var allAliases = _knownAliases.Values;
        for (int i = 0; i < allAliases.Length; i++)
        {
            var knownAlias = allAliases[i];
            var matchiness = knownAlias.CheckMatchiness(candidate);

            switch (matchiness)
            {
                case Matchiness.Exact:
                    // If we found an EXACT match, return immediately
                    return knownAlias;
                case Matchiness.Partial:
                    var maxMatches = allAliases.Length - i;
                    partialMatches ??= ImmutableArray.CreateBuilder<KnownAlias>(maxMatches);
                    partialMatches.Add(knownAlias);
                    break;
                default:
                    continue;
            }
        }

        return partialMatches switch
        {
            // null or []      => $"Didn't match to any of my known aliases.",
            [var onlyMatch] => onlyMatch,
            // _               => $"Ambiguous; it matched all of: {string.Join(", ", partialMatches)}"
            null => new Failure(ImmutableArray<KnownAlias>.Empty),
            _    => new Failure(partialMatches.DrainToImmutable())
        };
    }

    public readonly record struct Failure(ImmutableArray<KnownAlias> PartialMatches)
    {
        public string GetMessage()
        {
            return PartialMatches switch
            {
                [] => "Didn't match any of my known aliases.",
                _  => $"Ambiguous; it matched all of: {string.Join(", ", PartialMatches)}"
            };
        }
    }
}
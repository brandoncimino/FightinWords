using System.Collections.Immutable;
using System.Text;

namespace FightinWords.Console;

public sealed partial class AliasMatcher
{
    /// <summary>
    /// Contains a <see cref="CanonicalName"/> and possible <see cref="Aliases"/> it is allowed to also go by.
    /// </summary>
    public sealed class KnownAlias
    {
        public string                   CanonicalName { get; }
        public ImmutableHashSet<string> Aliases       { get; }

        public KnownAlias(string canonicalName, IEnumerable<string> aliases)
        {
            CanonicalName = ValidateName(canonicalName);
            Aliases = aliases
                      .Select(ValidateName)
                      .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            var builder = ImmutableHashSet.CreateBuilder<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var alias in aliases)
            {
                if (builder.Add(alias) is false)
                {
                    throw new ArgumentException($"The alias `{alias}` is not unique!", nameof(aliases));
                }
            }

            if (Aliases.TryGetValue(CanonicalName, out var matched))
            {
                throw new ArgumentException(
                    $"The {nameof(CanonicalName)} `{CanonicalName}` matches the alias `{matched}`!");
            }
        }

        private static string ValidateName(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            if (name.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException("Cannot contain whitespace!", nameof(name));
            }

            return name;
        }

        private static Matchiness? SingleStringMatchiness(ReadOnlySpan<char> candidate, ReadOnlySpan<char> full)
        {
            if (full.StartsWith(candidate, StringComparison.OrdinalIgnoreCase))
            {
                return full.Length == candidate.Length ? Matchiness.Exact : Matchiness.Partial;
            }

            return null;
        }

        public Matchiness? CheckMatchiness(ReadOnlySpan<char> candidate)
        {
            if (SingleStringMatchiness(candidate, CanonicalName) is { } canonicalMatch)
            {
                return canonicalMatch;
            }

            Matchiness? result = null;
            foreach (var alias in Aliases)
            {
                if (SingleStringMatchiness(candidate, alias) is { } aliasMatch)
                {
                    if (aliasMatch is Matchiness.Exact)
                    {
                        return aliasMatch;
                    }

                    result = aliasMatch;
                }
            }

            return result;
        }

        private bool PrintMembers(StringBuilder builder)
        {
            builder.Append($"\"{CanonicalName}\"");

            if (Aliases.Count > 0)
            {
                builder.Append(", aka ");
                builder.AppendJoin(", ", Aliases.Select(it => $"\"{it}\""));
            }

            return true;
        }
    }
}
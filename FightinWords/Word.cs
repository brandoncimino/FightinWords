using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using OneOf;

namespace FightinWords;

/// <summary>
/// A sequence of <see cref="Grapheme"/>s.
/// </summary>
/// <param name="Letters">My <see cref="Grapheme"/>s</param>
public readonly record struct Word(ImmutableArray<Grapheme> Letters) : IParsable<Word>, IEnumerable<Grapheme>
{
    public static readonly Word Empty = new(ImmutableArray<Grapheme>.Empty);
    public                 int  Length => Letters.Length;
    
    private static bool TrySplitGraphemes(
        string?             fullSource,
        out Word result,
        StringSplitOptions options,
        bool allowFailure
    )
    {
        if (string.IsNullOrEmpty(fullSource))
        {
            result = Empty;
            return true;
        }
        
        var builder = ImmutableArray.CreateBuilder<Grapheme>();
        var erator  = StringInfo.GetTextElementEnumerator(fullSource);
        while (erator.MoveNext())
        {
            var element = erator.GetTextElement();
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                element = element.Trim();
            }

            if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && element is "")
            {
                continue;
            }
            
            if (allowFailure)
            {
                if (Grapheme.TryParse(element, out var singleGrapheme))
                {
                    builder.Add(singleGrapheme);
                    continue;
                }

                result = default;
                return false;
            }
            
            builder.Add(Grapheme.Parse(element));
        }

        result = new Word(builder.DrainToImmutable());
        return true;
    }
    
    static Word IParsable<Word>.Parse(string s, IFormatProvider? provider) => Parse(s);

    [MustUseReturnValue]
    public static Word Parse(string s, StringSplitOptions options = StringSplitOptions.None)
    {
        if (TrySplitGraphemes(s, out var result, options, false))
        {
            return result;
        }

        throw new UnreachableException("Validation failures should've already been handled!");
    }

    static bool IParsable<Word>.TryParse(string? s, IFormatProvider? provider, out Word result) => TryParse(s, out result);

    [MustUseReturnValue]
    public static bool TryParse(string? s, out Word result, StringSplitOptions options = StringSplitOptions.None)
    {
        return TrySplitGraphemes(s, out result, options, true);
    }

    public ImmutableArray<Grapheme>.Enumerator GetEnumerator()
    {
        return Letters.GetEnumerator();
    }
    
    [SuppressMessage("ReSharper", "NotDisposedResourceIsReturned", Justification = "Dude, it's an enumerator, calm down")]
    IEnumerator<Grapheme> IEnumerable<Grapheme>.GetEnumerator() => Letters.AsEnumerable().GetEnumerator();

    [SuppressMessage("ReSharper", "NotDisposedResourceIsReturned", Justification = "Dude, it's an enumerator, calm down")]
    IEnumerator IEnumerable.GetEnumerator() => Letters.AsEnumerable().GetEnumerator();

    public override string ToString() => string.Join("", Letters);
}
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace FightinWords;

/// <summary>
/// <b>To 🧐:</b>
/// <br/>
/// A written character.
/// <br/>
/// <br/>
/// <b>To 💻:</b>
/// <br/>
/// One of:
/// <ul>
/// <li>A single <see cref="char"/></li>
/// <li>A single <see cref="Rune"/> <i>(which may be a <see cref="char.IsSurrogatePair(char,char)"/>)</i></li>
/// <li>A single "grapheme cluster" <i>(referred to as a "text element" in <see cref="StringInfo"/>)</i></li>
/// </ul>
/// </summary>
public readonly record struct Grapheme : IParsable<Grapheme>
{
    [MaybeNull] private readonly string _source;

    /// <summary>
    /// The full sequence of <see cref="char"/>s used to represent this <see cref="Grapheme"/>,
    /// which is guaranteed to be:
    /// <ul>
    /// <li>non-empty</li>
    /// <li><see cref="string.IsNormalized()"/></li>
    /// <li>"well-formed" UTF8 <i>(i.e. it has no unpaired <see cref="char.IsSurrogate(char)"/>s)</i></li>
    /// </ul>
    /// </summary>
    public string Source
    {
        get
        {
            ThrowNullRefIfNotInitialized();
            return _source;
        }
    }

    /// <summary>
    /// If <c>true</c>, then I don't actually contain anything - either I'm a <c>default(</c><see cref="Grapheme"/><c>)</c>
    /// or was instantiated with my no-argument <see cref="M:FightinWords.Grapheme.#ctor"/> constructor.
    /// </summary>
    [MemberNotNullWhen(false, nameof(_source))]
    public bool IsDefault => _source is null;

    /// <summary>
    /// The number of individual <see cref="char"/>s in my <see cref="_source"/>.
    /// </summary>
    public int LengthInChars => _source!.Length;

    /// <summary>
    /// The number of individual <see cref="Rune"/>s in my <see cref="_source"/>.
    /// </summary>
    /// <remarks>
    /// When the <see cref="_source"/> is 3+ characters, we have to <see cref="string.EnumerateRunes"/>.
    /// This is kind of annoying, but the alternative would be to do that ahead of time and store it in the struct,
    /// which seems silly since the <see cref="Rune"/> representation is the "middle ground":
    /// <ul>
    /// <li><see cref="char"/> - the smallest unit; the one that computers inherently understand</li>
    /// <li><see cref="StringInfo">grapheme cluster</see> - the largest unit; the one that people understand </li>
    /// </ul> 
    /// </remarks>
    public int LengthInRunes => _source!.Length switch
    {
        // 0 shouldn't be possible anyways, so we'll let it get handled by the default case.
        1 => 1,
        2 => Rune.TryGetRuneAt(_source, 1, out _) ? 2 : 1,
        _ => _source.EnumerateRunes().Count()
    };

    private Grapheme(string source)
    {
        _source = source;
    }

    /// <summary>
    /// Creates a <see cref="Grapheme"/> from a single <see cref="char.IsLetter(char)"/>.
    /// </summary>
    /// <param name="c">a <see cref="char.IsLetter(char)"/></param>
    /// <returns>a new <see cref="Grapheme"/></returns>
    /// <exception cref="ArgumentException"><paramref name="c"/> is not <see cref="IsGrapheme(char)"/></exception>
    /// <remarks>
    /// For consistency, all invalid inputs when constructing a <see cref="Grapheme"/> produce <see cref="ArgumentNullException"/>,
    /// even if it might be more accurate to throw an <see cref="ArgumentOutOfRangeException"/>.
    /// </remarks>
    public static Grapheme Parse(char c)
    {
        Preconditions.Require(c, IsGrapheme);
        return new Grapheme(c.ToString());
    }

    public static bool TryParse(char c, out Grapheme grapheme)
    {
        var isGrapheme = IsGrapheme(c);
        grapheme = isGrapheme ? new Grapheme(c.ToString()) : default;
        return isGrapheme;
    }


    public static bool IsGrapheme(char c)    => char.IsLetter(c)    || char.IsNumber(c)    || char.IsSymbol(c);
    public static bool IsGrapheme(Rune rune) => Rune.IsLetter(rune) || Rune.IsNumber(rune) || Rune.IsSymbol(rune);

    /// <summary>
    /// Creates a <see cref="Grapheme"/> from a single <see cref="Rune"/>.
    /// </summary>
    /// <param name="rune">a <see cref="Rune.IsLetter"/></param>
    /// <returns><inheritdoc cref="Parse(char)"/></returns>
    /// <exception cref="ArgumentException"><paramref name="rune"/> is not <see cref="IsGrapheme(Rune)"/></exception>
    /// <remarks><inheritdoc cref="Parse(char)"/></remarks>
    public static Grapheme Parse(Rune rune)
    {
        Preconditions.Require(rune, IsGrapheme);
        return new Grapheme(rune.ToString());
    }

    public static bool TryParse(Rune rune, out Grapheme grapheme)
    {
        var isGrapheme = IsGrapheme(rune);
        grapheme = isGrapheme ? new Grapheme(rune.ToString()) : default;
        return isGrapheme;
    }

    /// <summary>
    /// Creates a <see cref="Grapheme"/> out of a "grapheme cluster", aka "<see cref="StringInfo">text element</see>".
    /// </summary>
    /// <param name="graphemeCluster">a sequence of characterrs that get combined into a single visible glyph</param>
    /// <returns><inheritdoc cref="Parse(char)"/></returns>
    /// <exception cref="ArgumentException"><paramref name="graphemeCluster"/> contains more than one "<see cref="StringInfo">text element</see>"</exception>
    /// <exception cref="ArgumentException"><paramref name="graphemeCluster"/> <see cref="string.IsNullOrEmpty"/></exception>
    /// <remarks><inheritdoc cref="Parse(char)"/></remarks>
    public static Grapheme Parse(string graphemeCluster)
    {
        Preconditions.RejectIf(graphemeCluster, string.IsNullOrEmpty);

        // While eagerly doing string normalization might produce an extra string when validation fails, it:
        //  - Is the most accurate option, since the normalized string is the one we're actually going to keep
        //  - Doesn't cost anything extra in the "happy-path" (a valid grapheme cluster)
        //  - Is overshadowed by the performance cost of throwing an exception during the "sad-path" (an invalid cluster)
        //  - Is possible, unlike normalization-related operations on `ReadOnlySpan<char>`c
        var normalized = graphemeCluster.Normalize();

        Preconditions.Require(normalized.AsSpan(), IsGrapheme);
        return new Grapheme(normalized);
    }

    public static bool IsGrapheme(ReadOnlySpan<char> normalizedString)
    {
        var trimmed = TrimCombiningCharacters(normalizedString);
        
        return trimmed switch
        {
            [] => false,
            [var c] => IsGrapheme(c),
            [var h, var l] when Rune.TryCreate(h, l, out var rune) => IsGrapheme(rune),
            _ => StringInfo.GetNextTextElementLength(trimmed) == trimmed.Length
        };
    }

    private static ReadOnlySpan<char> TrimCombiningCharacters(ReadOnlySpan<char> span)
    {
        const char minCombiningChar = '\u0312';
        const char maxCombiningChar = '\u036F';
        var        lastNonCombining = span.LastIndexOfAnyExceptInRange(minCombiningChar, maxCombiningChar);

        return lastNonCombining switch
        {
            < 0 => span,
            _   => span[..(lastNonCombining+1)],
        };
    }

    private static bool ParseNormalizedString(string normalized, bool allowFailure, out Grapheme result)
    {
        Debug.Assert(normalized.IsNormalized());
        Debug.Assert(normalized.Length > 0);

        // Special case if we actually only have 1 char
        if (normalized is [var c])
        {
            if (allowFailure)
            {
                return TryParse(c, out result);
            }

            result = Parse(c);
            return true;
        }

        // Special case if we actually only have 1 Rune
        var firstRune = Rune.GetRuneAt(normalized, 0);
        if (firstRune.Utf16SequenceLength == normalized.Length)
        {
            if (allowFailure)
            {
                return TryParse(firstRune, out result);
            }

            result = Parse(firstRune);
            return true;
        }

        // Make sure that the string contains exactly 1 grapheme cluster
        var clusterLength = StringInfo.GetNextTextElementLength(normalized);
        if (clusterLength != normalized.Length)
        {
            if (allowFailure)
            {
                result = default;
                return false;
            }

            throw new ArgumentException(
                $"You must provide a string containing EXACTLY 1 grapheme cluster (this input's first cluster ends at index {clusterLength})",
                nameof(normalized)
            );
        }

        result = new Grapheme(normalized);
        return true;
    }

    /// <summary>
    /// Some fancy-schmancy nonsense I stole from <see cref="M:System.Collections.Immutable.ImmutableArray`1.ThrowNullRefIfNotInitialized"/>.
    /// </summary>
    [MemberNotNull(nameof(_source))]
    private void ThrowNullRefIfNotInitialized()
    {
        _ = _source!.Length;
    }
    
    static Grapheme IParsable<Grapheme>.Parse(string s, IFormatProvider? provider) => Parse(s);

    [MustUseReturnValue]
    public static bool TryParse(string? s, out Grapheme result)
    {
        if (s == null)
        {
            result = default;
            return false;
        }

        var normalized = s.Normalize();
        return ParseNormalizedString(normalized, true, out result);
    }

    static bool IParsable<Grapheme>.TryParse(string? s, IFormatProvider? provider, out Grapheme result) =>
        TryParse(s, out result);

    public override string ToString() => _source ?? "";
}
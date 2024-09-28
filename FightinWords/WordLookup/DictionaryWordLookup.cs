using System.Collections.Immutable;

namespace FightinWords.WordLookup;

/// <summary>
/// A simple, in-memory implementation of <see cref="IWordLookup"/> that's backed by a plain-old <see cref="ImmutableDictionary{TKey,TValue}"/>.
/// </summary>
/// <param name="dictionaryDictionary"></param>
public sealed class DictionaryWordLookup(
    ImmutableDictionary<Language, ImmutableDictionary<string, ImmutableList<WordDefinition>>> dictionaryDictionary)
    : IWordLookup
{
    public static DictionaryWordLookup Empty { get; } =
        new(ImmutableDictionary<Language, ImmutableDictionary<string, ImmutableList<WordDefinition>>>.Empty);

    private ImmutableDictionary<string, ImmutableList<WordDefinition>> GetWords(Language language)
    {
        if (dictionaryDictionary.TryGetValue(language, out var dictionary))
        {
            return dictionary;
        }

        throw new ArgumentException($"No sprechen {language}!");
    }

    public DictionaryWordLookup WithDefinition(
        WordDefinition definition
    )
    {
        var updated = AddOrUpdate(
            dictionaryDictionary,
            definition.Language,
            ImmutableDictionary<string, ImmutableList<WordDefinition>>.Empty,
            dic => AddOrUpdate(
                dic,
                definition.Word,
                ImmutableList<WordDefinition>.Empty,
                defs => defs.Add(definition)
            )
        );

        return new DictionaryWordLookup(updated);
    }

    /// <summary>
    /// Applies <paramref name="update"/> to the <see cref="ImmutableDictionary.GetValueOrDefault{TKey,TValue}(System.Collections.Immutable.IImmutableDictionary{TKey,TValue},TKey)"/>
    /// for <paramref name="key"/>, and sets the value to the result.
    /// </summary>
    /// <param name="dictionary">a dictionary that may or may not contain <paramref name="key"/></param>
    /// <param name="key">the <typeparamref name="K"/> that you want to modify</param>
    /// <param name="defaultValue">the <typeparamref name="V"/> used if <paramref name="key"/> isn't present</param>
    /// <param name="update">the function that modifies the existing <typeparamref name="V"/> or <paramref name="defaultValue"/></param>
    /// <returns>a new <see cref="ImmutableDictionary{TKey,TValue}"/> with an updated value for <paramref name="key"/></returns>
    private static ImmutableDictionary<K, V> AddOrUpdate<K, V>(
        ImmutableDictionary<K, V> dictionary,
        K                         key,
        V                         defaultValue,
        Func<V, V>                update
    ) where K : notnull
    {
        var oldValue = dictionary.GetValueOrDefault(key, defaultValue);
        var newValue = update(oldValue);
        return dictionary.SetItem(key, newValue);
    }

    public Task<IEnumerable<WordDefinition>> GetDefinitionsAsync(string word, Language language)
    {
        var dic = GetWords(language);
        return Task.FromResult(dic.GetValueOrDefault(word, ImmutableList<WordDefinition>.Empty).AsEnumerable());
    }
}
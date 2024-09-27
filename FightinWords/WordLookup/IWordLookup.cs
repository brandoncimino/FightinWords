namespace FightinWords.WordLookup;

/// <summary>
/// <b>Level 1️</b> <i>(bare minimum)</i><b>:</b>
/// <ul>
/// <li>Is this word legit?</li>
/// </ul>
/// <b>Level 2️:</b>
/// <ul>
/// <li>Is this word legit?</li>
/// <li>Any ol' definition</li>
/// <li>A link to the source of that definition</li>
/// </ul>
/// <b>Level 3️</b> <i>(the dream)</i><b>:</b>
/// <ul>
/// <li>Is this word legit?</li>
/// <li>ALL the definitions</li>
/// <li>Conjugation information, such as:
///     <ul>
///     <li>Is this a lemma?</li>
///     <li>If not, what <i>is</i> its lemma?<br/><i>(📎 Note: available from Wiktionary categories)</i></li>
///     </ul>
/// </li>
/// </ul>
/// </summary>
public interface IWordLookup
{
    /// <summary>
    /// Looks up a word.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <param name="language">The language to which the resulting <see cref="WordDefinition"/>s apply.
    /// <br/>
    /// Not that this does <b>not</b> directly influence the written language of the resulting <see cref="WordDefinition.Definition"/>s.</param>
    /// <returns>The found <see cref="WordDefinition"/>s with a matching <see cref="WordDefinition.Language"/></returns>
    public Task<IEnumerable<WordDefinition>> GetDefinitionsAsync(string word, Language language);

    public IEnumerable<WordDefinition> GetDefinitions(string word, Language language) =>
        GetDefinitionsAsync(word, language).Result;
}
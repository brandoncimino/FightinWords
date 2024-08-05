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
    public Task<bool> IsWordAsync(string            word, Language language);
    bool IsWord(string word, Language language) => IsWordAsync(word, language).Result;
    
    public Task<IEnumerable<WordDefinition>> GetDefinitionsAsync(string word, Language language);

    public IEnumerable<WordDefinition> GetDefinitions(string word, Language language) =>
        GetDefinitionsAsync(word, language).Result;
    
    public string GetRandomWord(int length, Language language, Random generator);
}
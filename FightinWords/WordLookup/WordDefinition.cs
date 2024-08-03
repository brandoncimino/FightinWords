namespace FightinWords.WordLookup;

/// <summary>
/// The standardized representation of a word regardless of the <see cref="IWordLookup"/> we used <i>(e.g. <see cref="Wiktionary.WiktionaryClient"/>, etc.)</i>
/// </summary>
/// <param name="Word"></param>
/// <param name="Language"></param>
/// <param name="PartOfSpeech"></param>
/// <param name="Definition"></param>
public readonly record struct WordDefinition(
    string        Word,
    Language      Language,
    PartOfSpeech? PartOfSpeech,
    string        Definition
);
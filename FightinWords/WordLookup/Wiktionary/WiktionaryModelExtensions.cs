using System.Collections.Immutable;

namespace FightinWords.WordLookup.Wiktionary;

public static class WiktionaryModelExtensions
{
    public static ImmutableList<WiktionaryModel.UsageDescription> GetDefinitionsForLanguage(this WiktionaryModel.DefinitionsResponse response, Language language)
    {
        return response.Usages.GetValueOrDefault(language.IsoLanguageCode(), ImmutableList<WiktionaryModel.UsageDescription>.Empty);
    }

    public static PartOfSpeech? MaybeGetPartOfSpeech(this WiktionaryModel.UsageDescription usageDescription)
    {
        return Enum.TryParse(usageDescription.PartOfSpeech, out PartOfSpeech partOfSpeech) ? partOfSpeech : null;
    }
}
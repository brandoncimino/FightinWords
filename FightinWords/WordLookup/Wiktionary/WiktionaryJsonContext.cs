using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace FightinWords.WordLookup.Wiktionary;

[JsonSerializable(typeof(ImmutableDictionary<string, ImmutableList<WiktionaryModel.UsageDescription>>), TypeInfoPropertyName = "Mutable_DefinitionsResponse")]
[JsonSerializable(typeof(ImmutableDictionary<string, string>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class WiktionaryJsonContext : JsonSerializerContext;
namespace FightinWords.Data;

public readonly record struct CharacterReference(
    string                             Html,
    string                             DisplayedCharacter,
    CharacterReference.ReferenceFlavor Flavor)
{
    public enum ReferenceFlavor
    {
        Named,
        Decimal,
        Hexadecimal
    }

    public static readonly CharacterReference Ampersand        = new("&amp;", "&", ReferenceFlavor.Named);
    public static readonly CharacterReference LessThan         = new("&lt;", "<", ReferenceFlavor.Named);
    public static readonly CharacterReference GreaterThan      = new("&gt;", ">", ReferenceFlavor.Named);
    public static readonly CharacterReference Quote            = new("&quot;", "\"", ReferenceFlavor.Named);
    public static readonly CharacterReference Apostrophe       = new("&apos;", "'", ReferenceFlavor.Named);
    public static readonly CharacterReference NonBreakingSpace = new("&nbsp;", " ", ReferenceFlavor.Named);
    public static readonly CharacterReference EnDash           = new("&ndash;", "–", ReferenceFlavor.Named);
    public static readonly CharacterReference EmDash           = new("&mdash;", "—", ReferenceFlavor.Named);
    public static readonly CharacterReference Copyright        = new("&copy;", "©", ReferenceFlavor.Named);
    public static readonly CharacterReference Registered       = new("&reg;", "®", ReferenceFlavor.Named);
    public static readonly CharacterReference Trademark        = new("&trade;", "™", ReferenceFlavor.Named);
    public static readonly CharacterReference Approximately    = new("&asymp;", "≈", ReferenceFlavor.Named);
    public static readonly CharacterReference NotEquals        = new("&ne;", "≠", ReferenceFlavor.Named);
    public static readonly CharacterReference BritishPound     = new("&pound;", "£", ReferenceFlavor.Named);
    public static readonly CharacterReference Euro             = new("&euro;", "€", ReferenceFlavor.Named);
    public static readonly CharacterReference Degrees          = new("&deg;", "°", ReferenceFlavor.Named);

    public static readonly CharacterReference LowerA_Hex         = new("&#x0061;", "a", ReferenceFlavor.Hexadecimal);
    public static readonly CharacterReference LowerA_Hex_Trimmed = new("&#x61;", "a", ReferenceFlavor.Hexadecimal);
    public static readonly CharacterReference LowerA_Dec         = new("&#97;", "a", ReferenceFlavor.Decimal);
}
using System.Text;

namespace FightinWords;

/// <summary>
/// TODO: stop the madness 
/// </summary>
public static class SingleRuneData
{
    public static readonly Rune EmojiModifier_Fitzpatrick_1 = @"🏻".EnumerateRunes().Single();
    public static readonly Rune EmojiModifier_Fitzpatrick_6 = @"🏿".EnumerateRunes().Single();
    
    #region Emoji

    public readonly record struct EmojiRune(Rune Rune, bool VariationSelectable);

    public static readonly EmojiRune WhiteFlag          = new(new Rune('\ud83c', '\udff3'), true);
    public static readonly EmojiRune BlackFlag          = new(new Rune('\ud83c', '\udff4'), false);
    public static readonly EmojiRune SkullAndCrossbones = new(new Rune('\u2620'), true);
    public static readonly EmojiRune Rainbow            = new(new Rune('\ud83c', '\udf08'), false);

    #endregion
}
using System.Text;

namespace FightinWords.Console.LetterPools;

/// <summary>
/// Determines what case letters should be displayed with in the UI.
/// </summary>
public enum LetterPoolCasing
{
    /// <summary>
    /// However we got them from <see cref="ILetterPool.LockIn"/>.
    /// </summary>
    AsIs,

    /// <summary>
    /// Converted <see cref="Rune.ToUpper"/>.
    /// </summary>
    Lower,

    /// <summary>
    /// Converted <see cref="Rune.ToLower"/>.
    /// </summary>
    Upper
}
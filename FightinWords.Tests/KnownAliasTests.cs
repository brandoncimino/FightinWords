using FightinWords.Console;

namespace FightinWords;

public class KnownAliasTests
{
    [Test]
    public void KnownAlias_RejectsDuplicateAliases()
    {
        Assert.Throws<ArgumentException>(() => new AliasMatcher.KnownAlias("yolo", ["a", "A"]));
    }

    [Test]
    public void KnownAlias_RejectsCanonicalDuplicatedByAlias()
    {
        Assert.Throws<ArgumentException>(() => new AliasMatcher.KnownAlias("yolo", ["YOLO"]));
    }
}
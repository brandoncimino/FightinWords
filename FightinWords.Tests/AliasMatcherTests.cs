using System.Collections.Immutable;
using FightinWords.Console;
using FluentAssertions;

namespace FightinWords;

public class AliasMatcherTests
{
    public sealed record Scenario(
        ImmutableArray<AliasMatcher.KnownAlias> KnownAliases,
        string                                  Input,
        AliasMatcher.KnownAlias                 ExpectedResult
    );


    [Test]
    public void KnownAlias_CheckMatchiness()
    {
        var knownAlias = new AliasMatcher.KnownAlias("yolo", ["yoloswaggins", "yoloswag"]);

        assert_matchiness(knownAlias, "y",        AliasMatcher.Matchiness.Partial);
        assert_matchiness(knownAlias, "yolo",     AliasMatcher.Matchiness.Exact);
        assert_matchiness(knownAlias, "yolos",    AliasMatcher.Matchiness.Partial);
        assert_matchiness(knownAlias, "yoloswag", AliasMatcher.Matchiness.Exact);
        assert_matchiness(knownAlias, "swag",     null);
    }

    private static void assert_matchiness(
        AliasMatcher.KnownAlias  knownAlias,
        string                   input,
        AliasMatcher.Matchiness? expectedResult
    )
    {
        knownAlias.CheckMatchiness(input)
                  .Should()
                  .Be(expectedResult);

        knownAlias.CheckMatchiness(input.ToUpperInvariant())
                  .Should()
                  .Be(expectedResult);

        knownAlias.CheckMatchiness(input.ToLowerInvariant())
                  .Should()
                  .Be(expectedResult);
    }

    [Test]
    public void AliasMatcher_PrefersExact()
    {
        var yoloAlias         = new AliasMatcher.KnownAlias("yolo",         []);
        var yoloSwagginsAlias = new AliasMatcher.KnownAlias("yoloswaggins", []);

        var aliasMatcher = new AliasMatcher([
            yoloAlias,
            yoloSwagginsAlias
        ]);

        aliasMatcher.FindMatch("y")
                    .IsT1
                    .Should()
                    .BeTrue();

        aliasMatcher.FindMatch("yolo")
                    .Value
                    .Should()
                    .Be(yoloAlias);

        aliasMatcher.FindMatch("yoloswag")
                    .Value
                    .Should()
                    .Be(yoloSwagginsAlias);
    }

    [Test]
    public void AliasMatcher_RejectsOverlappingCanonicalNames()
    {
        Assert.That(() => new AliasMatcher([
                new AliasMatcher.KnownAlias("yolo", ["one"]),
                new AliasMatcher.KnownAlias("YOLO", ["two"])
            ]
        ), Throws.Exception);
    }


    [Test]
    public void AliasMatcher_RejectsOverlappingAliases()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new AliasMatcher([
                new AliasMatcher.KnownAlias("yolo", ["one"]),
                new AliasMatcher.KnownAlias("swag", ["ONE"])
            ]);
        });
    }

    [Test]
    public void AliasMatcher_RejectsAliasOverlappingCanonical()
    {
        Assert.Throws<ArgumentException>(() => new AliasMatcher([
            new AliasMatcher.KnownAlias("yolo", ["YOLO"])
        ]));
    }

    [Test]
    public void AliasMatcher_RejectsCanonicalOverlapingAnotherAlias()
    {
        Assert.Throws<ArgumentException>(() => new AliasMatcher([
            new AliasMatcher.KnownAlias("yolo", []),
            new AliasMatcher.KnownAlias("swag", ["YOLO"])
        ]));
    }
}
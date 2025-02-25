using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

        TestHelpers.AssertEquals(
            aliasMatcher.FindMatch("y"),
            new AliasMatcher.Failure("y", [yoloAlias, yoloSwagginsAlias])
        );

        TestHelpers.AssertEquals(
            aliasMatcher.FindMatch("yolo"),
            yoloAlias
        );

        TestHelpers.AssertEquals(
            aliasMatcher.FindMatch("yoloswag"),
            yoloSwagginsAlias
        );

        TestHelpers.AssertEquals(
            aliasMatcher.FindMatch("swag"),
            new AliasMatcher.Failure("swag", []));
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

    #region Gnarly Enum

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EnumWithDuplicateValues
    {
        A,
        A_Duplicate = A,

        [AliasMatcher.Alias("B_Alias")] B,

        C,
    }

    private static readonly AliasMatcher.KnownAlias EnumWithDuplicateValues_KnownAlias_A = new(
        EnumWithDuplicateValues.A.ToString(),
        [nameof(EnumWithDuplicateValues.A_Duplicate)]
    );

    private static readonly AliasMatcher.KnownAlias EnumWithDuplicateValues_KnownAlias_B = new(
        EnumWithDuplicateValues.B.ToString(),
        ["B_Alias"]
    );

    private static readonly AliasMatcher.KnownAlias EnumWithDuplicateValues_KnownAlias_C =
        new(EnumWithDuplicateValues.C.ToString(), []);

    [Test]
    public void AliasMatcher_EnumWithDuplicateValues()
    {
        var matcher = AliasMatcher.ForEnum<EnumWithDuplicateValues>();

        TestHelpers.AssertEquals(
            matcher.KnownAliases.SequenceEqual([
                EnumWithDuplicateValues_KnownAlias_A,
                EnumWithDuplicateValues_KnownAlias_B,
                EnumWithDuplicateValues_KnownAlias_C
            ]),
            true
        );
    }

    #endregion

    #region Invalid Enum

    private enum InvalidEnum_DuplicatesAliasesOnSameMember
    {
        [AliasMatcher.Alias("Alias")]
        [AliasMatcher.Alias("alias")]
        A
    }

    private enum InvalidEnum_DuplicateAliasesAcrossMembers
    {
        [AliasMatcher.Alias("alias")] A,

        [AliasMatcher.Alias("ALIAS")] B
    }

    private enum InvalidEnum_AliasDuplicatesCanonical
    {
        [AliasMatcher.Alias("a")] A
    }

    private enum InvalidEnum_EquivalentCanonicals
    {
        A,
        a
    }

    private enum InvalidEnum_DuplicatesAcrossMembers
    {
        [AliasMatcher.Alias("b")] A,
        B
    }

    [Test]
    public void InvalidEnumTest(
        [Values(
            typeof(InvalidEnum_EquivalentCanonicals),
            typeof(InvalidEnum_DuplicatesAcrossMembers),
            typeof(InvalidEnum_AliasDuplicatesCanonical),
            typeof(InvalidEnum_DuplicatesAliasesOnSameMember),
            typeof(InvalidEnum_DuplicateAliasesAcrossMembers)
        )]
        Type enumType
    )
    {
        var method         = typeof(AliasMatcher).GetMethod(nameof(AliasMatcher.ForEnum))!;
        var methodWithType = method.MakeGenericMethod(enumType);
        try
        {
            methodWithType.Invoke(null, null);
        }
        catch (Exception e)
        {
            Assert.That(e, Has.InnerException.InstanceOf<ArgumentException>());
        }
    }

    #endregion
}
using System.Collections.Immutable;
using FluentAssertions;

namespace FightinWords;

public class ValueArrayTests
{
    [Test]
    public void CannotUseWhenDefault()
    {
        ValueArray<string> array = default;
        Assert.That(() => array.Count(), Throws.InvalidOperationException);
    }

    [Test]
    public void CannotContainDefaultArray()
    {
        var array = new ValueArray<string>(default);
        Assert.That(() => array.Count(), Throws.InvalidOperationException);
    }

    [Test]
    [TestCase()]
    [TestCase("a")]
    [TestCase("a", "b")]
    public void UsesValueEquality(params string[] elements)
    {
        var array_1 = elements.Select(it => new string(it)).ToImmutableArray();
        var array_2 = elements.Select(it => new string(it)).ToImmutableArray();

        if (elements.Length > 0)
        {
            array_1.Equals(array_2).Should().BeFalse();
            array_1.SequenceEqual(array_2).Should().BeTrue();
        }

        var vArray_1 = new ValueArray<string>(array_1);
        var vArray_2 = new ValueArray<string>(array_2);

        vArray_1.Equals(vArray_2).Should().BeTrue();
    }

    [Test]
    public void DefaultEqualsEmpty()
    {
        var empty = ValueArray<string>.Empty;
        var def   = default(ValueArray<string>);

        TestHelpers.AssertEquals(empty == def,             true);
        TestHelpers.AssertEquals(empty.Equals(def),        true);
        TestHelpers.AssertEquals(empty.SequenceEqual(def), true);

        TestHelpers.AssertEquals(def == empty,             true);
        TestHelpers.AssertEquals(def.Equals(empty),        true);
        TestHelpers.AssertEquals(def.SequenceEqual(empty), true);
    }
}
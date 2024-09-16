using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FightinWords.Submissions;

namespace FightinWords;

public class HistoryTests
{
    [Test]
    public void History_Record([Values(1,2,3)] int capacity)
    {
        Debug.Assert(capacity > 0);

        var history = new History<string>(capacity);

        var alphabet = Enumerable.Range('a', 'z' - 'a' + 1)
                                 .Select(it => (char)it)
                                 .Select(it => it.ToString());

        foreach (var letter in alphabet)
        {
            RecordAndAssert(history, letter, $"+ {letter}");
        }
    }

    private static void RecordAndAssert_WhenMoreCapacityAvailable<T>(History<T> history, T next, string label)
    {
        Assert.That(history.Count, Is.LessThan(history.Capacity));
        var expectedNextContents = ToArrayByEnumerator(history).Prepend(next).ToImmutableArray();

        TestHelpers.AssertEquals(history.Record(next, out var evictedEntry), false);
        TestHelpers.AssertEquals(evictedEntry, default(T));
        TestHelpers.AssertEquals(history.OffsetInStorage,            0);

        Assert_Content(history, expectedNextContents, 0, label);
    }

    private static void RecordAndAssert_WhenAtCapacity<T>(History<T> history, T next, string label)
    {
        TestHelpers.AssertEquals(history.Count, history.Capacity);
        
        var expectedOffsetInStorage = (history.OffsetInStorage + 1) % history.Capacity;
        var expectedNextContents    = ToArrayByEnumerator(history).Prepend(next).SkipLast(1).ToImmutableArray();
        var expectedToBeEvicted     = history.Oldest;
        
        Assert.Multiple(() =>
        {
            TestHelpers.AssertEquals(history.Record(next, out var evictedEntry), true);
            TestHelpers.AssertEquals(evictedEntry,                               expectedToBeEvicted);
            Assert_Content(history, expectedNextContents, expectedOffsetInStorage, label);
        });
    }

    private static void RecordAndAssert<T>(History<T> history, T next, string label)
    {
        if (history.Capacity > history.Count)
        {
            RecordAndAssert_WhenMoreCapacityAvailable(history, next, label);
        }
        else
        {
            RecordAndAssert_WhenAtCapacity(history, next, label);
        }
    }

    [Test]
    public void Newest_Oldest()
    {
        var history = new History<string>(3);
        history.Record("grandparent");
        TestHelpers.AssertEquals(history.OffsetInStorage, 0);
        history.Record("parent");
        TestHelpers.AssertEquals(history.OffsetInStorage, 0);
        history.Record("child");
        TestHelpers.AssertEquals(history.OffsetInStorage, 0);
        
        Assert.Multiple(() =>
        {
            TestHelpers.AssertEquals(history.Oldest, "grandparent");
            TestHelpers.AssertEquals(history.Newest, "child");
        });
    }
    
    [Test]
    public void History_Record_Manual()
    {
        var history = new History<string>(3);
        Assert_Content(history, [], 0, "initial");

        history.Record("a");
        Assert_Content(history, ["a"], 0, "+ a");

        history.Record("b");
        Assert_Content(history, ["b", "a"], 0, "+ b");

        history.Record("c");
        Assert_Content(history, ["c", "b", "a"], 0, "+ c");

        history.Record("d");
        Assert_Content(history, ["d", "c", "b"], 1, "+ d");

        history.Record("e");
        Assert_Content(history, ["e", "d", "c"], 2, "+ e");

        history.Record("f");
        Assert_Content(history, ["f", "e", "d"], 0, "+ f");

        history.Record("g");
        Assert_Content(history, ["g", "f", "e"], 1, "+ g");

        history.Record("h");
        Assert_Content(history, ["h", "g", "f"], 2, "+ h");
    }

    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
    private static ImmutableArray<T> ToArrayByIndices<T>(History<T> history)
    {
        var builder = ImmutableArray.CreateBuilder<T>(history.Count);
        for (int i = 0; i < history.Count; i++)
        {
            builder.Add(history[i]);
        }

        return builder.DrainToImmutable();
    }

    private static ImmutableArray<T> ToArrayByEnumerator<T>(History<T> history)
    {
        var builder = ImmutableArray.CreateBuilder<T>(history.Count);

        var erator = history.GetEnumerator();

        while (erator.MoveNext())
        {
            builder.Add(erator.Current);
        }

        return builder.DrainToImmutable();
    }

    [StackTraceHidden]
    private static void Assert_Content<T>(
        History<T> actual,
        IList<T>    expected,
        int expectedOffsetFromStorage,
        string label
    )
    {
        Assert.Multiple([StackTraceHidden]() =>
        {
            TestHelpers.AssertEquals(actual.Count, expected.Count, label);
            TestHelpers.AssertEquals(actual.OffsetInStorage, expectedOffsetFromStorage, label);

            TestHelpers.AssertEquals(ToArrayByIndices(actual),    expected, label);
            TestHelpers.AssertEquals(ToArrayByEnumerator(actual), expected, label);

            if (expected.Count > 0)
            {
                TestHelpers.AssertEquals(actual.Newest, expected[0], label);
                TestHelpers.AssertEquals(actual.Oldest, expected[^1], label);
            }

            Assert_CannotGetIndicesInStorageButNotHistory(actual);
        });
    }

    private static void Assert_CannotGetIndicesInStorageButNotHistory<T>(History<T> actual)
    {
        foreach (var i in Enumerable.Range(actual.Count, actual.Capacity - actual.Count))
        {
            Assert.That(() => actual[i], Throws.TypeOf<IndexOutOfRangeException>());
        }
                  
    }

    [Test]
    public void OneSeater()
    {
        var history = new History<string>(1);

        history.Record("first");
        history.Record("second");
        
        Assert_Content(history, ["second"], 0, "?");
    }

    [Test]
    public void TwoSeater()
    {
        var history = new History<string>(2);

        history.Record("grandparent");
        history.Record("parent");
        history.Record("child");
        
        Assert_Content(history, ["child", "parent"], 1, "after child kicks out grandparent");
    }

    [Test]
    public void ThreeSeater()
    {
        var history = new History<string>(3);

        history.Record("great-grandparent");
        history.Record("grandparent");
        history.Record("parent");
        
        RecordAndAssert_WhenAtCapacity(history, "child", "child kicking out great-grandparent");
        
        Assert_Content(history, ["child","parent","grandparent"], 1, "after child kicks out great-grandparent");

        RecordAndAssert_WhenAtCapacity(history, "future-baby", "future-baby kicking out grandparent");
        Assert_Content(history, ["future-baby", "child", "parent"], 2, "after future-baby kicks out grandparent");
    }
    
    [TestCase(1, 0, 0)]
    [TestCase(1, 1, 0)]
    [TestCase(1, 2, 0)]
    [TestCase(1, 3, 0)]
    
    [TestCase(3, -6, 0)]
    [TestCase(3, -5, 1)]
    [TestCase(3, -4, 2)]
    [TestCase(3, -3, 0)]
    [TestCase(3, -2, 1)]
    [TestCase(3, -1, 2)]
    [TestCase(3, 0, 0)]
    [TestCase(3, 1, 1)]
    [TestCase(3, 2, 2)]
    [TestCase(3, 3, 0)]
    [TestCase(3, 4, 1)]
    [TestCase(3, 5, 2)]
    [TestCase(3, 6, 0)]
    public void LoopIndex(int length, int index, int expectedLoopedIndex)
    {
        TestHelpers.AssertEquals(History<int>.LoopIndex(index, length), expectedLoopedIndex);
    }
}
using FightinWords.Console.LetterPools;

namespace FightinWords.LetterPools;

public class LetterPoolConfigScreenerTests
{
    [Test]
    public void ScreenInput_SizedPool(
        [Values(
            0,
            LetterPoolConfigScreener.MinimumPoolSize - 1,
            LetterPoolConfigScreener.MinimumPoolSize,
            LetterPoolConfigScreener.MinimumPoolSize + 1,
            LetterPoolConfigScreener.MinimumPoolSize * 10
        )]
        int poolSize
    )
    {
        var rawSubmission = Word.Parse(poolSize.ToString());
        var screener      = new LetterPoolConfigScreener();

        var actualResult = screener.ScreenInput(rawSubmission);

        if (poolSize < LetterPoolConfigScreener.MinimumPoolSize)
        {
            TestHelpers.AssertEquals(actualResult.IsT1, true);
        }
        else
        {
            TestHelpers.AssertEquals(actualResult.IsT0,          true);
            TestHelpers.AssertEquals(actualResult.AsT0.PoolSize, poolSize);
        }
    }

    public record ExplicitPoolData(
        string UserInput,
        string ExpectedPool
    );

    [TestCase("AaaAbBBc",    true)]
    [TestCase("onetwothree", true)]
    public void ScreenInput_ExplicitPool(
        string userInputString,
        bool   expectedAsPool
    )
    {
        var rawSubmission = Word.Parse(userInputString);

        var actualPool = new LetterPoolConfigScreener().ScreenInput(rawSubmission)
                                                       .GetT0()
                                                       .LockIn(TestHelpers.AlwaysThrowRandom);

        TestHelpers.AssertEquals(actualPool, rawSubmission);
    }

    [Test]
    public void ScreenInput_Invalid(
        [Values(
            "0",
            "",
            "a",
            "a1"
        )]
        string userInputString
    )
    {
        var rawSubmission = Word.Parse(userInputString, StringSplitOptions.RemoveEmptyEntries);

        var actualPool = new LetterPoolConfigScreener().ScreenInput(rawSubmission);
        TestHelpers.AssertEquals(actualPool.IsT1, true);
    }
}
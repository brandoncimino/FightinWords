using FightinWords;
using FightinWords.Console;
using FightinWords.WordLookup;

var rng = new Random();
// var pool = LetterPoolInputProcessor.ReadLine(5, rng, console);
var pool = "yoloswaggins";

var gamePlan = new GamePlan()
{
    Language       = Language.English,
    ProgenitorPool = Word.Parse(pool),
    TimeLimit      = TimeSpan.FromMinutes(3)
};

var manager = GameManager.StartGame(gamePlan, rng);
// manager.SubmitWord("lag");
while (true)
{
    manager.GameLoop();
}
// var wiktionary = new WiktionaryClient();
// var sin        = wiktionary.GetDefinitionsAsync("sin").Result;
// Console.WriteLine(sin);

// var puppyDef = wiktionary.GetDefinitionsAsync("puppy").Result;
// Console.WriteLine(puppyDef);

// while (true)
// {
//     console.Clear();
//     
//     
// }
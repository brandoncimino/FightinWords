using System.Text;
using FightinWords.Console;
using Spectre.Console;

Console.WriteLine("before app stuff");

Console.OutputEncoding = Encoding.UTF8;

var director = new GameDirector()
{
    Console = AnsiConsole.Create(new AnsiConsoleSettings()
    {
        Ansi = AnsiSupport.Yes
    }),
    Theme = new SpectreFactory.Theme()
};

FinalResults? finalResults;
while ((finalResults = director.GameLoop()) is null)
{
    continue;
}

AnsiConsole.MarkupLine($"Final Score: [green]{finalResults.TotalScore}[/]");
return 0;
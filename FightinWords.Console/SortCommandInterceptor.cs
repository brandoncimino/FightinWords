namespace FightinWords.Console;

public class SortCommandInterceptor
{
    private readonly AliasMatcher _aliasMatcher = AliasMatcher.ForEnum<GameReferee.SortCommand>();
}
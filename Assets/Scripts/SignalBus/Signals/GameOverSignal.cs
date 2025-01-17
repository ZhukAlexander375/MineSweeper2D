
public struct GameOverSignal 
{
    public GameMode CurrentGameMode;
    public bool IsGameOver;
    public bool IsGameWin;

    public GameOverSignal(GameMode currentGameMode, bool gameOver, bool gameWin)
    {
        CurrentGameMode = currentGameMode;
        IsGameOver = gameOver;
        IsGameWin = gameWin;
    }
}


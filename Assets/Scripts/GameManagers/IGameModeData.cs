
public interface IGameModeData
{
    GameMode Mode { get; }
    bool IsGameStarted { get; set; }  

    void InitializeNewGame();
    void InitializeFromSave(IGameModeData savedData);
    void IncrementOpenedCells();
    void IncrementCompletedSectors();
    void IncrementPlacedFlags(bool isPlacingFlag);
    void IncrementExplodedMines();
    void IncrementRewardLevel();
    void IncrementSectorBuyoutIndex();
    int GetOpenedCells();
    int GetPlacedFlags();
    int GetCompletedSectors();
    int GetExplodedMines();
    int GetRewardLevel();
    int GetSectorBuyoutLevel();
}

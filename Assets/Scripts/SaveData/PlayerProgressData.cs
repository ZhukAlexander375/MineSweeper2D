
using PixelAnticheat.SecuredTypes;

[System.Serializable]

public class PlayerProgressData
{
    public SecuredInt TotalReward;
    public GameMode LastSessionGameMode;
    public GameMode LastClassicSessionMode;
    public bool IsFirstTimePlayed;

    //public int TotalPlacedFlags;
    //public int TotalOpenedCells;
    //public int TotalCompletedSectors;

    public PlayerProgressData(PlayerProgressData playerProgress = null)
    {
        if (playerProgress != null)
        {
            TotalReward = playerProgress.TotalReward;
            LastSessionGameMode = playerProgress.LastSessionGameMode;
            LastClassicSessionMode = playerProgress.LastClassicSessionMode;
            IsFirstTimePlayed = playerProgress.IsFirstTimePlayed;            
        }
    }
}

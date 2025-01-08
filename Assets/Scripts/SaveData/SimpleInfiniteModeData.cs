
using UnityEngine;

[System.Serializable]

public class SimpleInfiniteModeData
{
    public bool IsGameStarted;
    public int OpenedCells;
    public int PlacedFlags;
    public int CompletedSectors;
    public int ExplodedMines;
    public int RewardLevel;
    public int SectorBuyoutCostLevel;
    public float TotalPlayTime;
    public bool IsGameOver;
    public Vector3 LastClickPosition;

    public SimpleInfiniteModeData(SimpleInfiniteStatisticController controller = null)
    {
        if (controller != null)
        {
            IsGameStarted = controller.IsGameStarted;
            OpenedCells = controller.OpenedCells;
            PlacedFlags = controller.PlacedFlags;
            CompletedSectors = controller.CompletedSectors;
            ExplodedMines = controller.ExplodedMines;
            RewardLevel = controller.RewardLevel;
            SectorBuyoutCostLevel = controller.SectorBuyoutCostLevel;
            TotalPlayTime = controller.TotalPlayTime;
            IsGameOver = controller.IsGameOver;
            LastClickPosition = controller.LastClickPosition;
        }
    }
}

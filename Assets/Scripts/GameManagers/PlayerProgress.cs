using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int Award { get; private set; }
    public int PlacedFlags { get; private set; }
    public int OpenedCells { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(ChangePlayersAward);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagsCount);
        SignalBus.Subscribe<CellRevealedSignal>(UpdateCellsCount);
    }

    public void ResetSessionStatistic()
    {
        OpenedCells = 0;
        PlacedFlags = 0;
    }

    private void ChangePlayersAward(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:
                Award += signal.Count;
                break;
        }
        //Debug.Log(StarAward);
    }

    private void UpdateCellsCount(CellRevealedSignal signal)
    {
        OpenedCells += 1;
        //Debug.Log(OpenedCells);
    }

    private void UpdateFlagsCount(FlagPlacingSignal signal)
    {
        bool isPlacingFlag = signal.IsFlagPlaced;

        if (isPlacingFlag)
            PlacedFlags++;
        else
            PlacedFlags--;
                
        PlacedFlags = Mathf.Max(PlacedFlags, 0);        
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(ChangePlayersAward);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagsCount);
        SignalBus.Unsubscribe<CellRevealedSignal>(UpdateCellsCount);
    }
}

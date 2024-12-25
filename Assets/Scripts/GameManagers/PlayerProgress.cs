using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int TotalReward { get; private set; }
    public GameMode LastSessionGameMode { get; private set; }

    //public int TotalPlacedFlags { get; private set; }
    //public int TotalOpenedCells { get; private set; }
    //public int TotalExplodedMines {  get; private set; }


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
        LoadProgress();
        //SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagsCount);
        //SignalBus.Subscribe<CellRevealedSignal>(UpdateCellsCount);

    }

    public void SetLastSessionGameMode(GameMode lastMode)
    {
        LastSessionGameMode = lastMode;
    }

    public void UpdateExplodedMinesCount(int count)
    {
        //ExplodedMines = count;
        //Debug.Log(ExplodedMines);
    }

    public void ResetSessionStatistic()
    {
        //OpenedCells = 0;
        //PlacedFlags = 0;
        //ExplodedMines = 0;
    }

    public void SavePlayerProgress()
    {
        PlayerProgressData playerProgress = new PlayerProgressData
        {
            TotalReward = TotalReward,
            LastSessionGameMode = LastSessionGameMode
        };

        SaveManager.Instance.SavePlayerProgress(playerProgress);
    }

    public bool CheckAwardCount(int value)
    {
        return TotalReward >= value;
    }

    private void ChangePlayersReward(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:
                TotalReward += signal.Count;
                break;
        }
        //Debug.Log(StarAward);
    }

    private void UpdateCellsCount(CellRevealedSignal signal)
    {
        //OpenedCells += 1;
        //Debug.Log(OpenedCells);
    }

    private void UpdateFlagsCount(FlagPlacingSignal signal)
    {
        /* bool isPlacingFlag = signal.IsFlagPlaced;

         if (isPlacingFlag)
             PlacedFlags++;
         else
             PlacedFlags--;

         PlacedFlags = Mathf.Max(PlacedFlags, 0);  */
    }

    private void LoadProgress()
    {
        PlayerProgressData playerProgress = SaveManager.Instance.LoadPlayerProgress();

        if (playerProgress != null)
        {
            TotalReward = playerProgress.TotalReward;
            LastSessionGameMode = playerProgress.LastSessionGameMode;

            SignalBus.Fire<PlayerProgressLoadCompletedSignal>();
        }

        else
        {
            Debug.LogWarning("Failed to load progress, applying default values.");
        }
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<OnGameRewardSignal>(ChangePlayersReward);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(ChangePlayersReward);
        //SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagsCount);
        //SignalBus.Unsubscribe<CellRevealedSignal>(UpdateCellsCount);
    }
}

using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int RewardCount { get; private set; }
    public int PlacedFlags { get; private set; }
    public int OpenedCells { get; private set; }

    /// <summary>
    /// SAVE!!!!
    /// </summary>
    public int ExplodedMines {  get; private set; }


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
        LoadProgress();
    }

    public void UpdateExplodedMinesCount(int count)
    {
        ExplodedMines = count;
        Debug.Log(ExplodedMines);
    }

    public void ResetSessionStatistic()
    {
        OpenedCells = 0;
        PlacedFlags = 0;
        ExplodedMines = 0;
    }

    public void SavePlayerProgress()
    {
        PlayerProgressData playerProgress = new PlayerProgressData
        {
            AwardCount = RewardCount,
            PlacedFlags = PlacedFlags,
            OpenedCells = OpenedCells,
        };

        SaveManager.Instance.SavePlayerProgress(playerProgress);
    }

    public bool CheckAwardCount(int value)
    {
        return RewardCount >= value;
    }

    private void ChangePlayersAward(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:
                RewardCount += signal.Count;
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

    private void LoadProgress()
    {
        PlayerProgressData playerProgress = SaveManager.Instance.LoadPlayerProgress();

        if (playerProgress != null)
        {
            RewardCount = playerProgress.AwardCount;
            PlacedFlags = playerProgress.PlacedFlags;
            OpenedCells = playerProgress.OpenedCells;
        }

        else
        {
            Debug.LogWarning("Failed to load progress, applying default values.");
        }
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(ChangePlayersAward);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagsCount);
        SignalBus.Unsubscribe<CellRevealedSignal>(UpdateCellsCount);
    }
}

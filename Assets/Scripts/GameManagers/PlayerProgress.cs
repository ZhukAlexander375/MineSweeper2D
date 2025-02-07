using PixelAnticheat.Detectors;
using PixelAnticheat.SecuredTypes;
using UnityEngine;
using PixelAnticheat.Models;
using PixelAnticheat;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public SecuredInt TotalReward { get; private set; }
    public GameMode LastSessionGameMode { get; private set; }
    public GameMode LastClassicSessionMode { get; private set; }
    public bool IsFirstTimePlayed { get; private set; }

    private MemoryHackDetector _memoryHackDetector;

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

        MemoryAntiCheat();
        //SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagsCount);
        //SignalBus.Subscribe<CellRevealedSignal>(UpdateCellsCount);

    }

    public void SetLastSessionGameMode(GameMode lastMode)
    {        
        LastSessionGameMode = lastMode;        
    }

    public void SetLastClassicSessionGameMode(GameMode lastClassic)
    {
        LastClassicSessionMode = lastClassic;
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
            LastSessionGameMode = LastSessionGameMode,
            LastClassicSessionMode = LastClassicSessionMode,
            IsFirstTimePlayed = IsFirstTimePlayed
        };


        SaveManager.Instance.SavePlayerProgress(playerProgress);
    }

    public bool CheckAwardCount(int value)
    {
        return TotalReward >= value;
    }

    public void SetFirstTimePlayed()
    {
        if (!IsFirstTimePlayed)
        {
            IsFirstTimePlayed = true;
        }
    }

    private void MemoryAntiCheat()
    {
        AntiCheat.Instance()
                .AddDetector<MemoryHackDetector>(new MemoryHackDetectorConfig());

        _memoryHackDetector = (MemoryHackDetector)AntiCheat.Instance().GetDetector<MemoryHackDetector>();
             
        _memoryHackDetector.OnCheatingDetected.AddListener(OnPlayerProgressHackDetected);
        _memoryHackDetector.StartDetector();
       
        if (_memoryHackDetector != null)
        {
            //Debug.Log("MemoryHackDetector Started");
        }
    }

    private void OnPlayerProgressHackDetected(string messege)
    {        
        TotalReward = 0;
        SignalBus.Fire(new OnGameRewardSignal(0, TotalReward));
        Debug.Log(messege);
    }

    private void ChangePlayersReward(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:                
                TotalReward += signal.Count;                
                break;
        }

        if (_memoryHackDetector.IsRunning())
        {
            Debug.Log("проверка?");
        }

        SavePlayerProgress();
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
            LastClassicSessionMode = playerProgress.LastClassicSessionMode;
            IsFirstTimePlayed = playerProgress.IsFirstTimePlayed;

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

    private void OnApplicationQuit()
    {
        SavePlayerProgress();
    }
}

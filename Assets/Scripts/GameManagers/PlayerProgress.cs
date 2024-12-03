using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int StarAward { get; private set; }
    public int PlacedFlags { get; private set; }


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
        SignalBus.Subscribe<OnGameRewardSignal>(ChangePlayerProgress);
        SignalBus.Subscribe<FlagPlacingSignal>(UpdateFlagsCount);
    }

    private void ChangePlayerProgress(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:
                StarAward += signal.Count;
                break;
        }
        //Debug.Log(StarAward);
    }

    public void UpdateFlagsCount(FlagPlacingSignal signal)
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
        SignalBus.Unsubscribe<OnGameRewardSignal>(ChangePlayerProgress);
        SignalBus.Unsubscribe<FlagPlacingSignal>(UpdateFlagsCount);
    }
}

using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }
    public int StarAward {  get; private set; }


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
    }

    private void ChangePlayerProgress(OnGameRewardSignal signal)
    {
        switch (signal.RewardId)
        {
            case 0:
                StarAward += signal.Count;
                break;
        }
        Debug.Log(StarAward);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnGameRewardSignal>(ChangePlayerProgress);
    }
}

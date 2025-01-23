using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusScreen : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text _awardText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _getBonusText;

    [Header("Buttons")]
    [SerializeField] private Button _getBonusButton;

    private float _timerUpdateInterval = 1f;
    private float _timerUpdateTime;

    private void Awake()
    {        
        SignalBus.Subscribe<OnGameRewardSignal>(OnBonusCollected);
    }
    private void Start()
    {
        _getBonusButton.onClick.AddListener(GetBonus);
    }

    private void Update()
    {
        if (RewardManager.Instance == null) return;
        {
            if (Time.time >= _timerUpdateTime)
            {
                _timerUpdateTime = Time.time + _timerUpdateInterval;
                UpdateTimer();
            }
        }

        CheckForRewardUpdates();
    }

    private void UpdateTimer()
    {
        if (RewardManager.Instance == null) return;

        TimeSpan timeUntilNextReward = RewardManager.Instance.GetTimeUntilNextReward();
        _timerText.text = FormatTimeSpan(timeUntilNextReward);
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        return $"{hours} h. {minutes} min. {seconds} sec.";
    }

    private void CheckForRewardUpdates()
    {

    }

    private void GetBonus()
    {
        RewardManager.Instance.CollectBonus();

        OnBonusCollected(new OnGameRewardSignal());        
    }

    private void OnBonusCollected(OnGameRewardSignal signal)
    {        
        UpdateButtonInteractable();
        UpdateTexts();
    }

    private void OnBonusCredited(OnBonusGrantSignal signal)
    {
        UpdateButtonInteractable();
        UpdateTexts();
    }

    private void UpdateButtonInteractable()
    {
        _getBonusButton.interactable = RewardManager.Instance.CurrentReward > 0;
        SignalBus.Fire(new ThemeChangeSignal(ThemeManager.Instance.CurrentTheme, ThemeManager.Instance.CurrentThemeIndex));
    }

    private void UpdateTexts()
    {
        bool isActive = RewardManager.Instance.CurrentReward > 0;

        _timerText.gameObject.SetActive(!isActive);
        _getBonusText.gameObject.SetActive(isActive);

        _awardText.text = RewardManager.Instance.CurrentReward.ToString();
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<OnBonusGrantSignal>(OnBonusCredited);
        OnBonusCredited(new OnBonusGrantSignal());
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnBonusGrantSignal>(OnBonusCredited);
        SignalBus.Unsubscribe<OnGameRewardSignal>(OnBonusCollected);
    }
}

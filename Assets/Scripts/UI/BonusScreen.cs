using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

    private RewardManager _rewardManager;
    private ThemeManager _themeManager;

    [Inject]
    private void Construct(RewardManager rewardManager, ThemeManager themeManager)
    {
        _rewardManager = rewardManager;
        _themeManager = themeManager;
    }

    private void Awake()
    {        
        SignalBus.Subscribe<OnGameRewardSignal>(OnBonusCollected);
    }
    private void Start()
    {
        OnBonusCredited(new OnBonusGrantSignal());
        _getBonusButton.onClick.AddListener(GetBonus);
    }

    private void Update()
    {
        if (_rewardManager == null) return;
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
        if (_rewardManager == null) return;

        TimeSpan timeUntilNextReward = _rewardManager.GetTimeUntilNextReward();
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
        _rewardManager.CollectBonus();

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
        _getBonusButton.interactable = _rewardManager.CurrentReward > 0;
        SignalBus.Fire(new ThemeChangeSignal(_themeManager.CurrentTheme, _themeManager.CurrentThemeIndex));
    }

    private void UpdateTexts()
    {
        bool isActive = _rewardManager.CurrentReward > 0;

        _timerText.gameObject.SetActive(!isActive);
        _getBonusText.gameObject.SetActive(isActive);

        _awardText.text = _rewardManager.CurrentReward.ToString();
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<OnBonusGrantSignal>(OnBonusCredited);
        //OnBonusCredited(new OnBonusGrantSignal());
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnBonusGrantSignal>(OnBonusCredited);
        SignalBus.Unsubscribe<OnGameRewardSignal>(OnBonusCollected);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PixelAnticheat.Detectors;
using PixelAnticheat.Models;
using PixelAnticheat;
using Zenject;

public class RewardManager : MonoBehaviour
{
    public int CurrentReward { get; private set; }

    private RewardConfig _rewardConfig;
    private RewardData _rewardData;
    private SaveManager _saveManager;
    private TimeHackDetector _timeHackDetector;   

    [Inject]
    private void Construct(SaveManager saveManager, RewardConfig rewardConfig)
    {
        _saveManager = saveManager;
        _rewardConfig = rewardConfig;
    }


    private void Start()
    {
        LoadRewardData();
        UpdateRewardState();
        CalculateCurrentReward();

        StartCoroutine(RewardTimerCoroutine());
        //TimeAntiCheat();  ------- without server's time don't working
    }        

    public TimeSpan GetTimeUntilNextReward()
    {
        DateTime now = DateTime.Now;

        foreach (int hour in _rewardConfig.RewardHours)
        {
            DateTime rewardTime = now.Date.AddHours(hour);
            if (rewardTime > now)
            {
                return rewardTime - now;
            }
        }

        DateTime firstRewardTomorrow = now.Date.AddDays(1).AddHours(_rewardConfig.RewardHours[0]);
        return firstRewardTomorrow - now;
    }

    public void CollectBonus()
    {
        int collectedReward = CurrentReward;

        for (int i = 0; i < _rewardData.AvailableRewards.Length; i++)
        {
            if (_rewardData.AvailableRewards[i])
            {
                _rewardData.AvailableRewards[i] = false;
                _rewardData.CollectedRewards[i] = true;
            }
        }

        if (CurrentReward > 0)
        {            
            //Debug.Log($"Player collected {CurrentReward} reward points.");
            SignalBus.Fire(new OnGameRewardSignal(0, CurrentReward));
            CurrentReward = 0;
        }

        SaveRewardData();
    }

    public void TimeCheatDetected()
    {
        //Debug.Log("AAAAAALLLLLO!!!!");
    }

    private void TimeAntiCheat()
    {
        // Initialize All Detectors
        AntiCheat.Instance()
            .AddDetector<TimeHackDetector>(new TimeHackDetectorConfig()
            {                
                availableTolerance = 30,
                networkCompare = false,
                timeCheckInterval = 10f
            });

        _timeHackDetector = (TimeHackDetector)AntiCheat.Instance().GetDetector<TimeHackDetector>();

        // Add Detectors Handlers       
        _timeHackDetector.OnCheatingDetected.AddListener(OnTimeHackDetected);

        //_timeHackDetector.OnCheatingDetected.AddListener(OnTimeHackDetected);
        _timeHackDetector.StartDetector();               

        if (_timeHackDetector != null)
        {
            Debug.Log("TimeHackDetector started.");           
        }
    }

    private void OnTimeHackDetected(string messege)
    {
        Debug.Log($"Cheating detected: {messege}");
    }

    private void UpdateRewardState()
    {
        DateTime now = DateTime.Now;

        // Сброс состояния, если день изменился
        if (_rewardData.LastRewardDay != now.Date.ToString("yyyy-MM-dd"))
        {
            ResetDailyRewards(now);
        }

        // Получаем все временные отметки наград
        List<DateTime> rewardTimes = GetRewardTimes(now);

        // Фильтруем награды: только прошлые и несобранные
        List<DateTime> pastRewards = rewardTimes
            .Where(rt => rt <= now)
            .ToList();

        // Помечаем старые награды как "собранные", если они вне диапазона 2 последних
        foreach (DateTime rewardTime in pastRewards.Take(pastRewards.Count - _rewardConfig.MaxMissedRewards))
        {
            if (!_rewardData.CollectedRewards[rewardTime.Hour])
            {
                _rewardData.CollectedRewards[rewardTime.Hour] = true;
            }
        }

        // Делаем доступными только последние 2 награды
        _rewardData.AvailableRewards = new bool[24];
        foreach (DateTime rewardTime in pastRewards.TakeLast(_rewardConfig.MaxMissedRewards))
        {
            if (!_rewardData.CollectedRewards[rewardTime.Hour])
            {
                _rewardData.AvailableRewards[rewardTime.Hour] = true;
            }
        }

        SaveRewardData();
    }

    private List<DateTime> GetRewardTimes(DateTime now)
    {
        List<DateTime> rewardTimes = new List<DateTime>();

        foreach (int hour in _rewardConfig.RewardHours)
        {
            rewardTimes.Add(now.Date.AddHours(hour));
        }

        return rewardTimes;
    }

    private void ResetDailyRewards(DateTime now)
    {
        _rewardData.LastRewardDay = now.Date.ToString("yyyy-MM-dd");
        _rewardData.AvailableRewards = new bool[24];
        _rewardData.CollectedRewards = new bool[24];
    }

    private void CalculateCurrentReward()
    {
        int previousReward = CurrentReward;
        CurrentReward = 0;

        for (int i = 0; i < _rewardData.AvailableRewards.Length; i++)
        {
            if (_rewardData.AvailableRewards[i])
            {
                CurrentReward += _rewardConfig.RewardAmount;
            }
        }

        if (CurrentReward > 0 || previousReward > 0)
        {
            SignalBus.Fire(new OnBonusGrantSignal(CurrentReward));
        }        
    }

    private IEnumerator RewardTimerCoroutine()
    {
        while (true)
        {
            TimeSpan timeUntilNextReward = GetTimeUntilNextReward();
                        
            yield return new WaitForSeconds(1f);

            if (timeUntilNextReward.TotalSeconds <= 1)
            {
                //Debug.Log("update?");

                UpdateRewardState();
                CalculateCurrentReward();

                SignalBus.Fire(new OnBonusGrantSignal());
            }            
        }
    }

    private void LoadRewardData()
    {
        _rewardData = _saveManager.LoadRewardData();

        if (_rewardData == null)
        {
            _rewardData = new RewardData();
        }
    }

    private void SaveRewardData()
    {
        _saveManager.SaveRewardData(_rewardData);
    }

    /*
    private void OnDestroy()
    {
        _timeHackDetector.StopDetector();
    }
    */
}



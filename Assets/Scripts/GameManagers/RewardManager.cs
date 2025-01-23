using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }
    public int CurrentReward { get; private set; }

    [SerializeField] private int _rewardAmount;
    [SerializeField] private int[] _rewardHours = { 8, 12, 16, 20 };
    [SerializeField] private int _maxMissedRewards = 2;

    private RewardData _rewardData;

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
        LoadRewardData();
        UpdateRewardState();
        CalculateCurrentReward();

        StartCoroutine(RewardTimerCoroutine());
    }

    public TimeSpan GetTimeUntilNextReward()
    {
        DateTime now = DateTime.Now;

        foreach (int hour in _rewardHours)
        {
            DateTime rewardTime = now.Date.AddHours(hour);
            if (rewardTime > now)
            {
                return rewardTime - now;
            }
        }

        DateTime firstRewardTomorrow = now.Date.AddDays(1).AddHours(_rewardHours[0]);
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

    private void UpdateRewardState()
    {
        DateTime now = DateTime.Now;

        // ����� ���������, ���� ���� ���������
        if (_rewardData.LastRewardDay != now.Date.ToString("yyyy-MM-dd"))
        {
            ResetDailyRewards(now);
        }

        // �������� ��� ��������� ������� ������
        List<DateTime> rewardTimes = GetRewardTimes(now);

        // ��������� �������: ������ ������� � �����������
        List<DateTime> pastRewards = rewardTimes
            .Where(rt => rt <= now)
            .ToList();

        // �������� ������ ������� ��� "���������", ���� ��� ��� ��������� 2 ���������
        foreach (DateTime rewardTime in pastRewards.Take(pastRewards.Count - _maxMissedRewards))
        {
            if (!_rewardData.CollectedRewards[rewardTime.Hour])
            {
                _rewardData.CollectedRewards[rewardTime.Hour] = true;
            }
        }

        // ������ ���������� ������ ��������� 2 �������
        _rewardData.AvailableRewards = new bool[24];
        foreach (DateTime rewardTime in pastRewards.TakeLast(_maxMissedRewards))
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

        foreach (int hour in _rewardHours)
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
        CurrentReward = 0;

        for (int i = 0; i < _rewardData.AvailableRewards.Length; i++)
        {
            if (_rewardData.AvailableRewards[i])
            {
                CurrentReward += _rewardAmount;
            }
        }

        SignalBus.Fire(new OnBonusGrantSignal());
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
        _rewardData = SaveManager.Instance.LoadRewardData();

        if (_rewardData == null)
        {
            _rewardData = new RewardData();
        }
    }

    private void SaveRewardData()
    {
        SaveManager.Instance.SaveRewardData(_rewardData);
    }
}



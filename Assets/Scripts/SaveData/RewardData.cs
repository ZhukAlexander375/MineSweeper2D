using System;

[Serializable]
public class RewardData
{
    public bool[] AvailableRewards = new bool[24];
    public bool[] CollectedRewards = new bool[24];
    public string LastRewardDay;
}

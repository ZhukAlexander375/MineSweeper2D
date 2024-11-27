using UnityEngine;

public struct OnGameRewardSignal 
{
    public int RewardId; //stars? diamonds? else?
    public int Count;

    public OnGameRewardSignal(int rewardId, int count)
    {
        RewardId = rewardId;
        Count = count;
    }
}

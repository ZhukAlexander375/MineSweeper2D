using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sector Rewards", menuName = "Create Sector Rewards")]
public class SectorRewardConfig : ScriptableObject
{
    [Header("Base rewards")]
    public List<RewardLevel> RewardLevels;

    [Header("Multipliers: Infinity, Hardcore, Time")]
    public List<float> RewardMultipliers;

    public float GetMultiplier(GameMode gameMode)
    {
        int index = (int)gameMode;

        if (index >= 0 && index < RewardMultipliers.Count)
        {
            return RewardMultipliers[index];
        }

        return 1f;
    }
}

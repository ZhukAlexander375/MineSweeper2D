using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sector Rewards", menuName = "Create Sector Rewards")]
public class SectorRewardConfig : ScriptableObject
{
    public List<RewardLevel> RewardLevels;
}

using UnityEngine;

[CreateAssetMenu(fileName = "RewardConfig", menuName = "Create Reward Config")]
public class RewardConfig : ScriptableObject
{
    public int RewardAmount;
    public int[] RewardHours = { 8, 12, 16, 20 };
    public int MaxMissedRewards = 2;
}

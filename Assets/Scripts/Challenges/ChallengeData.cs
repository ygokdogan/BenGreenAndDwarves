using Effects;
using UnityEngine;

namespace Challenges
{
    [CreateAssetMenu(fileName = "New Challenge Data", menuName = "Data/Challenge", order = 0)]
    public class ChallengeData : ScriptableObject
    {
        public string displayName;
        
        [TextArea]
        public string description;

        [Header("Starting Stat Effect")]
        public bool hasStartingStatChange;
        public StatType startingStat;
        public int startingValue;
        [Header("Starting Max Stat Effect")]
        public bool hasStartingMaxStatChange;
        public StatType startingMaxStat;
        public int startingMaxValue;

        [Header("Objective")]
        public ObjectiveType objectiveType;
        public int durationInDays;
        
        [Tooltip("Only for stat tracking objectives")]
        public StatType trackedStat;
        public int maximumValue;
        public int minimumValue;

        [Tooltip("Only for accept/reject objectives")]
        public int requiredCount;
        public CountScope countScope;
        
        [Header("Reward")]
        public RewardType rewardType;
        public StatType rewardStat;
        public int rewardValue;
    }

    public enum ObjectiveType
    {
        SurviveUntilEnd,

        KeepStatInRangeAtEnd,
        KeepStatInRangeAlways,

        KeepStatAboveAlways,
        KeepStatBelowAlways,

        KeepStatAboveAtEnd,
        KeepStatBelowAtEnd,

        AcceptOffers,
        RejectOffers
    }

    public enum RewardType
    {
        IncreaseMaxKeepingPercentage,
        CenterAllStats,
        CheatDeath,
        NormalizeStat,
        IgnoreUpkeep,
    }

    public enum CountScope
    {
        Total,
        PerDay
    }
}

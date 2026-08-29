using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Effects;

namespace Challenges
{
    public class ChallengeCardUI : MonoBehaviour
    {
        private ChallengeData challenge;

        [Header("UI References")]
        public TextMeshProUGUI displayNameText;
        public TextMeshProUGUI displayDescriptionText;
        public TextMeshProUGUI objectiveText;
        public TextMeshProUGUI rewardText;
        
        public Image objectiveIcon; 

        public void Setup(ChallengeData data)
        {
            challenge = data;
            if (challenge == null)
            {
                Debug.LogWarning("ChallengeCardUI received no ChallengeData.", this);
                return;
            }

            if (displayNameText != null)
                displayNameText.text = challenge.displayName;

            if (displayDescriptionText != null)
            {
                string startingEffects = FormatStartingEffects(challenge);
                string description = string.IsNullOrEmpty(challenge.description)
                    ? string.Empty
                    : $"<i>{challenge.description}</i>";

                displayDescriptionText.text = string.IsNullOrEmpty(startingEffects)
                    ? description
                    : string.IsNullOrEmpty(description)
                        ? startingEffects
                        : $"{description}\n{startingEffects}";
            }

            if (objectiveText != null)
                objectiveText.text = FormatObjective(challenge);

            if (rewardText != null)
                rewardText.text = FormatReward(challenge);
        }

        private string FormatObjective(ChallengeData data)
        {
            string duration = $"<color=#8A5A00>{data.durationInDays} Days</color>";
            
            return data.objectiveType switch
            {
                ObjectiveType.SurviveUntilEnd => $"Survive for {duration}.",
                ObjectiveType.KeepStatInRangeAtEnd => $"Finish with {data.trackedStat} between {data.minimumValue}-{data.maximumValue} after {duration}.",
                ObjectiveType.KeepStatInRangeAlways => $"Keep {data.trackedStat} between {data.minimumValue}-{data.maximumValue} for {duration}.",
                ObjectiveType.KeepStatAboveAlways => $"Keep {data.trackedStat} at {data.minimumValue} or above for {duration}.",
                ObjectiveType.KeepStatBelowAlways => $"Keep {data.trackedStat} at {data.maximumValue} or below for {duration}.",
                ObjectiveType.KeepStatAboveAtEnd => $"Finish with {data.trackedStat} at {data.minimumValue} or above after {duration}.",
                ObjectiveType.KeepStatBelowAtEnd => $"Finish with {data.trackedStat} at {data.maximumValue} or below after {duration}.",
                ObjectiveType.AcceptOffers => $"Accept {data.requiredCount} offers {FormatCountScope(data.countScope, duration)}.",
                ObjectiveType.RejectOffers => $"Reject {data.requiredCount} offers {FormatCountScope(data.countScope, duration)}.",
                _ => "Unknown Objective"
            };
        }

        private string FormatReward(ChallengeData data)
        {
            return data.rewardType switch
            {
                RewardType.IncreaseMaxKeepingPercentage => $"Reward: <color=#006D88>Increase {data.rewardStat} max to {data.rewardValue}</color>",
                RewardType.CenterAllStats => "Reward: <color=#006D88>Center All Stats</color>",
                RewardType.CheatDeath => $"Reward: <color=#006D88>Prevent one {data.rewardStat} game over</color>",
                RewardType.NormalizeStat => $"Reward: <color=#006D88>Normalize {data.rewardStat}</color>",
                RewardType.IgnoreUpkeep => $"Reward: <color=#006D88>Ignore negative {data.rewardStat} upkeep</color>",
                _ => "Reward: Unknown"
            };
        }

        private static string FormatStartingEffects(ChallengeData data)
        {
            string currentValueEffect = data.hasStartingStatChange
                ? FormatStartingEffect("Start", data.startingStat, data.startingValue)
                : string.Empty;

            string maximumValueEffect = data.hasStartingMaxStatChange
                ? FormatStartingEffect("Starting max", data.startingMaxStat, data.startingMaxValue)
                : string.Empty;

            if (string.IsNullOrEmpty(currentValueEffect)) return maximumValueEffect;
            if (string.IsNullOrEmpty(maximumValueEffect)) return currentValueEffect;
            return $"{currentValueEffect}\n{maximumValueEffect}";
        }

        private static string FormatStartingEffect(string label, StatType stat, int value)
        {
            string colorHex = value >= 0 ? "#167A36" : "#B42318";
            string sign = value > 0 ? "+" : string.Empty;
            return $"<color={colorHex}>{label}: {sign}{value} {stat}</color>";
        }

        private static string FormatCountScope(CountScope scope, string duration)
        {
            return scope == CountScope.PerDay
                ? $"each day for {duration}"
                : $"within {duration}";
        }
    }
}

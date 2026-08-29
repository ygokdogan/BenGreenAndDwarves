using System;
using System.Collections.Generic;
using Effects;
using UnityEngine;

namespace Challenges
{
    public class ChallengeManager : MonoBehaviour
    {
        public static ChallengeManager Instance;

        public ChallengeData activeChallenge;

        private bool isActive;
        private bool isFailed;
        private bool isCompleted;

        private int selectedDay;
        private int completedChallengeDays;

        private int acceptedToday;
        private int rejectedToday;

        private int totalAccepted;
        private int totalRejected;

        private bool cheatDeathAvailable;
        private StatType cheatDeathStat;
        private bool ignoresUpkeep;
        private StatType ignoredUpkeepStat;

        public event Action<ChallengeData> OnChallengeSelected;
        public event Action OnChallengeProgressChanged;
        public event Action<bool> OnChallengeResolved;

        public int CurrentChallengeDay
        {
            get
            {
                if (!activeChallenge || TimeManager.Instance == null) return 0;
                return Mathf.Clamp(TimeManager.Instance.CurrentDay - selectedDay + 1, 1, activeChallenge.durationInDays);
            }
        }

        public int CompletedChallengeDays
        {
            get
            {
                if (!activeChallenge || TimeManager.Instance == null) return 0;
                int elapsedDays = TimeManager.Instance.CurrentDay - selectedDay;
                return Mathf.Clamp(Mathf.Max(completedChallengeDays, elapsedDays), 0, activeChallenge.durationInDays);
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            if (StatManager.Instance)
                StatManager.Instance.OnStatChanged += OnStatChanged;
        }

        private void OnDestroy()
        {
            if (StatManager.Instance) StatManager.Instance.OnStatChanged -= OnStatChanged;
        }

        public void SelectChallenge(ChallengeData challenge)
        {
            if (!challenge) return;

            activeChallenge = challenge;
            
            isActive = true;
            isFailed = false;
            isCompleted = false;
            
            selectedDay = TimeManager.Instance.CurrentDay;
            completedChallengeDays = 0;
            
            acceptedToday = rejectedToday = totalAccepted = totalRejected = 0;
            cheatDeathAvailable = false;
            ignoresUpkeep = false;

            OnChallengeSelected?.Invoke(activeChallenge);

            if (activeChallenge.hasStartingStatChange)
            {
                StatManager.Instance.SetCurrent(activeChallenge.startingStat, activeChallenge.startingValue);
            }

            if (activeChallenge.hasStartingMaxStatChange)
            {
                StatManager.Instance.SetMax(activeChallenge.startingMaxStat, activeChallenge.startingMaxValue);
            }

            NotifyProgressChanged();
        }

        public void OnOfferAccepted()
        {
            if (!CanTrackChallenge()) return;

            acceptedToday++;
            totalAccepted++;
            NotifyProgressChanged();
        }

        public void OnOfferRejected()
        {
            if (!CanTrackChallenge()) return;
            
            rejectedToday++;
            totalRejected++;
            NotifyProgressChanged();
        }

        public void OnGameEnded()
        {
            if (!isActive || isCompleted) return;

            FailChallenge();
        }

        public bool TryPreventGameOver(StatType statType)
        {
            if (!cheatDeathAvailable || statType != cheatDeathStat)
                return false;

            cheatDeathAvailable = false;
            StatManager.Instance.SetCurrent(statType, 50);
            Debug.Log($"CheatDeath consumed for {statType}.");
            return true;
        }

        public StatEffect[] ResolveUpkeepEffects(StatEffect[] effects)
        {
            if (!ignoresUpkeep || effects == null)
                return effects;

            List<StatEffect> resolvedEffects = new List<StatEffect>(effects.Length);
            foreach (StatEffect effect in effects)
            {
                bool isIgnoredPenalty =
                    effect.type == ignoredUpkeepStat && effect.amount < 0;

                if (!isIgnoredPenalty)
                    resolvedEffects.Add(effect);
            }

            return resolvedEffects.ToArray();
        }

        private void OnStatChanged(StatType changedStat, int newValue, int oldValue)
        {
            if (!CanTrackChallenge()) return;

            if (changedStat != activeChallenge.trackedStat)
                return;

            switch (activeChallenge.objectiveType)
            {
                case ObjectiveType.KeepStatInRangeAlways:
                    if (newValue < activeChallenge.minimumValue || newValue > activeChallenge.maximumValue)
                    {
                        FailChallenge();
                    }
                    break;
                
                case ObjectiveType.KeepStatAboveAlways:
                    if (newValue < activeChallenge.minimumValue)
                    {
                        FailChallenge();
                    }
                    break;

                case ObjectiveType.KeepStatBelowAlways:
                    if (newValue > activeChallenge.maximumValue)
                    {
                        FailChallenge();
                    }

                    break;
            }

            NotifyProgressChanged();
        }
        
        public void OnDayResolved(int completedDay)
        {
            if (!CanTrackChallenge()) return;

            int finalDay = selectedDay + activeChallenge.durationInDays - 1;

            if (completedDay > finalDay) return;

            completedChallengeDays = Mathf.Clamp(completedDay - selectedDay + 1, 0, activeChallenge.durationInDays);

            CheckDailyCountObjective();
            
            if (isFailed) return;
            if (completedDay < finalDay)
            {
                ResetDailyCounts();
                NotifyProgressChanged();
                return;
            }

            CheckFinalObjective();
        }
        
        private void CheckDailyCountObjective()
        {
            bool isCountObjective =
                activeChallenge.objectiveType ==
                ObjectiveType.AcceptOffers ||
                activeChallenge.objectiveType ==
                ObjectiveType.RejectOffers;

            if (!isCountObjective)
                return;

            if (activeChallenge.countScope != CountScope.PerDay)
                return;

            int currentCount =
                activeChallenge.objectiveType ==
                ObjectiveType.AcceptOffers
                    ? acceptedToday
                    : rejectedToday;

            if (currentCount < activeChallenge.requiredCount)
            {
                FailChallenge();
            }
        }

        private void CheckFinalObjective()
        {
            switch (activeChallenge.objectiveType)
            {
                case ObjectiveType.SurviveUntilEnd:
                    CompleteChallenge();
                    break;
                
                case ObjectiveType.KeepStatInRangeAtEnd:
                    CheckRangeAtEnd();
                    break;
                
                case ObjectiveType.KeepStatAboveAtEnd:
                    CheckAboveAtEnd();
                    break;
                
                case ObjectiveType.KeepStatBelowAtEnd:
                    CheckBelowAtEnd();
                    break;

                case ObjectiveType.AcceptOffers:
                case ObjectiveType.RejectOffers:
                    CheckTotalCountAtEnd();
                    break;
                
                case ObjectiveType.KeepStatAboveAlways:
                case ObjectiveType.KeepStatBelowAlways:
                case ObjectiveType.KeepStatInRangeAlways:
                    CompleteChallenge();
                    break;
            }
        }

        private void CheckRangeAtEnd()
        {
            int currentValue = StatManager.Instance.stats[activeChallenge.trackedStat];
            
            bool isSuccessful = currentValue >= activeChallenge.minimumValue && currentValue <= activeChallenge.maximumValue;

            if (isSuccessful) 
                CompleteChallenge();
            else 
                FailChallenge();
        }

        private void CheckAboveAtEnd()
        {
            int currentValue =
                StatManager.Instance.stats[activeChallenge.trackedStat];

            if (currentValue >= activeChallenge.minimumValue)
                CompleteChallenge();
            else
                FailChallenge();
        }
        
        private void CheckBelowAtEnd()
        {
            int currentValue =
                StatManager.Instance.stats[activeChallenge.trackedStat];

            if (currentValue <= activeChallenge.maximumValue)
                CompleteChallenge();
            else
                FailChallenge();
        }

        private void CheckTotalCountAtEnd()
        {
            if (activeChallenge.countScope == CountScope.PerDay)
            {
                CompleteChallenge();
                return;
            }
            
            int currentCount = activeChallenge.objectiveType == ObjectiveType.AcceptOffers ? totalAccepted : totalRejected;

            if (currentCount >= activeChallenge.requiredCount)
                CompleteChallenge();
            else
                FailChallenge();
        }

        private void CompleteChallenge()
        {
            isActive = false;
            isCompleted = true;

            ApplyReward();
            OnChallengeResolved?.Invoke(true);
            OnChallengeProgressChanged?.Invoke();
            Debug.Log($"Challenge Completed: {activeChallenge.displayName}");
        }

        private void FailChallenge()
        {
            isActive = false;
            isFailed = true;
            OnChallengeResolved?.Invoke(false);
            
            Debug.Log($"Challenge Failed: {activeChallenge.displayName}");
        }
        
        private void ApplyReward()
        {
            switch (activeChallenge.rewardType)
            {
                case RewardType.IncreaseMaxKeepingPercentage:

                    StatManager.Instance.SetMaxKeepingPercentage(
                        activeChallenge.rewardStat,
                        activeChallenge.rewardValue
                    );

                    break;

                case RewardType.CheatDeath:
                    cheatDeathStat = activeChallenge.rewardStat;
                    cheatDeathAvailable = true;
                    break;

                case RewardType.NormalizeStat:
                    StatManager.Instance.NormalizeToHalfOfMaximum(
                        activeChallenge.rewardStat
                    );
                    break;
                
                case RewardType.CenterAllStats:
                    StatManager.Instance.NormalizeAllStats();
                    break;

                case RewardType.IgnoreUpkeep:
                    ignoredUpkeepStat = activeChallenge.rewardStat;
                    ignoresUpkeep = true;
                    break;
            }
        }
        
        private bool CanTrackChallenge()
        {
            return activeChallenge &&
                   isActive &&
                   !isFailed &&
                   !isCompleted;
        }

        private void ResetDailyCounts()
        {
            acceptedToday = 0;
            rejectedToday = 0;
        }

        public int GetCurrentOfferCount()
        {
            if (!activeChallenge) return 0;

            bool accepting = activeChallenge.objectiveType == ObjectiveType.AcceptOffers;
            return activeChallenge.countScope == CountScope.PerDay
                ? (accepting ? acceptedToday : rejectedToday)
                : (accepting ? totalAccepted : totalRejected);
        }

        private void NotifyProgressChanged()
        {
            OnChallengeProgressChanged?.Invoke();
        }
    }
}

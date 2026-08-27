using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Effects
{
    [System.Serializable]
    public struct PendingEffect
    {
        public EncounterData encounter;
        public StatEffect effect;
        public int triggerHour;
        public string triggerText;
    }
    
    public class PendingEffects : MonoBehaviour
    {
        public static PendingEffects Instance;
        public event Action<List<PendingEffect>> OnEffectsTriggered;
        
        public List<PendingEffect> pending = new List<PendingEffect>();
        private List<PendingEffect> triggeredEffects = new List<PendingEffect>();

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
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnHourChanged += OnTimeAdvanced;
                TimeManager.Instance.OnDayEnded += OnTimeAdvanced;
            }
        }

        private void OnDestroy()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnHourChanged -= OnTimeAdvanced;
                TimeManager.Instance.OnDayEnded -= OnTimeAdvanced;
            }
        }

        private void OnTimeAdvanced(int currentVal)
        {
            CheckAndApply();
        }

        public bool HasPendingEffectsForCurrentTime()
        {
            if (TimeManager.Instance == null) return false;
            int now = TimeManager.Instance.TotalHoursElapsed;
            for (int i = 0; i < pending.Count; i++)
            {
                if (pending[i].triggerHour <= now) return true;
            }
            return false;
        }

        public void Schedule(EncounterData encounter, StatEffect effect, int hoursAfter)
        {
            int triggerHour = TimeManager.Instance.TotalHoursElapsed + hoursAfter + 1;
            string text = encounter.accepted ? encounter.acceptedDelayedText : encounter.rejectedDelayedText;
            pending.Add(new PendingEffect{ encounter = encounter, effect = effect, triggerHour = triggerHour, triggerText = text });
        }
    
        private void CheckAndApply()
        {
            int now = TimeManager.Instance.TotalHoursElapsed;
            triggeredEffects.Clear();
            
            for (int i = pending.Count -1; i >= 0; i--)
            {
                if (pending[i].triggerHour <= now)
                {
                    StatManager.Instance.ApplyEffect(pending[i].effect, checkGameOver: false);
                    triggeredEffects.Add(pending[i]);
                    pending.RemoveAt(i);
                }
            }

            if (triggeredEffects.Count > 0)
            {
                triggeredEffects.Reverse();
                OnEffectsTriggered?.Invoke(triggeredEffects);
            }
        }
    }
}
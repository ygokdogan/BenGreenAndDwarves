using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public struct PendingEffect
    {
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

        public void Schedule(StatEffect effect, int hoursAfter, string triggerText)
        {
            int triggerHour = TimeManager.Instance.TotalHoursElapsed + hoursAfter + 1;
            pending.Add(new PendingEffect{ effect = effect, triggerHour = triggerHour, triggerText = triggerText });
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
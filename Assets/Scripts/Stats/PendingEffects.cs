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
    }
    
    public class PendingEffects : MonoBehaviour
    {
        public static PendingEffects Instance;
        public event Action<List<StatEffect>> OnEffectsTriggered;
        
        public List<PendingEffect> pending = new List<PendingEffect>();
        private List<StatEffect> triggeredEffects = new List<StatEffect>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            TimeManager.Instance.OnHourChanged += _ => CheckAndApply();
        }

        public void Schedule(StatEffect effect, int hoursAfter)
        {
            int triggerHour = TimeManager.Instance.TotalHoursElapsed + hoursAfter + 1;
            pending.Add(new PendingEffect{ effect = effect, triggerHour = triggerHour });
        }
    
        private void CheckAndApply()
        {
            Debug.Log("");
            int now = TimeManager.Instance.TotalHoursElapsed;
            triggeredEffects.Clear();
            
            for (int i = pending.Count -1; i >= 0; i--)
            {
                if (pending[i].triggerHour <= now)
                {
                    StatManager.Instance.ApplyEffect(pending[i].effect);
                    triggeredEffects.Add(pending[i].effect);
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
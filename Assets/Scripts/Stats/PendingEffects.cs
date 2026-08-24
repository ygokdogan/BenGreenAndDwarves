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
        public List<PendingEffect> pending = new List<PendingEffect>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            TimeManager.Instance.OnHourChanged += _ => CheckAndApply();
        }

        public void Schedule(StatEffect effect, int hoursAfter)
        {
            int triggerHour = TimeManager.Instance.TotalHoursElapsed + hoursAfter;
            pending.Add(new PendingEffect{ effect = effect, triggerHour = triggerHour });
        }
    
        private void CheckAndApply()
        {
            Debug.Log("");
            int now = TimeManager.Instance.TotalHoursElapsed;
            for (int i = 0; i < pending.Count - 1; i++)
            {
                if (pending[i].triggerHour <= now)
                {
                    StatManager.Instance.ApplyEffect(pending[i].effect);
                    pending.RemoveAt(i);
                }
            }
        }
    }
}
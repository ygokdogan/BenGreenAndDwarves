using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatUIManager : MonoBehaviour
    {
        public static StatUIManager Instance;

        [Header("UI Bars")]
        public StatBar.StatBar happinessBar;
        public StatBar.StatBar healthBar;
        public StatBar.StatBar storageBar;
        public StatBar.StatBar cashBar;

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
            {
                StatManager.Instance.OnStatChanged += SetBarValue;
                RefreshAllSliders();
            }

            ClearPreview();
        }

        private void OnDestroy()
        {
            if (StatManager.Instance)
            {
                StatManager.Instance.OnStatChanged -= SetBarValue;
            }
        }

        public void RefreshAllSliders()
        {
            if (!StatManager.Instance) return;
            foreach (var stat in StatManager.Instance.stats)
            {
                SetBarValue(stat.Key, stat.Value, stat.Value);
            }
        }

        private void SetBarValue(StatType type, int newValue, int oldValue)
        {
            switch (type)
            {
                case StatType.Happiness: if (happinessBar) happinessBar.SetValue(newValue, oldValue); break;
                case StatType.Health: if (healthBar) healthBar.SetValue(newValue, oldValue); break;
                case StatType.Storage: if (storageBar) storageBar.SetValue(newValue, oldValue); break;
                case StatType.Cash: if (cashBar) cashBar.SetValue(newValue, oldValue); break;
                default: break;
            }
        }

        public void ShowPreview(StatEffect[] effects)
        {
            ClearPreview();
            if (effects == null) return;

            foreach (StatEffect effect in effects)
            {
                SetIndicatorActive(effect.type, true);
            }
        }

        public void ClearPreview()
        {
            SetIndicatorActive(StatType.Happiness, false);
            SetIndicatorActive(StatType.Health, false);
            SetIndicatorActive(StatType.Storage, false);
            SetIndicatorActive(StatType.Cash, false);
        }

        private void SetIndicatorActive(StatType type, bool active)
        {
            switch (type)
            {
                case StatType.Happiness: if (happinessBar) happinessBar.Highlight(active); break;
                case StatType.Health: if (healthBar) healthBar.Highlight(active); break;
                case StatType.Storage: if (storageBar) storageBar.Highlight(active); break;
                case StatType.Cash: if (cashBar) cashBar.Highlight(active); break;
            }
        }
    }
}
using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatUIManager : MonoBehaviour
    {
        public static StatUIManager Instance;

        [Header("UI Barları")]
        public Slider happinessSlider;
        public Slider healthSlider;
        public Slider storageSlider;
        public Slider cashSlider;

        [Header("Stat Indicators (Preview Icons / Dots)")]
        public GameObject happinessIndicator;
        public GameObject healthIndicator;
        public GameObject storageIndicator;
        public GameObject cashIndicator;

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
            if (happinessSlider) { happinessSlider.minValue = 0; happinessSlider.maxValue = 100; }
            if (healthSlider) { healthSlider.minValue = 0; healthSlider.maxValue = 100; }
            if (storageSlider) { storageSlider.minValue = 0; storageSlider.maxValue = 100; }
            if (cashSlider) { cashSlider.minValue = 0; cashSlider.maxValue = 100; }

            if (StatManager.Instance != null)
            {
                StatManager.Instance.OnStatChanged += SetSliderValue;
                RefreshAllSliders();
            }

            ClearPreview();
        }

        private void OnDestroy()
        {
            if (StatManager.Instance != null)
            {
                StatManager.Instance.OnStatChanged -= SetSliderValue;
            }
        }

        public void RefreshAllSliders()
        {
            if (StatManager.Instance == null) return;
            foreach (var stat in StatManager.Instance.stats)
            {
                SetSliderValue(stat.Key, stat.Value);
            }
        }

        private void SetSliderValue(StatType type, int value)
        {
            switch (type)
            {
                case StatType.Happiness: if (happinessSlider) happinessSlider.value = value; break;
                case StatType.Health: if (healthSlider) healthSlider.value = value; break;
                case StatType.Storage: if (storageSlider) storageSlider.value = value; break;
                case StatType.Cash: if (cashSlider) cashSlider.value = value; break;
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
                case StatType.Happiness: if (happinessIndicator) happinessIndicator.SetActive(active); break;
                case StatType.Health: if (healthIndicator) healthIndicator.SetActive(active); break;
                case StatType.Storage: if (storageIndicator) storageIndicator.SetActive(active); break;
                case StatType.Cash: if (cashIndicator) cashIndicator.SetActive(active); break;
            }
        }
    }
}
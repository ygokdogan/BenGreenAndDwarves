using Effects;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance;

        [Header("UI Bars")]
        public GameObject bars;
        public StatBar.StatBar happinessBar;
        public StatBar.StatBar healthBar;
        public StatBar.StatBar storageBar;
        public StatBar.StatBar cashBar;
        
        [Header("Day & Time ")]
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI timeText;

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

            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnHourChanged += ChangeTimeText;
                TimeManager.Instance.OnDayEnded += ChangeDayText;
            }
            
            ChangeDayText(TimeManager.Instance.CurrentDay);
            ChangeTimeText(TimeManager.Instance.CurrentHour);
            ClearPreview();
        }

        private void OnDestroy()
        {
            if (StatManager.Instance)
            {
                StatManager.Instance.OnStatChanged -= SetBarValue;
            }

            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnHourChanged -= ChangeTimeText;
                TimeManager.Instance.OnDayEnded -= ChangeDayText;
            }
        }

        public void RefreshAllSliders()
        {
            if (!StatManager.Instance) return;
            foreach (var stat in StatManager.Instance.stats)
            {
                SetBarValue(stat.Key, stat.Value, 0);
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
                if (effect.Instant)
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

        private void ChangeDayText(int day) => dayText.text = $"Day: {day}";
        private void ChangeTimeText(int time) => timeText.text = $"Time: {time}.00";
    }
}
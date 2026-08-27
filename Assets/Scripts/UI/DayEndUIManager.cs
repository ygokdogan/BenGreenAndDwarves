using System;
using System.Collections.Generic;
using Stats;
using TMPro;
using UI.Utilities;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class DayEndUIManager : MonoBehaviour
    {
        public static DayEndUIManager Instance;

        [Header("UI References")]
        public GameObject gameplayUI;
        public GameObject dayEndPanel;
        public TextMeshProUGUI dayTitleText;
        public TextMeshProUGUI summaryBodyText;
        public TypewriterEffect summaryTypewriter;

        [Header("Daily Upkeep Costs (Negative amounts)")]
        public int dailyCashCost = -10;
        public int dailyHealthCost = -5;
        public int dailyHappinessCost = -5;
        public int dailyStorageCost = -5;

        private Action onNextDayCallback;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (summaryBodyText != null && summaryTypewriter == null)
            {
                summaryTypewriter = summaryBodyText.GetComponent<TypewriterEffect>();
                if (summaryTypewriter == null) summaryTypewriter = summaryBodyText.gameObject.AddComponent<TypewriterEffect>();
            }
        }

        private void Start()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnDayEnded += ShowDayEndSummary;
            }
        }

        private void OnDestroy()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnDayEnded -= ShowDayEndSummary;
            }
        }

        public void ShowDayEndSummary(int completedDay)
        {
            if (dayEndPanel != null)
            {
                dayEndPanel.transform.DOKill();
                dayEndPanel.transform.localScale = Vector3.zero;
                dayEndPanel.SetActive(true);
                dayEndPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
            if (summaryBodyText) summaryBodyText.gameObject.SetActive(true);
            if (gameplayUI) gameplayUI.SetActive(false);

            if (dayTitleText)
            {
                dayTitleText.text = $"Day {completedDay} Complete";
            }

            StatEffect[] upkeepEffects = new StatEffect[]
            {
                new StatEffect { type = StatType.Cash, amount = dailyCashCost },
                new StatEffect { type = StatType.Health, amount = dailyHealthCost },
                new StatEffect { type = StatType.Happiness, amount = dailyHappinessCost },
                new StatEffect { type = StatType.Storage, amount = dailyStorageCost }
            };

            // Apply multi-stat deductions without triggering immediate game over
            StatManager.Instance.ApplyEffects(upkeepEffects, checkGameOver: false);

            string summaryText = $"Daily Expenses & Fatigue:\n" +
                                 $"• Rent & Bills: Cash {dailyCashCost}\n" +
                                 $"• Fatigue: Health {dailyHealthCost}\n" +
                                 $"• Stress: Happiness {dailyHappinessCost}\n" +
                                 $"• Maintenance: Storage {dailyStorageCost}";

            if (summaryTypewriter != null)
            {
                summaryTypewriter.Play(summaryText);
            }
            else if (summaryBodyText != null)
            {
                summaryBodyText.text = summaryText;
            }
        }

        public void OnStartNextDayClicked()
        {
            if (summaryTypewriter != null && summaryTypewriter.IsTyping)
            {
                summaryTypewriter.Skip();
                return;
            }

            if (dayEndPanel) dayEndPanel.SetActive(false);

            if (StatManager.Instance != null && StatManager.Instance.CheckGameOver())
            {
                return;
            }

            if (gameplayUI) gameplayUI.SetActive(true);
            TimeManager.Instance.StartNextDay();
            EncounterManager.Instance?.LoadNextEncounter();
        }
    }
}


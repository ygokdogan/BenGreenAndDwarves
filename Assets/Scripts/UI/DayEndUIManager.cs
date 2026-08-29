using System;
using System.Collections.Generic;
using Challenges;
using TMPro;
using UI.Utilities;
using UnityEngine;
using DG.Tweening;
using Effects;

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
        

        public bool ShowDayEndSummary(int completedDay, UpkeepEffects dailyUpkeep)
        {
            if (EncounterManager.Instance != null && EncounterManager.Instance.IsWaitingForPendingEffects)
            {
                return false;
            }

            if (PendingEffects.Instance != null && PendingEffects.Instance.HasPendingEffectsForCurrentTime())
            {
                return false;
            }

            if (PendingEffectPopup.Instance != null && PendingEffectPopup.Instance.popupRoot != null)
            {
                PendingEffectPopup.Instance.popupRoot.SetActive(false);
            }

            if (dayEndPanel != null)
            {
                dayEndPanel.transform.DOKill();
                dayEndPanel.transform.localScale = Vector3.zero;
                dayEndPanel.SetActive(true);
                dayEndPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
            if (summaryBodyText) summaryBodyText.gameObject.SetActive(true);
            if (dayTitleText)
            {
                dayTitleText.text = $"Day {completedDay} Complete\n {dailyUpkeep.title}";
            }
            
            StatEffect[] resolvedEffects = ChallengeManager.Instance
                ? ChallengeManager.Instance.ResolveUpkeepEffects(dailyUpkeep.effects)
                : dailyUpkeep.effects;

            StatManager.Instance.ApplyEffects(resolvedEffects, checkGameOver: false);
            
            string finalSummary = dailyUpkeep.summaryText;
            
            if (resolvedEffects != null && resolvedEffects.Length > 0)
            {
                finalSummary += "\n\n<b>Daily Effects:</b>\n";
                foreach (var effect in resolvedEffects)
                {
                    string sign = effect.amount > 0 ? "+" : ""; 
                    finalSummary += $"{effect.type}: {sign}{effect.amount}\n";
                }
            }
            

            if (summaryTypewriter != null)
            {
                summaryTypewriter.Play(finalSummary);
            }
            else if (summaryBodyText != null)
            {
                summaryBodyText.text = finalSummary;
            }

            return true;
        }

        public void OnStartNextDayClicked()
        {
            if (summaryTypewriter != null && summaryTypewriter.IsTyping)
            {
                summaryTypewriter.Skip();
                return;
            }

            GameFlowManager.Instance?.StartNextDay();
        }
    }
}

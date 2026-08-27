using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UI.Utilities;
using UnityEngine;
using DG.Tweening;
using Effects;
using ScriptableObjects;

namespace UI
{
    public class PendingEffectPopup : MonoBehaviour
    {
        public static PendingEffectPopup Instance;

        public EncounterUIManager encounterUI;

        public GameObject popupRoot;
        public TextMeshProUGUI bodyText;
        public TypewriterEffect typewriterEffect;
        private Action onClosed;
        
        private Queue<List<PendingEffect>> _effectGroupsQueue = new Queue<List<PendingEffect>>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bodyText != null && typewriterEffect == null)
            {
                typewriterEffect = bodyText.GetComponent<TypewriterEffect>();
                if (typewriterEffect == null) typewriterEffect = bodyText.gameObject.AddComponent<TypewriterEffect>();
            }
        }

        public void Show(List<PendingEffect> effects, Action closed)
        {
            if (DayEndUIManager.Instance != null && DayEndUIManager.Instance.dayEndPanel != null)
            {
                DayEndUIManager.Instance.dayEndPanel.SetActive(false);
            }
            if (encounterUI != null) encounterUI.HideAllPanels();
            onClosed = closed;
            
            Dictionary<EncounterData, List<PendingEffect>> groupedEffects = new Dictionary<EncounterData, List<PendingEffect>>();
            List<PendingEffect> nullEncounters = new List<PendingEffect>();

            foreach (var effect in effects)
            {
                if (effect.encounter != null)
                {
                    if (!groupedEffects.ContainsKey(effect.encounter))
                    {
                        groupedEffects[effect.encounter] = new List<PendingEffect>();
                    }
                    groupedEffects[effect.encounter].Add(effect);
                }
                else
                {
                    nullEncounters.Add(effect);
                }
            }
            
            _effectGroupsQueue.Clear();
            foreach (var group in groupedEffects.Values)
            {
                _effectGroupsQueue.Enqueue(group);
            }
            if (nullEncounters.Count > 0)
            {
                _effectGroupsQueue.Enqueue(nullEncounters);
            }
            
            ShowNextGroup();
        }

        private void ShowNextGroup()
        {
            if (_effectGroupsQueue.Count == 0)
            {
                popupRoot.SetActive(false);
                Action callback = onClosed;
                onClosed = null;
                callback?.Invoke();
                return;
            }
            
            List<PendingEffect> currentGroup = _effectGroupsQueue.Dequeue();
            
            if (popupRoot != null && !popupRoot.activeSelf)
            {
                popupRoot.transform.DOKill();
                popupRoot.transform.localScale = Vector3.zero;
                popupRoot.SetActive(true);
                popupRoot.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
            
            if (bodyText != null) bodyText.gameObject.SetActive(true);
            
            string fullText = BuildGroupText(currentGroup);

            if (typewriterEffect != null)
            {
                typewriterEffect.Play(fullText);
            }
            else if (bodyText != null)
            {
                bodyText.text = fullText;
            }
        }

        public void SkipText()
        {
            if (typewriterEffect != null && typewriterEffect.IsTyping)
            {
                typewriterEffect.Skip();
            }
        }

        public void Close()
        {
            if (typewriterEffect != null && typewriterEffect.IsTyping)
            {
                typewriterEffect.Skip();
                return;
            }
            
            ShowNextGroup();
        }
        
        private string BuildGroupText(IReadOnlyList<PendingEffect> group)
        {
            string commonText = "Results of your previous choices has appeared:";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(commonText);
            builder.AppendLine();

            var currentEncounter = group[0].encounter;
            string delayedText = currentEncounter.accepted ? currentEncounter.acceptedDelayedText : currentEncounter.rejectedDelayedText;
            builder.AppendLine(delayedText);

            return builder.ToString().TrimEnd();
        }

        private string GetStatName(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return "Happiness";
                case StatType.Cash: return "Cash";
                case StatType.Health: return "Health";
                case StatType.Storage: return "Storage";
                default: return type.ToString();
            }
        }
    }
}
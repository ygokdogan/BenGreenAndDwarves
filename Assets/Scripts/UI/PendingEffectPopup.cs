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
        
        private Queue<List<PendingEffect>> effectGroupsQueue = new Queue<List<PendingEffect>>();

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
            
            if (onClosed == null) onClosed = closed;

            bool wasAlreadyOpen = popupRoot != null && popupRoot.activeSelf;
            
            var groupsByEncounter = new Dictionary<EncounterData, List<PendingEffect>>();
            var groupsInDisplayOrder = new List<List<PendingEffect>>();

            foreach (PendingEffect effect in effects)
            {
                // Effects originating from one encounter share one popup. Effects from
                // different encounters retain their own popup and apply independently.
                if (effect.encounter == null)
                {
                    groupsInDisplayOrder.Add(new List<PendingEffect> { effect });
                    continue;
                }

                if (!groupsByEncounter.TryGetValue(effect.encounter, out List<PendingEffect> group))
                {
                    group = new List<PendingEffect>();
                    groupsByEncounter.Add(effect.encounter, group);
                    groupsInDisplayOrder.Add(group);
                }

                group.Add(effect);
            }

            foreach (List<PendingEffect> group in groupsInDisplayOrder)
            {
                effectGroupsQueue.Enqueue(group);
            }
            
            if (!wasAlreadyOpen)
            {
                ShowNextGroup();
            }
        }

        private void ShowNextGroup()
        {
            if (effectGroupsQueue.Count == 0)
            {
                popupRoot.SetActive(false);
                Action callback = onClosed;
                onClosed = null;
                callback?.Invoke();
                return;
            }
            
            List<PendingEffect> currentGroup = effectGroupsQueue.Dequeue();
            foreach (PendingEffect pendingEffect in currentGroup)
            {
                PendingEffects.Instance?.ApplyTriggeredEffect(pendingEffect);
            }
            
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
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Results of your previous choices has appeared:");
            builder.AppendLine();

            builder.AppendLine(group[0].triggerText);

            return builder.ToString().TrimEnd();
        }
    }
}

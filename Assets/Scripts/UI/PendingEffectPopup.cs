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
        
        private Queue<PendingEffect> effectQueue = new Queue<PendingEffect>();

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
            
            foreach (var effect in effects)
            {
                effectQueue.Enqueue(effect);
            }
            
            if (!wasAlreadyOpen)
            {
                ShowNextEffect();
            }
        }

        private void ShowNextEffect()
        {
            if (effectQueue.Count == 0)
            {
                popupRoot.SetActive(false);
                Action callback = onClosed;
                onClosed = null;
                callback?.Invoke();
                return;
            }
            
            PendingEffect currentEffect = effectQueue.Dequeue();
            PendingEffects.Instance?.ApplyTriggeredEffect(currentEffect);
            
            if (popupRoot != null && !popupRoot.activeSelf)
            {
                popupRoot.transform.DOKill();
                popupRoot.transform.localScale = Vector3.zero;
                popupRoot.SetActive(true);
                popupRoot.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
            
            if (bodyText != null) bodyText.gameObject.SetActive(true);
            
            string fullText = BuildEffectText(currentEffect);

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
            
            ShowNextEffect();
        }
        
        private string BuildEffectText(PendingEffect pendingEffect)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Results of your previous choices has appeared:");
            builder.AppendLine();

            builder.AppendLine(pendingEffect.triggerText);

            return builder.ToString().TrimEnd();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Stats;
using TMPro;
using UI.Utilities;
using UnityEngine;
using DG.Tweening;

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
            encounterUI.HideAllPanels();

            if (popupRoot != null)
            {
                popupRoot.transform.DOKill();
                popupRoot.transform.localScale = Vector3.zero;
                popupRoot.SetActive(true);
                popupRoot.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
            if (bodyText != null) bodyText.gameObject.SetActive(true);
            onClosed = closed;

            string fullText = BuildEffectText(effects);

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

            popupRoot.SetActive(false);
            Action callback = onClosed;
            onClosed = null;
            callback?.Invoke();
        }
        
        private string BuildEffectText(IReadOnlyList<PendingEffect> effects)
        {
            StringBuilder builder = new StringBuilder("Results of your previous choices has appeared:\n");
            foreach (PendingEffect pendingEffect in effects)
            {
                string sign = pendingEffect.effect.amount >= 0 ? "+" : string.Empty;
                builder.Append($"{pendingEffect.triggerText}").Append("\n• ")
                    .Append(GetStatName(pendingEffect.effect.type))
                    .Append(" ")
                    .Append(sign)
                    .Append(pendingEffect.effect.amount);
            }

            return builder.ToString();
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
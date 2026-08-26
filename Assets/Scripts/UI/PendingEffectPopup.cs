using System;
using System.Collections.Generic;
using System.Text;
using Stats;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class PendingEffectPopup : MonoBehaviour
    {
        public static PendingEffectPopup Instance;

        public EncounterUIManager encounterUI;

        public GameObject popupRoot;
        public TextMeshProUGUI bodyText;
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
        }

        public void Show(List<PendingEffect> effects, Action closed)
        {
            encounterUI.HideAllPanels();
            popupRoot.SetActive(true);
            bodyText.text = BuildEffectText(effects);
            onClosed = closed;
        }

        public void Close()
        {
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
using System;
using System.Collections.Generic;
using System.Text;
using Stats;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PendingEffectPopup : MonoBehaviour
    {
        public static PendingEffectPopup Instance;

        private EncounterUIManager encounterUI;

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
            
            encounterUI = GetComponent<EncounterUIManager>();
        }

        public void Show(List<StatEffect> effects, Action closed)
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
        
        private string BuildEffectText(IReadOnlyList<StatEffect> effects)
        {
            StringBuilder builder = new StringBuilder("Results of your previous choices has appeared:\n");
            foreach (StatEffect effect in effects)
            {
                string sign = effect.amount >= 0 ? "+" : string.Empty;
                builder.Append("\n• ")
                    .Append(GetStatName(effect.type))
                    .Append(" ")
                    .Append(sign)
                    .Append(effect.amount);
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
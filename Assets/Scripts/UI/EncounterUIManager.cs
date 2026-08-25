using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EncounterUIManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject encounterPanel;
        public GameObject resultPanel;
        public GameObject vendorNamePanel;

        [Header("Encounter Panel UI")]
        public TextMeshProUGUI vendorNameText;
        public TextMeshProUGUI dealBodyText;
    
        [Header("Result Panel UI")]
        public TextMeshProUGUI resultBodyText;

        private EncounterData currentEncounter;

        public void ShowEncounterPanel(EncounterData encounter)
        {
            currentEncounter = encounter;
            if (StatUIManager.Instance != null) StatUIManager.Instance.ClearPreview();

            resultPanel.SetActive(false);
            vendorNamePanel.SetActive(true);
            encounterPanel.SetActive(true);
        
            vendorNameText.text = currentEncounter.vendor.vendorName;
            dealBodyText.text = currentEncounter.saleText;
        }
        
        public void ShowResultPanel(string resultMessage)
        {
            if (StatUIManager.Instance != null) StatUIManager.Instance.ClearPreview();

            encounterPanel.SetActive(false);
            vendorNamePanel.SetActive(true);
            resultPanel.SetActive(true);
        
            resultBodyText.text = resultMessage;
        }

        public void HideAllPanels()
        {
            if (StatUIManager.Instance != null) StatUIManager.Instance.ClearPreview();

            vendorNamePanel.SetActive(false);
            encounterPanel.SetActive(false);
            resultPanel.SetActive(false);
        }

        public void OnAcceptHoverEnter()
        {
            if (currentEncounter != null && StatUIManager.Instance != null)
            {
                StatUIManager.Instance.ShowPreview(currentEncounter.GetClaimedEffects());
            }
        }

        public void OnRejectHoverEnter()
        {
            if (currentEncounter != null && StatUIManager.Instance != null)
            {
                StatUIManager.Instance.ShowPreview(currentEncounter.rejectedEffects);
            }
        }

        public void OnHoverExit()
        {
            if (StatUIManager.Instance != null)
            {
                StatUIManager.Instance.ClearPreview();
            }
        }
    }
}
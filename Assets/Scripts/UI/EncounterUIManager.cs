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

        public void ShowEncounterPanel(EncounterData currentEncounter)
        {
            resultPanel.SetActive(false);
            vendorNamePanel.SetActive(true);
            encounterPanel.SetActive(true);
        
            vendorNameText.text = currentEncounter.vendor.vendorName;
            dealBodyText.text = currentEncounter.saleText;
        }
        
        public void ShowResultPanel(string resultMessage)
        {
            encounterPanel.SetActive(false);
            vendorNamePanel.SetActive(true);
            resultPanel.SetActive(true);
        
            resultBodyText.text = resultMessage;
        }

        public void HideAllPanels()
        {
            vendorNamePanel.SetActive(false);
            encounterPanel.SetActive(false);
            resultPanel.SetActive(false);
        }
    }
}
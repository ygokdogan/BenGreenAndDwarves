using ScriptableObjects;
using TMPro;
using UI.Utilities;
using UnityEngine;
using DG.Tweening;

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
        public TypewriterEffect dealTypewriter;

        [Header("Result Panel UI")]
        public TextMeshProUGUI resultBodyText;
        public TypewriterEffect resultTypewriter;

        [Header("Swipeable Choice Cards")]
        public SwipeableCard acceptCard;
        public SwipeableCard declineCard;

        private EncounterData currentEncounter;

        private void Awake()
        {
            if (dealBodyText != null && dealTypewriter == null)
            {
                dealTypewriter = dealBodyText.GetComponent<TypewriterEffect>();
                if (dealTypewriter == null) dealTypewriter = dealBodyText.gameObject.AddComponent<TypewriterEffect>();
            }

            if (resultBodyText != null && resultTypewriter == null)
            {
                resultTypewriter = resultBodyText.GetComponent<TypewriterEffect>();
                if (resultTypewriter == null) resultTypewriter = resultBodyText.gameObject.AddComponent<TypewriterEffect>();
            }
        }

        public void ShowEncounterPanel(EncounterData encounter)
        {
            currentEncounter = encounter;
            if (HUDManager.Instance != null) HUDManager.Instance.ClearPreview();

            resultPanel.SetActive(false);

            AnimateScaleUp(vendorNamePanel, 0.35f);
            AnimateScaleUp(encounterPanel, 0.4f);

            if (declineCard != null) declineCard.AnimateInFromBottom(0.45f, 0.0f);
            if (acceptCard != null) acceptCard.AnimateInFromBottom(0.45f, 0.08f);

            if (dealBodyText != null) dealBodyText.gameObject.SetActive(true);

            vendorNameText.text = currentEncounter.vendor.vendorName;

            if (dealTypewriter != null)
            {
                dealTypewriter.Play(currentEncounter.offerText);
            }
            else if (dealBodyText != null)
            {
                dealBodyText.text = currentEncounter.offerText;
            }
        }

        public void ShowResultPanel(string resultMessage)
        {
            if (HUDManager.Instance != null) HUDManager.Instance.ClearPreview();

            encounterPanel.SetActive(false);
            vendorNamePanel.SetActive(false);

            AnimateScaleUp(resultPanel, 0.4f);

            if (resultBodyText != null) resultBodyText.gameObject.SetActive(true);

            if (resultTypewriter != null)
            {
                resultTypewriter.Play(resultMessage);
            }
            else if (resultBodyText != null)
            {
                resultBodyText.text = resultMessage;
            }
        }

        private void AnimateScaleUp(GameObject target, float duration = 0.35f)
        {
            target.transform.DOKill();
            target.transform.localScale = Vector3.zero;
            target.SetActive(true);
            target.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
        }

        public void SkipDealText()
        {
            if (dealTypewriter != null && dealTypewriter.IsTyping)
            {
                dealTypewriter.Skip();
            }
        }

        public void SkipResultText()
        {
            if (resultTypewriter != null && resultTypewriter.IsTyping)
            {
                resultTypewriter.Skip();
            }
        }

        public void HideAllPanels()
        {
            if (HUDManager.Instance != null) HUDManager.Instance.ClearPreview();

            vendorNamePanel.SetActive(false);
            encounterPanel.SetActive(false);
            resultPanel.SetActive(false);
        }

        public void OnOfferAccepted()
        {
            if (EncounterManager.Instance)
            {
                EncounterManager.Instance.AcceptOffer();
            }
        }
        
        public void OnOfferRejected()
        {
            if (EncounterManager.Instance)
            {
                EncounterManager.Instance.RejectOffer();
            }
        }

        public void OnAcceptHoverEnter()
        {
            if (currentEncounter != null && HUDManager.Instance != null)
            {
                HUDManager.Instance.ShowPreview(currentEncounter.GetClaimedEffects());
            }
        }

        public void OnRejectHoverEnter()
        {
            if (currentEncounter != null && HUDManager.Instance != null)
            {
                //HUDManager.Instance.ShowPreview(currentEncounter.rejectedEffects);
            }
        }

        public void OnHoverExit()
        {
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.ClearPreview();
            }
        }
    }
}
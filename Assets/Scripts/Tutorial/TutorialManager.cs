using System;
using DG.Tweening;
using TMPro;
using UI.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public Transform tutorialTextTransform;
        public TextMeshProUGUI tutorialText;
        public TypewriterEffect typewriter;

        public GameObject vendor;
        public GameObject background;
        public GameObject dayTimeHud;
        public GameObject statBars;
        public GameObject encounterPanel;
        public GameObject deckTransform;
        public GameObject settingsButton;
        public GameObject dayEndPanel;
        
        public Transform challengeTutorialPosition;
        public Transform dayEndTutorialPosition;

        private Vector3 initalPosition;
        
        [TextArea(4, 12)]
        public string[] tutorialTexts;

        private int i = 0;

        private void Start()
        {
            initalPosition = tutorialTextTransform.localPosition;
            i = 0;
            BeginTutorial();
        }

        //0
        private void BeginTutorial()
        {
            HideAllPanels();
            typewriter.Play(tutorialTexts[0]);
        }
        
        //1
        private void TimeHudTutorial()
        {
            dayTimeHud.SetActive(true);
        }

        //2
        private void StatBarsTutorial()
        {
            statBars.SetActive(true);

        }

        //3
        private void EncounterTutorial()
        {
            encounterPanel.SetActive(true);
            vendor.SetActive(true);
        }

        //4
        private void ChallengeTutorial()
        {
            HideAllPanels();
            tutorialTextTransform.DOLocalMove(challengeTutorialPosition.localPosition, .35f).SetEase(Ease.OutBounce);
            deckTransform.SetActive(true);
        }

        //5
        private void SettingsButtonTutorial()
        {
            settingsButton.SetActive(true);
        }
        
        //6
        private void DayEndPanelTutorial()
        {
            HideAllPanels();
            tutorialTextTransform.DOLocalMove(dayEndTutorialPosition.localPosition, .35f).SetEase(Ease.OutBounce);
            dayEndPanel.SetActive(true);
        }

        private void LastWordsTutorial()
        {
            HideAllPanels();
            tutorialTextTransform.DOLocalMove(initalPosition, .35f).SetEase(Ease.OutBounce);
        }

        private void EndTutorial()
        {
            PlayerPrefs.SetInt("SeenTutorial", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void OnContinueButtonTutorial()
        {
            if (typewriter.IsTyping)
            {
                typewriter.Skip();
                return;
            }
            else
            {
                i++;
                ShowNextTutorial();
                if(i < tutorialTexts.Length)
                    typewriter.Play(tutorialTexts[i]);
            }
        }

        private void ShowNextTutorial()
        {
            transform.DOKill();
            switch (i)
            {
                case 0: 
                    BeginTutorial();
                    break;
                case 1:
                    TimeHudTutorial();
                    break;
                case 2:
                    StatBarsTutorial();
                    break;
                case 3:
                    EncounterTutorial();
                    break;
                case 4:
                    ChallengeTutorial();
                    break;
                case 5:
                    SettingsButtonTutorial();
                    break;
                case 6:
                    DayEndPanelTutorial();
                    break;
                case 7: 
                    LastWordsTutorial();
                    break;
                case 8:
                    EndTutorial();
                    break;
                default:
                    EndTutorial();
                    break;
            }
        }

        private void HideAllPanels()
        {
            vendor.SetActive(false);
            background.SetActive(true);
            dayTimeHud.SetActive(false);
            statBars.SetActive(false);
            encounterPanel.SetActive(false);
            deckTransform.SetActive(false);
            settingsButton.SetActive(false);
            dayEndPanel.SetActive(false);
        }
    }
}

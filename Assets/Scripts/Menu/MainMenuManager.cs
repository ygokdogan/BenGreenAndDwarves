using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menu Panels")]
        public GameObject settingsPanel;
        public GameObject extrasPanel;

        private bool seenLore = false;
        private bool seenTutorial = false;

        private void Awake()
        {
            seenLore = PlayerPrefs.GetInt("SeenLoreScreen") == 1;
            seenTutorial = PlayerPrefs.GetInt("SeenTutorial") == 1;
        }

        private void Start()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (extrasPanel != null) extrasPanel.SetActive(false);
        }

        public void PlayGame()
        {
            if (!seenLore)
            {
                SceneManager.LoadScene("LoreScene");
                return;
            }

            if (!seenTutorial)
            {
                SceneManager.LoadScene("TutorialScene");
                return;
            } 
            
            SceneManager.LoadScene("GameplayScene");
        }

        public void QuitGame()
        {
            Application.Quit(); 
        }

        public void OpenSettings()
        {
            settingsPanel.SetActive(true); 
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false); 
        }

        public void OpenExtras()
        {
            if (extrasPanel != null) extrasPanel.SetActive(true);
        }

        public void CloseExtras()
        {
            if (extrasPanel != null) extrasPanel.SetActive(false);
        }

        public void ReplayTutorial()
        {
            SceneManager.LoadScene("TutorialScene");
        }

        public void ReplayLore()
        {
            PlayerPrefs.SetInt("ReplayLoreFromExtras", 1);
            SceneManager.LoadScene("LoreScene");
        }
        
    }
}

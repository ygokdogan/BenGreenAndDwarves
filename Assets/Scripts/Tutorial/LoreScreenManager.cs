using System;
using TMPro;
using UI.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial
{
    public class LoreScreenManager : MonoBehaviour
    {
        [Header("Typewriter")]
        public TypewriterEffect writer;
        private string lore;
        
        [Header("Button")]
        public Button continueButton;

        private bool replayingFromExtras;
        
        private void Start()
        {
            replayingFromExtras = PlayerPrefs.GetInt("ReplayLoreFromExtras") == 1;
            PlayerPrefs.DeleteKey("ReplayLoreFromExtras");

            lore = writer.GetComponent<TextMeshProUGUI>().text;
            continueButton.gameObject.SetActive(false);
            writer.Play(lore, null, SetContinueButtonActive);
            PlayerPrefs.SetInt("SeenLoreScreen", 1);
        }

        public void LoadNextScene()
        {
            if (replayingFromExtras)
            {
                SceneManager.LoadScene("MenuScene");
                return;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void SetContinueButtonActive()
        {
            continueButton.gameObject.SetActive(true);
        }
    }
}

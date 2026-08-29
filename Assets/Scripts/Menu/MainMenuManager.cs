using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menü Panelleri")]
        public GameObject settingsPanel;
        
        private void Start()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(1);
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
    }
}
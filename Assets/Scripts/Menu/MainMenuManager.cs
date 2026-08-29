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

        [Header("Ayarlar UI")]
        public Slider volumeSlider;
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;

        private Resolution[] resolutions;

        private void Start()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
            
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResIndex;
            resolutionDropdown.RefreshShownValue();
        
            fullscreenToggle.isOn = Screen.fullScreen;
            volumeSlider.value = AudioListener.volume; // Oyunun ana ses seviyesini çeker (0 ile 1 arasıdır)
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution res = resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
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
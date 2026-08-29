using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class SettingsManager : MonoBehaviour
    {
        private const string MasterVolumeKey = "Settings.MasterVolume";
        private const string MusicVolumeKey = "Settings.MusicVolume";
        private const string SfxVolumeKey = "Settings.SfxVolume";
        private const string FullscreenKey = "Settings.Fullscreen";
        private const string ResolutionWidthKey = "Settings.ResolutionWidth";
        private const string ResolutionHeightKey = "Settings.ResolutionHeight";

        [Header("Audio UI")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Display UI")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private readonly List<Resolution> resolutions = new();

        private void Start()
        {
            InitializeResolutionDropdown();
            LoadSettings();
        }

        public void SetMasterVolume(float volume)
        {
            float clampedVolume = Mathf.Clamp01(volume);
            AudioListener.volume = clampedVolume;
            PlayerPrefs.SetFloat(MasterVolumeKey, clampedVolume);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float volume)
        {
            float clampedVolume = Mathf.Clamp01(volume);

            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.volume = clampedVolume;

            PlayerPrefs.SetFloat(MusicVolumeKey, clampedVolume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float volume)
        {
            float clampedVolume = Mathf.Clamp01(volume);

            if (AudioManager.Instance != null)
            {
                if (AudioManager.Instance.uiSource != null)
                    AudioManager.Instance.uiSource.volume = clampedVolume;

                if (AudioManager.Instance.challengeFX != null)
                    AudioManager.Instance.challengeFX.volume = clampedVolume;
            }

            PlayerPrefs.SetFloat(SfxVolumeKey, clampedVolume);
            PlayerPrefs.Save();
        }

        public void SetFullscreen(bool isFullscreen)
        {
            ApplyResolution(Screen.width, Screen.height, isFullscreen);
            PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex < 0 || resolutionIndex >= resolutions.Count)
                return;

            Resolution resolution = resolutions[resolutionIndex];
            ApplyResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt(ResolutionWidthKey, resolution.width);
            PlayerPrefs.SetInt(ResolutionHeightKey, resolution.height);
            PlayerPrefs.Save();
        }

        private void InitializeResolutionDropdown()
        {
            if (resolutionDropdown == null)
                return;

            resolutions.Clear();
            HashSet<string> uniqueResolutions = new();

            foreach (Resolution resolution in Screen.resolutions)
            {
                string key = $"{resolution.width}x{resolution.height}";
                if (uniqueResolutions.Add(key))
                    resolutions.Add(resolution);
            }

            if (resolutions.Count == 0)
            {
                resolutions.Add(new Resolution
                {
                    width = Screen.width,
                    height = Screen.height
                });
            }

            List<string> options = new List<string>();
            foreach (Resolution resolution in resolutions)
            {
                options.Add($"{resolution.width} x {resolution.height}");
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
        }

        private void LoadSettings()
        {
            float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, .5f);
            float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSfxVolume(sfxVolume);

            int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
            int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);
            ApplyResolution(savedWidth, savedHeight, fullscreen);

            if (masterVolumeSlider != null)
                masterVolumeSlider.SetValueWithoutNotify(masterVolume);

            if (musicVolumeSlider != null)
                musicVolumeSlider.SetValueWithoutNotify(musicVolume);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);

            if (fullscreenToggle != null)
                fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

            if (resolutionDropdown != null)
                resolutionDropdown.SetValueWithoutNotify(FindResolutionIndex(savedWidth, savedHeight));
        }

        private int FindResolutionIndex(int width, int height)
        {
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == width && resolutions[i].height == height)
                    return i;
            }

            return 0;
        }

        private static void ApplyResolution(int width, int height, bool fullscreen)
        {
            Screen.SetResolution(width, height, fullscreen);
        }
    }
}

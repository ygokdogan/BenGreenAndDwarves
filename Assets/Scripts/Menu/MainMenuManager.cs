using UnityEngine;
using UnityEngine.UI; // Slider ve Toggle için
using TMPro; // Dropdown (Açılır menü) için
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Listeler için

public class MainMenuManager : MonoBehaviour
{
    [Header("Menü Panelleri")]
    public GameObject settingsPanel;

    [Header("Ayarlar UI")]
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions; // Bilgisayarın desteklediği çözünürlükleri tutacak liste

    private void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 1. Çözünürlük Ayarlarını Yükle
        resolutions = Screen.resolutions; // Bilgisayarın tüm çözünürlüklerini al
        resolutionDropdown.ClearOptions(); // Dropdown'un içindeki eski yazıları temizle

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Genişlik x Yükseklik şeklinde metin oluştur (Örn: 1920 x 1080)
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Mevcut ekran çözünürlüğünü bul
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options); // Listeyi menüye ekle
        resolutionDropdown.value = currentResIndex; // Mevcut çözünürlüğü seçili yap
        resolutionDropdown.RefreshShownValue();

        // 2. Tam Ekran ve Ses Ayarlarının Başlangıç Değerlerini Yükle
        fullscreenToggle.isOn = Screen.fullScreen;
        volumeSlider.value = AudioListener.volume; // Oyunun ana ses seviyesini çeker (0 ile 1 arasıdır)
    }

    // --- AYARLAR FONKSİYONLARI ---

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Oyunun genel sesini ayarlar
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen; // Tam ekranı aç/kapat
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen); // Seçilen çözünürlüğü uygula
    }

    // --- MENÜ BUTON FONKSİYONLARI ---

    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkıldı!"); 
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
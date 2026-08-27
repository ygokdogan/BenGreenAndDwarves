using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource audioSource;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // Buton tıklama event'ine dinleyici ekliyoruz
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDisable()
    {
        // Obje kapandığında dinleyiciyi kaldırıyoruz (memory leak olmaması için)
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            // Sesi üst üste binecek şekilde (kesilmeden) oynatır
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("UIButtonSound: AudioClip veya AudioSource eksik!", this);
        }
    }
}
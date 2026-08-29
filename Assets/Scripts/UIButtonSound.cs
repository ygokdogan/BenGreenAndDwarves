using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip clickSound;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISFX(clickSound);
        }
        else
        {
            Debug.LogWarning("UIButtonSound: AudioClip veya AudioSource eksik!", this);
        }
    }
}
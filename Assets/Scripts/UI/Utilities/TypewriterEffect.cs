using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.Utilities
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float charactersPerSecond = 40f;
        [SerializeField] private bool playOnEnable = false;

        [Header("Auto Sizing")]
        [SerializeField] private bool autoSizeText = true;
        [SerializeField] private float minFontSize = 18f;
        [SerializeField] private float maxFontSize = 72f;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typingSound;
        [SerializeField] private int playSoundEveryXChars = 2;

        private TMP_Text targetText;
        private Coroutine typingCoroutine;
        private Action onCompleteCallback;
        private string fullText = string.Empty;

        public bool IsTyping { get; private set; }

        private void Awake()
        {
            targetText = GetComponent<TMP_Text>();
            ApplyAutoSizing();
        }

        private void OnEnable()
        {
            if (playOnEnable && targetText != null && !string.IsNullOrEmpty(targetText.text))
            {
                Play(targetText.text, charactersPerSecond);
            }
        }

        private void OnDisable()
        {
            StopTyping();
        }

        private void ApplyAutoSizing()
        {
            if (targetText != null && autoSizeText)
            {
                targetText.enableAutoSizing = true;
                targetText.fontSizeMin = minFontSize;
                targetText.fontSizeMax = maxFontSize;
            }
        }

        public void Play(string text, float? customSpeed = null, Action onComplete = null)
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
                ApplyAutoSizing();
            }

            StopTyping();

            fullText = text ?? string.Empty;
            targetText.text = fullText;
            
            // Metni atadıktan sonra mesh'i güncelleyerek TextMeshPro'nun yeni font boyutunu hesaplamasını sağlıyoruz.
            targetText.ForceMeshUpdate();

            onCompleteCallback = onComplete;
            float speed = customSpeed ?? charactersPerSecond;

            if (!gameObject.activeInHierarchy || !enabled || speed <= 0)
            {
                targetText.maxVisibleCharacters = targetText.textInfo.characterCount;
                IsTyping = false;
                Action cb = onCompleteCallback;
                onCompleteCallback = null;
                cb?.Invoke();
                return;
            }

            typingCoroutine = StartCoroutine(TypeRoutine(speed));
        }

        private IEnumerator TypeRoutine(float speed)
        {
            IsTyping = true;
            int totalCharacters = targetText.textInfo.characterCount;
            targetText.maxVisibleCharacters = 0;

            float delay = 1f / speed;
            int charCount = 0;

            while (charCount < totalCharacters)
            {
                charCount++;
                targetText.maxVisibleCharacters = charCount;

                if (audioSource != null && typingSound != null && (charCount % playSoundEveryXChars == 0))
                {
                    audioSource.PlayOneShot(typingSound);
                }

                yield return new WaitForSeconds(delay);
            }

            targetText.maxVisibleCharacters = totalCharacters;
            IsTyping = false;
            
            Action cb = onCompleteCallback;
            onCompleteCallback = null;
            cb?.Invoke();
        }

        public void Skip()
        {
            if (!IsTyping) return;

            StopTyping();

            if (targetText != null)
            {
                targetText.ForceMeshUpdate();
                targetText.maxVisibleCharacters = targetText.textInfo.characterCount;
            }

            IsTyping = false;
            Action cb = onCompleteCallback;
            onCompleteCallback = null;
            cb?.Invoke();
        }

        public void StopTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            IsTyping = false;
        }
    }
}
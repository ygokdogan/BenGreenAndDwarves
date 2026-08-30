using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource uiSource;
    public AudioSource challengeFX;

    public AudioSource musicSource;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField, Min(0f)] private float musicFadeDuration = 0.6f;

    private Coroutine musicFadeRoutine;
    
    private const string MusicVolumeKey = "Settings.MusicVolume";
    private const string SfxVolumeKey = "Settings.SfxVolume";
    
    public float TargetMusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    public float TargetSfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
    }

    private void EnsureAudioSources()
    {
        if (uiSource == null)
            uiSource = CreateAudioSource("UI Source");

        if (challengeFX == null)
            challengeFX = CreateAudioSource("Challenge SFX");

        if (musicSource == null)
            musicSource = CreateAudioSource("Music Source");

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        
        musicSource.volume = TargetMusicVolume;
        uiSource.volume = TargetSfxVolume;
        challengeFX.volume = TargetSfxVolume;
    }

    private AudioSource CreateAudioSource(string sourceName)
    {
        GameObject sourceObject = new GameObject(sourceName);
        sourceObject.transform.SetParent(transform);
        return sourceObject.AddComponent<AudioSource>();
    }

    private void PlayMusicForScene(Scene scene, bool instantCut = false)
    {
        PlayMusic(scene.buildIndex >= 2 ? gameplayMusic : mainMenuMusic, instantCut);
    }

    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlayMusic(AudioClip clip, bool instantCut = false)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            musicSource.volume = TargetMusicVolume; 
            return;
        }

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(FadeToMusic(clip, instantCut));
    }

    public void PlayMusicInstantly(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);
        
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = TargetMusicVolume;
        musicSource.loop = false;
        musicSource.Play();
    }

    private IEnumerator FadeToMusic(AudioClip clip, bool instantCut)
    {
        float targetVolume = TargetMusicVolume;

        if (!instantCut && musicSource.isPlaying && musicFadeDuration > 0f)
            yield return FadeMusicVolume(musicSource.volume, 0f);

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicFadeDuration > 0f ? 0f : targetVolume;
        musicSource.loop = true;
        musicSource.Play();

        if (musicFadeDuration > 0f)
            yield return FadeMusicVolume(0f, targetVolume);

        musicFadeRoutine = null;
    }

    private IEnumerator FadeMusicVolume(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(from, to, elapsed / musicFadeDuration);
            yield return null;
        }

        musicSource.volume = to;
    }

    public void PlayUISFX(AudioClip clip)
    {
        if (uiSource != null && clip != null)
            uiSource.PlayOneShot(clip);
    }

    public void PlayChallengeSFX(AudioClip clip)
    {
        if (challengeFX == null || clip == null) return;
        challengeFX.PlayOneShot(clip);
    }

    public void StopChallengeSFX()
    {
        if (challengeFX != null)
            challengeFX.Stop();
    }
 }

using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;
using UnityEngine.EventSystems;
using System.Collections;

public class AudioManager : MonoBehaviour, IPointerEnterHandler
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public static AudioManager Instance;

    [Header("Music Tracks")]
    public AudioClip startScreenMusic;
    public AudioClip farmMusic;
    public AudioClip battleMusic;
    public AudioClip endMusic;   // optional if needed

    [Header("Sound Effects")]
    public AudioClip itemPickupSFX;
    public AudioClip plantSeedSFX;
    public AudioClip clickHoverSFX;
    public AudioClip mousePressSFX;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Log.Info($"Scene changed → {newScene.name}");
        PlayMusicForScene(newScene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        // TITLE SCREEN
        if (sceneName == "TitleScreen")
        {
            clipToPlay = startScreenMusic;
        }
        // MAIN GAME
        else if (sceneName == "Game")
        {
            clipToPlay = farmMusic;
        }
        // BATTLE NIGHT SCENE
        else if (sceneName == "Battle")
        {
            clipToPlay = battleMusic;
        }
        // END SCENE
        else if (sceneName == "End")
        {
            clipToPlay = endMusic != null ? endMusic : farmMusic; // fallback
        }

        // If no music for this scene → stop
        if (clipToPlay == null)
        {
            FadeToStop();
            return;
        }

        // Don't restart if the same music is already playing
        if (musicSource.clip == clipToPlay)
            return;

        FadeToNewTrack(clipToPlay);
    }

    // Fade between tracks smoothly
    private void FadeToNewTrack(AudioClip newClip)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeMusicRoutine(newClip));
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip)
    {
        float fadeTime = 0.5f;

        // Fade out
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = 1 - (t / fadeTime);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = t / fadeTime;
            yield return null;
        }

        musicSource.volume = 1f;
    }

    private void FadeToStop()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        float fadeTime = 0.5f;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicSource.volume = 1 - (t / fadeTime);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = null;
    }

    // SOUND EFFECTS
    public void PlayMousePressSFX() => sfxSource.PlayOneShot(mousePressSFX);
    public void PlayClickHoverSFX() => sfxSource.PlayOneShot(clickHoverSFX);
    public void PlayItemPickupSFX() => sfxSource.PlayOneShot(itemPickupSFX);
    public void OnPointerEnter(PointerEventData eventData) => PlayClickHoverSFX();
}
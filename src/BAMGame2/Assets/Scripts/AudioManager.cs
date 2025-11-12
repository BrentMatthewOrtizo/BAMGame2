using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour, IPointerEnterHandler
{

    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public static AudioManager Instance;

    [Header("Music Tracks")] public AudioClip startScreenMusic;
    public AudioClip farmMusic;

    //implement later
    [Header("Sound Effects")] public AudioClip itemPickupSFX;
    public AudioClip plantSeedSFX;
    public AudioClip clickHoverSFX;
    public AudioClip mousePressSFX;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    

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
        Log.Info($"Scene changed to {newScene.name}");
        PlayMusicForScene(newScene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName.Contains("TitleScreen"))
        {
            clipToPlay = startScreenMusic;
            Log.Info($"Playing {clipToPlay}");
        }
        else if (sceneName.Contains("Game"))
        {
            clipToPlay = farmMusic;
            Log.Info($"Playing {clipToPlay}");
        }

        if (clipToPlay != null)
        {
            if (musicSource.clip != clipToPlay)
            {
                musicSource.clip = clipToPlay;
                musicSource.Play();
            }
        }
        else
        {
            musicSource.Stop();
        }
    }
    
    // SOUND EFFECTS
    public void PlayMousePressSFX()
    {
        sfxSource.PlayOneShot(mousePressSFX);
    }
    
    public void PlayClickHoverSFX()
    {
        sfxSource.PlayOneShot(clickHoverSFX);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayClickHoverSFX();
    }

    public void PlayItemPickupSFX()
    {
        sfxSource.PlayOneShot(itemPickupSFX);
    }
}
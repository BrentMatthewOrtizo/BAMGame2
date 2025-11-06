using UnityEngine;
using UnityEngine.SceneManagement;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class AudioManager : MonoBehaviour
{

    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    public static AudioManager Instance;

    [Header("Music Tracks")] public AudioClip startScreenMusic;
    public AudioClip farmMusic;

    //implement later
    [Header("Sound Effects")] public AudioClip itemPickupSFX;
    public AudioClip plantSeedSFX;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    

    private void Awake()
    {
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
        }
        else if (sceneName.Contains("Game"))
        {
            clipToPlay = farmMusic;
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
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Play immediately when entering the scene
        audioSource.Play();
    }

    private void OnDisable()
    {
        if (audioSource != null && gameObject.scene.isLoaded)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset for next time
    }
}
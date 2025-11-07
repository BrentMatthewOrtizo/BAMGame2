using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource uiAudioSource; // assign in Inspector

    [Header("Optional: Custom Clip Override")]
    [SerializeField] private AudioClip customClip;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (uiAudioSource == null)
        {
            Debug.LogWarning($"[UIButtonSFX] No AudioSource assigned for {gameObject.name}!");
            return;
        }

        AudioClip clipToPlay = customClip != null ? customClip : uiAudioSource.clip;
        uiAudioSource.PlayOneShot(clipToPlay);
    }
}
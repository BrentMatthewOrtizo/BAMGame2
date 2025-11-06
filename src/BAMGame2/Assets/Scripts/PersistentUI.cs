using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PersistantUI : MonoBehaviour
{
    private static PersistantUI instance;
    private GameObject pauseButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this Canvas across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates if you revisit a scene
        }
        pauseButton = GameObject.FindWithTag("PauseButton");
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByBuildIndex(0).buildIndex)
        {
            
            pauseButton.SetActive(false);
        }
        else
        {
            pauseButton.SetActive(true);
        }
    }
    
}
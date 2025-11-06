using UnityEngine;
using UnityEngine.SceneManagement;
public class PersistantUI : MonoBehaviour
{
    private static PersistantUI instance;

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
    }
    
}
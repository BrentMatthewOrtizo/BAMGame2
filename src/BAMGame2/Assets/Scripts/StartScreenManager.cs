using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        // AudioManager.Instance.PlayMousePressSFX();
        SceneManager.LoadScene(1);
    }

}

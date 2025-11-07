using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

}

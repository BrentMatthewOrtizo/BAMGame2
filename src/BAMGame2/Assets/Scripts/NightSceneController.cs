using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightSceneController : MonoBehaviour
{
    public float nightDuration = 30f;
    public TextMeshProUGUI timerText;

    private void Start() => StartCoroutine(NightTimer());

    private IEnumerator NightTimer()
    {
        float t = nightDuration;
        while (t > 0)
        {
            timerText.text = $"Night ends in: {Mathf.CeilToInt(t)}s";
            t -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Game");
    }
}
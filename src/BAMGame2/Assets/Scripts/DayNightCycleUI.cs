using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycleUI : MonoBehaviour
{
    [Header("Durations")]
    public float dayDuration = 180f;
    public float nightDuration = 30f;

    [Header("UI")]
    public TextMeshProUGUI phaseLabel;
    public TextMeshProUGUI timerLabel;

    public bool IsDay { get; private set; } = true;
    private float timeLeft;

    private void Start()
    {
        StartDay();
        StartCoroutine(CycleLoop());
    }

    private IEnumerator CycleLoop()
    {
        while (true)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimer(timeLeft);

            if (timeLeft <= 0)
            {
                if (IsDay)
                {
                    SaveBeforeTransition();
                    SceneManager.LoadScene("Battle");
                    yield break;
                }
                else
                {
                    SceneManager.LoadScene("Game");
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void StartDay()
    {
        IsDay = true;
        timeLeft = dayDuration;
        phaseLabel.text = "Day";
        UpdateTimer(timeLeft);
    }

    private void UpdateTimer(float t)
    {
        int m = Mathf.FloorToInt(t / 60);
        int s = Mathf.FloorToInt(t % 60);
        timerLabel.text = $"Timer: {m:0}:{s:00}";
    }

    private void SaveBeforeTransition()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player && GameStateManager.Instance)
            GameStateManager.Instance.SavePlayer(player.transform.position);
    }
}
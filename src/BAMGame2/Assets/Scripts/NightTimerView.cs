using Game.Runtime;
using TMPro;
using UnityEngine;
using Game399.Shared.Models;

public class NightTimerView : ObserverMonoBehaviour
{
    private static WorldStateModel World => ServiceResolver.Resolve<WorldStateModel>();

    [SerializeField] private TextMeshProUGUI timerText;

    protected override void Subscribe()
    {
        World.TimeLeft.ChangeEvent += OnTimeChanged;
        World.IsDay.ChangeEvent += OnPhaseChanged;
    }

    protected override void Unsubscribe()
    {
        World.TimeLeft.ChangeEvent -= OnTimeChanged;
        World.IsDay.ChangeEvent -= OnPhaseChanged;
    }

    private void OnPhaseChanged(bool isDay)
    {
        if (!isDay)
            UpdateNightUI(World.TimeLeft.Value);
    }

    private void OnTimeChanged(float time)
    {
        if (!World.IsDay.Value)
            UpdateNightUI(time);
    }

    private void UpdateNightUI(float time)
    {
        timerText.text = $"Night ends in: {Mathf.CeilToInt(time)}s";
    }
}
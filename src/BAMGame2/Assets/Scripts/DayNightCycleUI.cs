using Game399.Shared.Models;
using Game399.Shared.Services;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycleUI : ObserverMonoBehaviour
{
    [Header("Durations")]
    public float dayDuration = 180f;

    [Header("UI")]
    public TextMeshProUGUI phaseLabel;
    public TextMeshProUGUI timerLabel;

    private IWorldStateService _worldService;
    private WorldStateModel _world;
    private bool _transitionTriggered;

    // Convenience wrappers if anything else reads these
    public bool IsDay => _world?.IsDay.Value ?? true;
    public float TimeLeft => _world?.TimeLeft.Value ?? 0f;
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();

    protected override void Awake()
    {
        base.Awake();
        _worldService = ServiceResolver.Resolve<IWorldStateService>();
        _world = _worldService.World;
    }

    protected override void Start()
    {
        base.Start();

        // When this UI is in the Game scene, start the day phase
        if (SceneManager.GetActiveScene().name == "Game")
        {
            _worldService.StartDay(dayDuration);
        }
    }

    protected override void Subscribe()
    {
        if (_world == null) return;

        _world.IsDay.ChangeEvent += OnPhaseChanged;
        _world.TimeLeft.ChangeEvent += OnTimeChanged;
    }

    protected override void Unsubscribe()
    {
        if (_world == null) return;

        _world.IsDay.ChangeEvent -= OnPhaseChanged;
        _world.TimeLeft.ChangeEvent -= OnTimeChanged;
    }

    private void Update()
    {
        if (_worldService == null || _transitionTriggered)
            return;

        // Domain tick
        _worldService.Tick(Time.deltaTime);

        // Handle transition once when timer hits zero
        if (_world.TimeLeft.Value <= 0.01f)
        {
            _transitionTriggered = true;

            if (_world.IsDay.Value)
            {
                SavePlayerAndCrops();
                SceneManager.LoadScene("Battle");
            }
            else
            {
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void OnPhaseChanged(bool isDay)
    {
        if (phaseLabel != null)
            phaseLabel.text = isDay ? "Day" : "Night";
    }

    private void OnTimeChanged(float t)
    {
        if (timerLabel == null) return;

        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timerLabel.text = $"Timer: {m:0}:{s:00}";
    }

    private void SavePlayerAndCrops()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player && GameStateManager.Instance)
        {
            GameStateManager.Instance.SavePlayer(player.transform.position);
            Log.Info($"Saved player position: {player.transform.position}");
        }
        else
        {
            Log.Warn("Could not find Player or GameStateManager when saving position.");
        }

        if (FarmManager.Instance != null)
        {
            Log.Info("Saving crops before night transition...");
            FarmManager.Instance.SaveCrops();
        }
        else
        {
            Log.Warn("FarmManager not found when saving crops.");
        }
    }
}
using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    public GameObject pauseMenuPanel;
    public static bool IsGamePaused { get; private set; } = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pauseMenuPanel.SetActive(!pauseMenuPanel.activeSelf);
            InventoryManager.Instance.hotbarPanel.SetActive(!InventoryManager.Instance.hotbarPanel.activeSelf);
            InventoryManager.Instance.inventoryPanel.SetActive(false);
            InventoryManager.Instance.inventoryTab.SetActive(false);
            SetPause(pauseMenuPanel.activeSelf);
            Log.Info($"IsGamePaused set to {pauseMenuPanel.activeSelf}");
            AudioManager.Instance.PlayMousePressSFX();
        }
        
    }

    public static void SetPause(bool pause)
    {
        IsGamePaused = pause;
    }

    public void Interact()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }
        pauseMenuPanel.SetActive(!pauseMenuPanel.activeSelf);
        InventoryManager.Instance.hotbarPanel.SetActive(!InventoryManager.Instance.hotbarPanel.activeSelf);
        InventoryManager.Instance.inventoryPanel.SetActive(false);
        InventoryManager.Instance.inventoryTab.SetActive(false);
        SetPause(pauseMenuPanel.activeSelf);
        Log.Info($"IsGamePaused set to {pauseMenuPanel.activeSelf}");
    }

    public void Exit()
    {
        SceneManager.LoadSceneAsync(0);
        pauseMenuPanel.SetActive(false);
        SetPause(false);
        Log.Info("IsGamePaused set to false");
    }
}

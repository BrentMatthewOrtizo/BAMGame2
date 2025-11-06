using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenShop()
    {
        Debug.Log("[ShopManager] Shop UI opened (placeholder).");
        // TODO: When making the UI, enable a shop panel here
    }

    public void CloseShop()
    {
        Debug.Log("[ShopManager] Shop UI closed.");
    }
}
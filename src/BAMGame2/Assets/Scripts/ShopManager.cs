using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject shopCanvas;
    public Image animalImage;
    public TMP_Text statsText;
    public TMP_Text goldText;
    public Button buyButton;
    public TMP_Text popupText;
    public Button exitShopButton;

    [Header("Animal Buttons")]
    public Button chickenButton;
    public Button cowButton;
    public Button pigButton;
    public Button duckButton;
    
    [Header("Animal Sprites")]
    public Sprite chickenSprite;
    public Sprite cowSprite;
    public Sprite pigSprite;
    public Sprite duckSprite;

    private PlayerWallet wallet;
    private AnimalData selectedAnimal;
    private Dictionary<string, AnimalData> animals = new();

    private bool shopOpen = false;
    private DayNightCycleUI dayNightCycle;

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

    private void Start()
    {
        wallet = FindObjectOfType<PlayerWallet>();
        dayNightCycle = FindObjectOfType<DayNightCycleUI>();

        if (wallet == null)
        {
            Debug.LogError("[ShopManager] No PlayerWallet found in scene.");
            return;
        }

        // Define animals
        animals["Chicken"] = new AnimalData("Chicken", 10, 3, 3);
        animals["Cow"] = new AnimalData("Cow", 20, 8, 2);
        animals["Pig"] = new AnimalData("Pig", 15, 5, 3);
        animals["duck"] = new AnimalData("duck", 50, 10, 10);

        // Hook buttons
        chickenButton.onClick.AddListener(() => SelectAnimal("Chicken"));
        cowButton.onClick.AddListener(() => SelectAnimal("Cow"));
        pigButton.onClick.AddListener(() => SelectAnimal("Pig"));
        duckButton.onClick.AddListener(() => SelectAnimal("duck"));
        buyButton.onClick.AddListener(BuyAnimal);
        exitShopButton.onClick.AddListener(CloseShop);
        exitShopButton.gameObject.SetActive(false);

        wallet.OnGoldChanged += UpdateGoldDisplay;
        UpdateGoldDisplay(wallet.gold);

        shopCanvas.SetActive(false);
    }

    private void Update()
    {
        // Close the shop automatically when the day timer ends
        if (shopOpen && dayNightCycle != null && dayNightCycle.IsDay && dayNightCycle.timeLeft <= 0f)
        {
            Debug.Log("[ShopManager] Timer reached 0 — closing shop.");
            CloseShop();
        }
    }

    public void ToggleShop()
    {
        if (shopOpen)
            CloseShop();
        else
            OpenShop();
    }

    public void OpenShop()
    {
        shopCanvas.SetActive(true);
        UpdateGoldDisplay(wallet.gold);
        shopOpen = true;
        
        SetLeftPanelVisible(false);
        exitShopButton.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        shopCanvas.SetActive(false);
        shopOpen = false;
        exitShopButton.gameObject.SetActive(false);
    }

    private void SelectAnimal(string name)
    {
        selectedAnimal = animals[name];
        statsText.text = $"HP: {selectedAnimal.hp}\nDamage: {selectedAnimal.damage}\nCost: {selectedAnimal.cost} gold";
        buyButton.interactable = !selectedAnimal.isOwned;

        // Update animal image based on selection
        switch (name)
        {
            case "Chicken":
                animalImage.sprite = chickenSprite;
                break;
            case "Cow":
                animalImage.sprite = cowSprite;
                break;
            case "Pig":
                animalImage.sprite = pigSprite;
                break;
            case "duck":
                animalImage.sprite = duckSprite;
                break;
            default:
                animalImage.sprite = null;
                break;
        }

        // Make sure image is visible if it was hidden before
        animalImage.enabled = true;
        SetLeftPanelVisible(true);
    }

    private void BuyAnimal()
    {
        if (selectedAnimal == null) return;

        if (selectedAnimal.isOwned)
        {
            ShowPopup($"{selectedAnimal.name} already owned.");
            return;
        }

        if (wallet.gold < selectedAnimal.cost)
        {
            ShowPopup("Not enough gold!");
            return;
        }

        wallet.SpendGold(selectedAnimal.cost);
        selectedAnimal.isOwned = true;
        buyButton.interactable = false;
        ShowPopup($"{selectedAnimal.name} added to farm team!");
    }

    private void UpdateGoldDisplay(int amount)
    {
        goldText.text = $"Gold: {amount}";
    }

    private void ShowPopup(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowPopupCoroutine(message));
    }

    private System.Collections.IEnumerator ShowPopupCoroutine(string message)
    {
        popupText.text = message;
        popupText.alpha = 1f;
        yield return new WaitForSecondsRealtime(1.5f);
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime)
        {
            popupText.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        popupText.alpha = 0;
    }

    [System.Serializable]
    public class AnimalData
    {
        public string name;
        public int cost;
        public int hp;
        public int damage;
        public bool isOwned;

        public AnimalData(string n, int c, int h, int d)
        {
            name = n; cost = c; hp = h; damage = d;
            isOwned = false;
        }
    }

    // New Input System hook for "E" or Interact
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (shopOpen)
            CloseShop();
    }

    // Public property for clean timer access
    public bool IsShopOpen => shopOpen;
    
    private void SetLeftPanelVisible(bool visible)
    {
        animalImage.gameObject.SetActive(visible);
        statsText.gameObject.SetActive(visible);
        buyButton.gameObject.SetActive(visible);
    }
}
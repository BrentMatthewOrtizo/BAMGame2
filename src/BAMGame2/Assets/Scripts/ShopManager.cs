using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Animal Buttons")]
    public Button chickenButton;
    public Button cowButton;
    public Button pigButton;
    public Button kapybaraButton;

    private PlayerWallet wallet;
    private AnimalData selectedAnimal;
    private Dictionary<string, AnimalData> animals = new();

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
        if (wallet == null)
        {
            Debug.LogError("[ShopManager] No PlayerWallet found in scene.");
            return;
        }

        // Define animals
        animals["Chicken"]  = new AnimalData("Chicken", 10, 3, 3);
        animals["Cow"]      = new AnimalData("Cow", 20, 8, 2);
        animals["Pig"]      = new AnimalData("Pig", 15, 5, 3);
        animals["Kapybara"] = new AnimalData("Kapybara", 50, 10, 10);

        // Hook buttons
        chickenButton.onClick.AddListener(() => SelectAnimal("Chicken"));
        cowButton.onClick.AddListener(() => SelectAnimal("Cow"));
        pigButton.onClick.AddListener(() => SelectAnimal("Pig"));
        kapybaraButton.onClick.AddListener(() => SelectAnimal("Kapybara"));
        buyButton.onClick.AddListener(BuyAnimal);

        wallet.OnGoldChanged += UpdateGoldDisplay;
        UpdateGoldDisplay(wallet.gold);

        shopCanvas.SetActive(false);
    }

    public void OpenShop()
    {
        shopCanvas.SetActive(true);
        UpdateGoldDisplay(wallet.gold);
        Time.timeScale = 0f; // Pause world
    }

    public void CloseShop()
    {
        shopCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    private void SelectAnimal(string name)
    {
        selectedAnimal = animals[name];
        statsText.text = $"HP: {selectedAnimal.hp}\nDamage: {selectedAnimal.damage}\nCost: {selectedAnimal.cost} gold";
        buyButton.interactable = !selectedAnimal.isOwned;
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
}
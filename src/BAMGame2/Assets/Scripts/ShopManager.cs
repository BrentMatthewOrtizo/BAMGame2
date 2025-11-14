using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Prefab")]
    public GameObject shopCanvasPrefab;

    // Runtime UI references
    private GameObject shopCanvas;
    private Image animalImage;
    private TMP_Text statsText;
    private TMP_Text goldText;
    private Button buyButton;
    private TMP_Text popupText;
    private Button exitShopButton;

    private Button chickenButton;
    private Button cowButton;
    private Button pigButton;
    private Button duckButton;

    [Header("Animal Sprites")]
    public Sprite chickenSprite;
    public Sprite cowSprite;
    public Sprite pigSprite;
    public Sprite duckSprite;

    private PlayerWallet wallet;
    private DayNightCycleUI dayNightCycle;

    private AnimalData selectedAnimal;
    private readonly Dictionary<string, AnimalData> animals = new();
    private bool shopOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Define animals once
        animals["Chicken"] = new AnimalData("Chicken", 10, 3, 3);
        animals["Cow"]     = new AnimalData("Cow",     20, 8, 2);
        animals["Pig"]     = new AnimalData("Pig",     15, 5, 3);
        animals["duck"]    = new AnimalData("duck",    50, 10, 10);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupUIForScene();
        CloseShop();
    }

    private void SetupUIForScene()
    {
        wallet = FindFirstObjectByType<PlayerWallet>();
        dayNightCycle = FindFirstObjectByType<DayNightCycleUI>();

        // BATTLE SCENE → no shop
        if (SceneManager.GetActiveScene().name != "Game")
        {
            shopCanvas = null;
            shopOpen = false;
            return;
        }

        // GAME SCENE → instantiate prefab if needed
        if (shopCanvas == null)
        {
            shopCanvas = Instantiate(shopCanvasPrefab);

            // Place UI in scene canvas if required
            Canvas sceneCanvas = FindFirstObjectByType<Canvas>();
            if (sceneCanvas != null)
                shopCanvas.transform.SetParent(sceneCanvas.transform, false);
        }

        BindUIReferences();
        HookButtonEvents();

        shopCanvas.SetActive(false);

        if (wallet != null)
        {
            wallet.OnGoldChanged -= UpdateGold;
            wallet.OnGoldChanged += UpdateGold;
            UpdateGold(wallet.gold);
        }
    }

    private void BindUIReferences()
    {
        Transform root = shopCanvas.transform;

        animalImage     = root.Find("ShopPanel/LeftPanel/AnimalImage")?.GetComponent<Image>();
        statsText       = root.Find("ShopPanel/LeftPanel/StatsText")?.GetComponent<TMP_Text>();
        buyButton       = root.Find("ShopPanel/LeftPanel/BuyButton")?.GetComponent<Button>();

        chickenButton   = root.Find("ShopPanel/RightPanel/AnimalButtonChicken")?.GetComponent<Button>();
        cowButton       = root.Find("ShopPanel/RightPanel/AnimalButtonCow")?.GetComponent<Button>();
        pigButton       = root.Find("ShopPanel/RightPanel/AnimalButtonPig")?.GetComponent<Button>();
        duckButton      = root.Find("ShopPanel/RightPanel/AnimalButtonDuck")?.GetComponent<Button>();

        goldText        = root.Find("ShopPanel/HeaderGold/GoldText")?.GetComponent<TMP_Text>();
        exitShopButton  = root.Find("ShopPanel/ExitShopButton")?.GetComponent<Button>();
        popupText       = root.Find("PopupText")?.GetComponent<TMP_Text>();
    }

    private void HookButtonEvents()
    {
        chickenButton.onClick.RemoveAllListeners();
        cowButton.onClick.RemoveAllListeners();
        pigButton.onClick.RemoveAllListeners();
        duckButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        exitShopButton.onClick.RemoveAllListeners();

        chickenButton.onClick.AddListener(() => SelectAnimal("Chicken"));
        cowButton.onClick.AddListener(() => SelectAnimal("Cow"));
        pigButton.onClick.AddListener(() => SelectAnimal("Pig"));
        duckButton.onClick.AddListener(() => SelectAnimal("duck"));
        buyButton.onClick.AddListener(BuyAnimal);
        exitShopButton.onClick.AddListener(CloseShop);
    }

    public void ToggleShop()
    {
        if (shopOpen) CloseShop();
        else OpenShop();
    }

    public void OpenShop()
    {
        if (shopCanvas == null) return;

        shopCanvas.SetActive(true);
        shopOpen = true;
        SetLeftPanel(false);
    }

    public void CloseShop()
    {
        if (shopCanvas != null)
            shopCanvas.SetActive(false);

        shopOpen = false;
    }

    private void SelectAnimal(string name)
    {
        selectedAnimal = animals[name];
        statsText.text = $"HP: {selectedAnimal.hp}\nDamage: {selectedAnimal.damage}\nCost: {selectedAnimal.cost}";

        buyButton.interactable = !selectedAnimal.isOwned;

        animalImage.sprite = name switch
        {
            "Chicken" => chickenSprite,
            "Cow"     => cowSprite,
            "Pig"     => pigSprite,
            "duck"    => duckSprite,
            _         => null
        };

        SetLeftPanel(true);
    }

    private void BuyAnimal()
    {
        if (wallet.gold < selectedAnimal.cost)
        {
            ShowPopup("Not enough gold!");
            return;
        }

        selectedAnimal.isOwned = true;
        wallet.SpendGold(selectedAnimal.cost);
        buyButton.interactable = false;

        ShowPopup($"{selectedAnimal.name} added!");
    }

    private void UpdateGold(int amount)
    {
        goldText.text = $"Gold: {amount}";
    }

    private void ShowPopup(string message)
    {
        popupText.text = message;
        popupText.alpha = 1;
        StopAllCoroutines();
        StartCoroutine(FadePopup());
    }

    private IEnumerator FadePopup()
    {
        yield return new WaitForSecondsRealtime(1);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            popupText.alpha = 1 - t;
            yield return null;
        }
    }

    private void SetLeftPanel(bool visible)
    {
        animalImage.gameObject.SetActive(visible);
        statsText.gameObject.SetActive(visible);
        buyButton.gameObject.SetActive(visible);
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
        }
    }
}
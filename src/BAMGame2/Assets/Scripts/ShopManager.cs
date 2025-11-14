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

    [Header("Animals Sold Here")]
    public List<AnimalDefinition> animalsForSale;

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

    private PlayerWallet wallet;
    private DayNightCycleUI dayNightCycle;

    private AnimalDefinition selectedAnimal;
    private bool shopOpen = false;


    // ---------------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------------
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


    // ---------------------------------------------------------
    // SCENE SETUP
    // ---------------------------------------------------------
    private void SetupUIForScene()
    {
        wallet = FindFirstObjectByType<PlayerWallet>();
        dayNightCycle = FindFirstObjectByType<DayNightCycleUI>();

        // No shop in battle scene
        if (SceneManager.GetActiveScene().name != "Game")
        {
            shopCanvas = null;
            shopOpen = false;
            return;
        }

        // Instantiate prefab only once
        if (shopCanvas == null)
        {
            shopCanvas = Instantiate(shopCanvasPrefab);

            Canvas sceneCanvas = FindFirstObjectByType<Canvas>();
            if (sceneCanvas != null)
                shopCanvas.transform.SetParent(sceneCanvas.transform, false);
        }

        BindUIReferences();
        HookButtonEvents();

        shopCanvas.SetActive(false);

        // Update gold display
        if (wallet != null)
        {
            wallet.OnGoldChanged -= UpdateGold;
            wallet.OnGoldChanged += UpdateGold;
            UpdateGold(wallet.gold);
        }
    }


    // ---------------------------------------------------------
    // UI BINDING
    // ---------------------------------------------------------
    private void BindUIReferences()
    {
        Transform root = shopCanvas.transform;

        animalImage   = root.Find("ShopPanel/LeftPanel/AnimalImage")?.GetComponent<Image>();
        statsText     = root.Find("ShopPanel/LeftPanel/StatsText")?.GetComponent<TMP_Text>();
        buyButton     = root.Find("ShopPanel/LeftPanel/BuyButton")?.GetComponent<Button>();

        chickenButton = root.Find("ShopPanel/RightPanel/AnimalButtonChicken")?.GetComponent<Button>();
        cowButton     = root.Find("ShopPanel/RightPanel/AnimalButtonCow")?.GetComponent<Button>();
        pigButton     = root.Find("ShopPanel/RightPanel/AnimalButtonPig")?.GetComponent<Button>();
        duckButton    = root.Find("ShopPanel/RightPanel/AnimalButtonDuck")?.GetComponent<Button>();

        goldText      = root.Find("ShopPanel/HeaderGold/GoldText")?.GetComponent<TMP_Text>();
        exitShopButton = root.Find("ShopPanel/ExitShopButton")?.GetComponent<Button>();

        popupText     = root.Find("PopupText")?.GetComponent<TMP_Text>();
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
        duckButton.onClick.AddListener(() => SelectAnimal("Duck"));

        buyButton.onClick.AddListener(BuyAnimal);
        exitShopButton.onClick.AddListener(CloseShop);
    }


    // ---------------------------------------------------------
    // SHOP OPEN/CLOSE
    // ---------------------------------------------------------
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


    // ---------------------------------------------------------
    // SELECTING ANIMALS
    // ---------------------------------------------------------
    private void SelectAnimal(string name)
    {
        selectedAnimal = animalsForSale.Find(a => a.animalName == name);

        if (selectedAnimal == null)
        {
            Debug.LogError($"Animal '{name}' not found in animalsForSale list!");
            return;
        }

        // Update UI
        statsText.text =
            $"HP: {selectedAnimal.hp}\n" +
            $"Damage: {selectedAnimal.damage}\n" +
            $"Cost: {selectedAnimal.cost}";

        animalImage.sprite = selectedAnimal.portraitSprite;

        // Enable left panel
        SetLeftPanel(true);

        // Disable buy button if already owned
        buyButton.interactable = !PlayerAnimalInventory.Instance.OwnsAnimal(selectedAnimal);
    }


    // ---------------------------------------------------------
    // BUYING ANIMALS
    // ---------------------------------------------------------
    private void BuyAnimal()
    {
        if (selectedAnimal == null) return;

        if (wallet.gold < selectedAnimal.cost)
        {
            ShowPopup("Not enough gold!");
            return;
        }

        wallet.SpendGold(selectedAnimal.cost);
        PlayerAnimalInventory.Instance.AddAnimal(selectedAnimal);

        buyButton.interactable = false;

        ShowPopup($"{selectedAnimal.animalName} purchased!");
    }


    // ---------------------------------------------------------
    // UI HELPERS
    // ---------------------------------------------------------
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
        yield return new WaitForSecondsRealtime(1f);
        for (float t = 0; t < 1f; t += Time.deltaTime)
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
}
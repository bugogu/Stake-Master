using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Item
{
    public Sprite itemImage;
    public string itemName;
    public int itemPrice;
    public bool isPurchased;
}
public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;
    #region Variables
    #region Untitled
    [SerializeField] private GameManager gamManager;
    [SerializeField] private ParticleSystem purchaseFx;
    [Space]
    [Header("Level Progress")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Slider progressSlider;
    #endregion
    #region Game Over Panel
    [Space]
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button getButton;
    [SerializeField] private Sprite winButtonSp, loseButtonSp;
    [SerializeField] private Vector3 lastScale;
    [SerializeField] private TMP_Text gameoverRocoinText;
    [HideInInspector] public int earnRocoin;
    [HideInInspector] public float multipleRocoin;
    #endregion
    #region Settings Panel
    [Space]
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    public Toggle vibrateToggle;
    #endregion
    #region Shop Settings
    [Header("Layout Settings")]
    [SerializeField] private float itemSpacing = .5f;
    float itemHeight;
    [Header("UI Elements")]
    [SerializeField] private ShopDatabase characterDB;
    [SerializeField] private Transform shopMenu;
    [SerializeField] private Transform shopItemsContainer;
    [SerializeField] private GameObject itemPrefab;
    [Header("Shop Events")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;
    [SerializeField] private TMP_Text shopCointText;
    private System.Collections.Generic.List<int> purchasedIndex = new System.Collections.Generic.List<int>();
    #endregion
    #region Main Panel
    [Space]
    [Header("Main Panel")]
    [SerializeField] private Button rocketButton, incomeButton;
    [SerializeField] private AudioClip powerUp;
    [HideInInspector] public bool panelSetActive = true;
    [HideInInspector] public int rocoin, rocketLevel, incomeLevel, rocketPrice, incomePrice;
    [SerializeField] private GameObject clonePrefab1;
    [SerializeField] private Transform cloneParent1;
    [SerializeField] private GameObject countPowerActiveEffect, incomePowerActiveEffect;
    #endregion
    #region Others
    [Space]
    [Header("Others")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private RectTransform dangerImage;
    [SerializeField] private ParticleSystem upgradeParticle;
    [SerializeField] private Animator stikcmanAnim;
    [SerializeField] private TMP_Text rocoinText, rocketLevelText, incomeLevelText, rocketPriceText, incomePriceText;
    #endregion
    #region Private Variables
    private Transform _finishTransform;
    private RectTransform _gameOverPanelRect, _settingsPanelRect;
    private PlayerManager _plM;
    private GameObject _tank, _player, _tapSprite, _mainPanel;
    private int _vibrateValue, dartCount, tackCount, daggerCount, activeIndex;
    private float _maxDist; // Initial distance between Player and Finish Transform
    private LevelLoader _levelLoadEffect;
    private ParticleSystem ps;
    private int newSelectedItemIndex = 0;
    private int previousSelectedItemIndex = 0;
    #endregion
    #endregion
    #region General
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Component();
        PrefsControl();
    }
    private void Start()
    {
        _maxDist = GetDistance();
        SoundSettings();
        AddShopEvent();
        GenerateShopItemsUI();
    }
    private void Update()
    {
        GameOverControl();
        ControlPowerButtons();
        UpdateTextAndPrice();
        if (playerTransform.position.z <= _maxDist && playerTransform.position.z <= _finishTransform.position.z)
        {
            float dist = 1 - (GetDistance() / _maxDist);
            SetLevelProgress(dist);
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rocoin += 10000;
        }
#endif
    }
    void SetLevelProgress(float p)
    {
        progressSlider.value = p;
    }
    float GetDistance()
    {
        return Vector3.Distance(playerTransform.position, _finishTransform.position);
    }
    void PrefsControl()
    {
        activeIndex = SceneManager.GetActiveScene().buildIndex;
        rocketPrice = PlayerPrefs.GetInt("RocketPrice", 200);
        rocketLevel = PlayerPrefs.GetInt("RocketLevel");
        incomePrice = PlayerPrefs.GetInt("IncomePrice", 500);
        incomeLevel = PlayerPrefs.GetInt("IncomeLevel");
        multipleRocoin = PlayerPrefs.GetInt("Multiple");
        rocoin = PlayerPrefs.GetInt("Rocoin", 1000);
        multipleRocoin = 1f;
    }
    void SoundSettings()
    {
        vibrateToggle.isOn = PlayerPrefs.GetInt("vibrateOption", 1) == 1;
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            LoadVolume();
        }
        else
            LoadVolume();
    }
    void GameOverControl()
    {
        if (_player == null || _tank.activeInHierarchy == false)
        {
            earnRocoin = (_plM.giftValue + _plM.damageHolder + (int)(_plM.damageHolder * multipleRocoin)) * _plM.hotMultiplier;
            gameoverRocoinText.text = earnRocoin.ToString();
            Invoke("GameOverPanel", 1.1f);
        }
        if (_plM._countNumber <= 0 && _plM.gameOver)
        {
            Invoke("GameOverPanel", 1.2f);
        }
    }
    void Component()
    {
        _finishTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        _levelLoadEffect = GameObject.FindObjectOfType<LevelLoader>();
        _tank = gamManager.enemyParent.transform.GetChild(0).GetChild(0).gameObject;
        _plM = GameObject.FindObjectOfType<PlayerManager>();
        _gameOverPanelRect = gameOverPanel.GetComponent<RectTransform>();
        _settingsPanelRect = settingsPanel.GetComponent<RectTransform>();
        _player = GameObject.FindWithTag("Player");
        _mainPanel = GameObject.Find("MainPanel");
        _tapSprite = GameObject.Find("CanvasSwipeSprite");
    }
    void ControlPowerButtons()
    {
        if (panelSetActive)
        {
            if (rocoin < rocketPrice)
            {
                rocketButton.interactable = false;
                countPowerActiveEffect.SetActive(false);
            }
            if (rocoin < incomePrice)
            {
                incomeButton.interactable = false;
                incomePowerActiveEffect.SetActive(false);
            }
        }
    }
    #endregion
    #region Main Panel
    public void ClickToStart()
    {
        stikcmanAnim.SetTrigger("Kick");
        stikcmanAnim.gameObject.transform.parent = null;
        panelSetActive = false;
        _mainPanel.SetActive(false);
        _tapSprite.SetActive(false);
        countPowerActiveEffect.SetActive(false);
        incomePowerActiveEffect.SetActive(false);
        if (GameManager.instance1.turretMode)
        {
            dangerImage.gameObject.SetActive(true);
            dangerImage.DOScale(Vector3.one, 0.5f).OnComplete(() =>
            {
                dangerImage.DOScale(Vector3.zero, 0.5f);
            }).SetLoops(5, LoopType.Yoyo);
        }
    }
    private void UpdateTextAndPrice()
    {
        if (panelSetActive)
        {
            rocketLevelText.text = "Level " + rocketLevel;
            incomeLevelText.text = "Level " + incomeLevel;
            rocketPriceText.text = rocketPrice.ToString();
            incomePriceText.text = incomePrice.ToString();
            rocoinText.text = rocoin.ToString();
            shopCointText.text = rocoin.ToString();
        }
    }
    public void RocketButton()
    {
        purchaseFx.Play();
        upgradeParticle.Play();
        AudioSource.PlayClipAtPoint(powerUp, transform.position);
        Invoke(nameof(StopUpgradeParticle), 1);
        rocoin -= rocketPrice;
        _plM._countNumber++;
        rocketLevel++;
        rocketPrice += 200;
        GenerateClone();
        GameManager.instance1.cloneCount++;
        PlayerPrefs.SetInt("CloneCount", GameManager.instance1.cloneCount);
        PlayerPrefs.SetInt("CountNumber", _plM._countNumber);
    }
    private void StopUpgradeParticle()
    {
        upgradeParticle.Stop();
    }
    private void GenerateClone()
    {
        GameObject go = Instantiate(clonePrefab1, cloneParent1);
        GameManager.instance1.bullets.Add(go);
        go.transform.localPosition = Vector3.zero;
        GameManager.instance1.Align();
    }
    public void IncomeButton()
    {
        purchaseFx.Play();
        upgradeParticle.Play();
        AudioSource.PlayClipAtPoint(powerUp, transform.position);
        Invoke(nameof(StopUpgradeParticle), 1);
        rocoin -= incomePrice;
        multipleRocoin += .20f;
        incomeLevel++;
        incomePrice += 250;
    }
    #endregion
    #region Game Over Panel
    private void GameOverPanel()
    {
        _gameOverPanelRect.gameObject.SetActive(true);
        _gameOverPanelRect.DOScale(lastScale, 1);
        if (_tank.activeInHierarchy == false)
        {
            getButton.image.sprite = winButtonSp;
        }
        else
        {
            getButton.image.sprite = loseButtonSp;
        }
    }
    public void GetEarnButton()
    {
        getButton.interactable = false;
        if (_tank.activeInHierarchy == false)
        {
            rocoin += (earnRocoin + (100 * GameManager.instance1.levelIndex));
            getButton.image.sprite = winButtonSp;
        }
        else
        {
            rocoin += earnRocoin;
            getButton.image.sprite = loseButtonSp;
        }
        PlayerPrefs.SetInt("Rocoin", rocoin);
        PlayerPrefs.SetInt("TankHealt", _tank.GetComponent<EnemyManager>().tankHealt);
        PlayerPrefs.SetFloat("HealtBar", _tank.GetComponent<EnemyManager>().healtBarImage.fillAmount);
        PlayerPrefs.SetInt("RocketPrice", rocketPrice);
        PlayerPrefs.SetInt("RocketLevel", rocketLevel);
        PlayerPrefs.SetInt("IncomePrice", incomePrice);
        PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
        PlayerPrefs.SetFloat("Multiple", multipleRocoin);
        if (_tank.activeInHierarchy != false)
        {
            _levelLoadEffect.LoadNextLevel(0);
        }
        else if (_tank.activeInHierarchy == false)
        {
            PlayerPrefs.DeleteKey("TankHealt");
            PlayerPrefs.SetFloat("HealtBar", 1);
            GameManager.instance1.levelIndex++;
            PlayerPrefs.SetInt("Level", GameManager.instance1.levelIndex++);
            _levelLoadEffect.LoadNextLevel(0);
        }
    }
    public void Earn3xRocoin()
    {
        rocoin += (earnRocoin * 3);
        PlayerPrefs.SetInt("Rocoin", rocoin);
        PlayerPrefs.SetInt("TankHealt", _tank.GetComponent<EnemyManager>().tankHealt);
        PlayerPrefs.SetFloat("HealtBar", _tank.GetComponent<EnemyManager>().healtBarImage.fillAmount);
        PlayerPrefs.SetInt("RocketPrice", rocketPrice);
        PlayerPrefs.SetInt("RocketLevel", rocketLevel);
        PlayerPrefs.SetInt("IncomePrice", incomePrice);
        PlayerPrefs.SetInt("IncomeLevel", incomeLevel);
        PlayerPrefs.SetFloat("Multiple", multipleRocoin);
        if (_tank.activeInHierarchy != false)
        {
            _levelLoadEffect.LoadNextLevel(0);
        }
        else if (_tank.activeInHierarchy == false)
        {
            PlayerPrefs.DeleteKey("TankHealt");
            PlayerPrefs.SetFloat("HealtBar", 1);
            _levelLoadEffect.LoadNextLevel(1);
        }
    }
    #endregion
    #region Settings Panel 
    public void SettingsActive()
    {
        _settingsPanelRect.gameObject.SetActive(true);
        _settingsPanelRect.DOScale(lastScale, 1);
    }
    public void CloseSettingsButton()
    {
        _settingsPanelRect.DOScale(Vector3.zero, 1);
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        SaveVolume();
    }
    private void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }
    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
    public void ChangeVibrate()
    {
        PlayerPrefs.SetInt("vibrateOption", vibrateToggle.isOn ? 1 : 0);
    }
    #endregion
    #region Shop Panel
    void AddShopEvent()
    {
        openShopButton.onClick.RemoveAllListeners();
        openShopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.RemoveAllListeners();
        closeShopButton.onClick.AddListener(CloseShop);
    }
    void OpenShop()
    {
        shopPanel.SetActive(true);
    }
    void CloseShop()
    {
        shopPanel.SetActive(false);
    }
    void GenerateShopItemsUI()
    {
        for (int i = 0; i < purchasedIndex.Count; i++)
        {
            characterDB.Purchase(purchasedIndex[i]);
        }
        itemHeight = shopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(shopItemsContainer.GetChild(0).gameObject);
        shopItemsContainer.DetachChildren();
        for (int i = 0; i < characterDB.CharacterCount; i++)
        {
            Item item = characterDB.GetCharacter(i);
            ShopItemControl shopItem = Instantiate(itemPrefab, shopItemsContainer).GetComponent<ShopItemControl>();

            shopItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));
            shopItem.SetCharacterName(item.itemName);
            shopItem.SetCharacterImage(item.itemImage);
            shopItem.SetCharacterPrice(item.itemPrice);
            if (item.isPurchased)
            {
                shopItem.SetCharacterAsPurchased();
                shopItem.OnItemSelect(i, OnItemSelected);
            }
            else
            {
                shopItem.SetCharacterPrice(item.itemPrice);
                shopItem.OnItemPurchase(i, OnItemPurchased);
            }
            shopItemsContainer.GetComponent<RectTransform>().sizeDelta =
            Vector2.up * ((itemHeight + itemSpacing) * characterDB.CharacterCount + itemSpacing);
        }
    }
    void OnItemSelected(int index)
    {
        SelectItemUI(index);
    }
    void SelectItemUI(int itemInd)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemInd;
        PlayerPrefs.SetInt("SkinID", (itemInd + 1));
        ShopItemControl prevUiItem = GetItemUI(previousSelectedItemIndex);
        ShopItemControl newUiItem = GetItemUI(newSelectedItemIndex);
        prevUiItem.DeselectItem();
        newUiItem.SelectItem();
    }
    ShopItemControl GetItemUI(int index)
    {
        return shopItemsContainer.GetChild(index).GetComponent<ShopItemControl>();
    }
    void OnItemPurchased(int index)
    {
        Item item = characterDB.GetCharacter(index);
        ShopItemControl uiItem = GetItemUI(index);
        if (!(rocoin > item.itemPrice)) return;
        purchaseFx.Play();
        rocoin -= item.itemPrice;
        characterDB.Purchase(index);
        uiItem.SetCharacterAsPurchased();
        uiItem.OnItemSelect(index, OnItemSelected);
        purchasedIndex.Add(index);
    }
    #endregion
}
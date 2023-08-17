using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance1;
    #region Variables
    #region Serialize
    [Space]
    [Header("General")]
    public GameObject enemyParent;
    [SerializeField] private GameObject levelsParent;
    [SerializeField] private GameObject platform, clonePrefab2, platformLeftSide, platformRightSide;
    [SerializeField] private Material[] platformMats, skyboxMats;
    [SerializeField] private Transform cloneParent2;
    [HideInInspector] public int cloneCount, levelIndex;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject bulletPrefab;
    public LayerMask layerMask;
    public float minX, maxX, distance;
    [Space]
    [Header("Turret")]
    public bool turretMode;
    public int widthX, damageDealt;
    public float speed;
    [SerializeField] private GameObject turretRocketPrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int spawnCount;
    [SerializeField] private Transform spawnPoint;
    [Space]
    [Header("Gift")]
    [SerializeField] private bool giftLevel;
    [SerializeField] private GameObject giftPrefab;
    [SerializeField] private int giftWave;
    [SerializeField] private float giftX, giftZMin, giftZMax;
    [Space]
    [Header("Bomb")]
    [SerializeField] private bool bombLevel;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int bombWave;
    [SerializeField] private float bombX, bombZMin, bombZMax;
    [Space]
    [Header("Arrow Animastion")]
    [SerializeField] private GameObject upgradeArrowsMove, downArrowsMove;
    [SerializeField] private float arrowsForwardSpeed;
    [Space]
    [Header("Explosion")]
    [SerializeField] private GameObject tankExplosionPrefab;
    [SerializeField] private Transform explosionPos;
    [Space]
    [Header("Level Text")]
    [SerializeField] private TMP_Text levelText, levelText2;
    #endregion
    #region Privates
    private int _turretIndex, _giftIndex, _bombIndex, _loadSceneIndex;
    private bool _tankActive = true;
    private PlayerManager _gameoverControl;
    private bool _turretGenerate = true;
    private GameObject _isActiveTank;
    #endregion
    #endregion
    #region General
    private void Awake()
    {
        if (instance1 == null)
        {
            instance1 = this;
        }
        levelIndex = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", levelIndex);
        Application.targetFrameRate = 60;
        _isActiveTank = enemyParent.transform.GetChild(0).GetChild(0).gameObject;
        _gameoverControl = GameObject.FindObjectOfType<PlayerManager>();
        cloneCount = PlayerPrefs.GetInt("CloneCount", 0);
        for (int i = 0; i < cloneCount; i++)
        {
            GameObject go = Instantiate(clonePrefab2, cloneParent2);
            bullets.Add(go);
            go.transform.localPosition = Vector3.zero;
            Align();
        }

    }
    void Start()
    {
        Application.targetFrameRate = 60;
        RenderSettings.skybox = skyboxMats[Random.Range(0, 1)];
        Material platformMat = platform.GetComponent<MeshRenderer>().material = platformMats[Random.Range(0, platformMats.Length)];
        platformLeftSide.GetComponent<MeshRenderer>().material.color = platformMat.color;
        platformRightSide.GetComponent<MeshRenderer>().material.color = platformMat.color;
        platformMat.mainTextureScale = new Vector2(Random.Range(1, 3), 40);
        levelText.text = levelIndex.ToString();
        levelText2.text = "Level " + (levelIndex);
        _giftIndex = PlayerPrefs.GetInt("Gift");
        _bombIndex = PlayerPrefs.GetInt("Bomb");
        _turretIndex = PlayerPrefs.GetInt("TurretLevel", 0);
        _turretIndex++;
        damageDealt += levelIndex;
        PlayerPrefs.SetInt("TurretLevel", _turretIndex);
        Debug.Log(levelIndex);
        #region Level Generate
        if (levelIndex > levelsParent.transform.childCount)
        {
            levelsParent.transform.GetChild(Random.Range(0, levelsParent.transform.childCount)).gameObject.SetActive(true);
        }
        else
        {
            levelsParent.transform.GetChild(levelIndex - 1).gameObject.SetActive(true);
        }
        if (giftLevel)
        {
            Vector3 giftLocation = new Vector3(Random.Range(-giftX, giftX), _gameoverControl.gameObject.transform.position.y, Random.Range(giftZMin, giftZMax));
            if (_giftIndex % giftWave == 0)
            {
                GameObject obj = Instantiate(giftPrefab);
                obj.transform.position = giftLocation;
            }
            _giftIndex++;
            PlayerPrefs.SetInt("Gift", _giftIndex);
        }
        if (bombLevel)
        {
            Vector3 bombLocation = new Vector3(Random.Range(-bombX, bombX), _gameoverControl.gameObject.transform.position.y, Random.Range(bombZMin, bombZMax));
            if (_bombIndex % bombWave == 0)
            {
                GameObject obj = Instantiate(bombPrefab);
                obj.transform.position = bombLocation;
            }
            _bombIndex++;
            PlayerPrefs.SetInt("Bomb", _bombIndex);
        }
        if (_turretIndex % 3 == 0)
        {
            turretMode = true;
        }
        #endregion
    }
    void Update()
    {
        if (!GUIManager.instance.panelSetActive) { if (Input.GetButton("Fire1")) { GetRay(); } }
        if (turretMode && _turretGenerate == true && GUIManager.instance.panelSetActive == false)
        {
            StartCoroutine("GenerateTurretRocket");
            _turretGenerate = false;
        }
        ArrowsForwardMove();
        ExplosionEffect();
    }
    #endregion
    #region Circle Bullet Alignment
    private void MoveBullet(Transform objectTransform, float degree)
    {
        Vector3 pos = Vector3.zero;
        pos.x = Mathf.Cos(degree * Mathf.Deg2Rad);
        pos.y = Mathf.Sin(degree * Mathf.Deg2Rad);
        objectTransform.localPosition = pos * distance;
    }
    public void Align()
    {
        float angle = 1f;
        float bulletCount = bullets.Count;
        angle = 360 / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            MoveBullet(bullets[i].transform, i * angle);
        }
    }
    private void GetRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Vector3 mouse = hit.point;
            mouse.x = Mathf.Clamp(mouse.x, minX, maxX);
            distance = mouse.x;
            Align();
        }
    }
    #endregion
    #region Others
    private void ArrowsForwardMove()
    {
        if (!GUIManager.instance.panelSetActive && _gameoverControl.gameOver == false)
        {
            upgradeArrowsMove.transform.Translate(Vector3.forward * arrowsForwardSpeed * Time.deltaTime);
            downArrowsMove.transform.Translate(Vector3.forward * arrowsForwardSpeed * Time.deltaTime);
        }
    }
    private void ExplosionEffect()
    {

        if (_isActiveTank.activeInHierarchy == false && _tankActive == true)
        {
            Instantiate(tankExplosionPrefab, explosionPos);
            _tankActive = false;
        }
    }
    IEnumerator GenerateTurretRocket()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(turretRocketPrefab, spawnPoint);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    #endregion
}



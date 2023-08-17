using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public enum FinishType
    {
        Normal,
        Reverse,
        TwoSide,
        TwoSideReverse,
        SideReverse,
        SideReverse1
    }
    public FinishType finishType;
    #region Variables
    #region Serialize
    [Header("General")]
    [SerializeField] private GameObject upgradeArrows, downArrows, hotImage, finishConfetti, finishConfettiWorld, hotTextObj, confetti, bombExplode, clonePrefab;
    [SerializeField] private Transform cloneParent;
    [SerializeField] private TMP_Text hotText;
    [Header("Player")]
    [SerializeField] private int skinLenght;
    public float forwardSpeed;
    [SerializeField] private Transform lookRotObj;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private float lastShootSpeed, horizontalSpeed, horizontalLimit, lerpValue;
    [SerializeField] private AudioClip positive, negative, collectBullet, win;
    #endregion
    #region Public
    [HideInInspector] public bool lastShootActive = false;
    [HideInInspector] public int _countNumber;
    [HideInInspector] public int damageHolder = 0;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public int hotMultiplier = 1;
    [HideInInspector] public int giftValue;
    #endregion
    #region Private
    private Rigidbody _rb;
    private float _horizontal, _newX, _lastShootDuration;
    private int _gateNumber;
    private GateManager _gateScript;
    private bool finishControl = false;
    #endregion
    #endregion
    #region General
    private void Awake()
    {
        _countNumber = PlayerPrefs.GetInt("CountNumber");
        if (PlayerPrefs.HasKey("CountNumber") == false)
        {
            _countNumber = 0;
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        SelectFinishType();
        hotText.text = hotMultiplier + "x";
        if (PlayerPrefs.HasKey("SkinID")) return;
        PlayerPrefs.SetInt("SkinID", 1);
    }
    void Update()
    {
        if (gameOver == true && _countNumber > 0)
        {
            InputControl(false);
        }
        HorizontalMove();
        ForwardMove();
        UpdateCountText();
        RotateRPG();
        LastShoot();
        if (!transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.activeSelf)
        {
            for (int i = 1; i < skinLenght + 1; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.SetActive(true);
        }
        if (!finishControl) return;
        if (GameManager.instance1.enemyParent.transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy) return;
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Gate":
                _gateNumber = other.gameObject.GetComponent<GateManager>().GetGateNumber();
                _gateScript = other.gameObject.GetComponent<GateManager>();
                if (_gateScript.gateType == GateManager.GateType.PositiveGate)
                {
                    AudioSource.PlayClipAtPoint(positive, transform.position);
                    upgradeArrows.SetActive(true);
                    _countNumber += _gateNumber;
                    for (int i = 0; i < _gateNumber; i++)
                    {
                        GameObject g = Instantiate(clonePrefab, cloneParent);
                        GameManager.instance1.bullets.Add(g);
                        g.transform.localPosition = Vector3.zero;
                        GameManager.instance1.Align();
                    }
                }
                else if (_gateScript.gateType == GateManager.GateType.NegativeGate)
                {
                    AudioSource.PlayClipAtPoint(negative, transform.position);
                    _countNumber += _gateNumber;
                    downArrows.SetActive(true);
                    if (GUIManager.instance.vibrateToggle.isOn)
                    {
                        Handheld.Vibrate();
                    }
                    _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                    for (int i = 0; i < Mathf.Abs(_gateNumber); i++)
                    {
                        GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                        GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                        Destroy(g);
                        GameManager.instance1.Align();
                    }
                }
                else if (_gateScript.gateType == GateManager.GateType.PositiveGateX)
                {
                    AudioSource.PlayClipAtPoint(positive, transform.position);
                    for (int i = 0; i < (_countNumber * _gateNumber) - _countNumber; i++)
                    {
                        GameObject g = Instantiate(clonePrefab, cloneParent);
                        GameManager.instance1.bullets.Add(g);
                        g.transform.localPosition = Vector3.zero;
                        GameManager.instance1.Align();
                    }
                    upgradeArrows.SetActive(true);
                    _countNumber *= _gateNumber;
                }
                else if (_gateScript.gateType == GateManager.GateType.NegativeGateY)
                {
                    AudioSource.PlayClipAtPoint(negative, transform.position);
                    for (int i = 0; i < _countNumber - (_countNumber / _gateNumber); i++)
                    {
                        GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                        GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                        Destroy(g);
                        GameManager.instance1.Align();
                    }
                    downArrows.SetActive(true);
                    _countNumber /= _gateNumber;
                    if (GUIManager.instance.vibrateToggle.isOn)
                    {
                        Handheld.Vibrate();
                    }
                }
                else if (_gateScript.gateType == GateManager.GateType.JetPositive)
                {
                    // AudioSource.PlayClipAtPoint(success, transform.position);
                    upgradeArrows.SetActive(true);
                    StartCoroutine(GenerateClone(_gateNumber));
                }
                _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                break;
            case "Bullet":
                AudioSource.PlayClipAtPoint(collectBullet, transform.position);
                _countNumber += 1;
                GameObject ga = Instantiate(clonePrefab, cloneParent);
                GameManager.instance1.bullets.Add(ga);
                ga.transform.localPosition = Vector3.zero;
                GameManager.instance1.Align();
                Destroy(other.gameObject);
                break;
            case "Gift":
                AudioSource.PlayClipAtPoint(positive, transform.position);
                giftValue += 300;
                GameObject.Instantiate(confetti, other.transform);
                confetti.transform.parent = null;
                Destroy(other.gameObject, 0.35f);
                break;
            case "Bomb":
                AudioSource.PlayClipAtPoint(negative, transform.position);
                if (_countNumber % 2 == 0)
                {
                    for (int i = 0; i < _countNumber / 2; i++)
                    {
                        GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                        GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                        Destroy(g);
                        GameManager.instance1.Align();
                    }
                    GameObject.Instantiate(bombExplode, other.transform);
                    confetti.transform.parent = null;
                    Destroy(other.gameObject, 0.2f);
                }
                else if (_countNumber % 2 != 0)
                {
                    for (int i = 0; i < (_countNumber + 1) / 2; i++)
                    {
                        GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                        GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                        Destroy(g);
                        GameManager.instance1.Align();
                    }
                    GameObject.Instantiate(bombExplode, other.transform);
                    confetti.transform.parent = null;
                    Destroy(other.gameObject, 0.2f);
                }
                _countNumber /= 2;
                break;
            case "Finish":
                finishControl = true;
                AudioSource.PlayClipAtPoint(win, transform.position);
                _rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                hotTextObj.SetActive(true);
                gameOver = true;
                transform.position = Vector3.Lerp(transform.position, other.gameObject.transform.position, lerpValue);
                cloneParent.position = Vector3.Lerp(cloneParent.position, other.gameObject.transform.position, lerpValue);
                StartCoroutine(GenerateClone(_gateNumber));
                finishConfetti.SetActive(true);
                finishConfettiWorld.SetActive(true);
                other.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                break;
            case "TurretRocket":
                _countNumber -= (GameManager.instance1.damageDealt);
                if (GUIManager.instance.vibrateToggle.isOn)
                {
                    Handheld.Vibrate();
                }
                _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                for (int i = 0; i < GameManager.instance1.damageDealt; i++)
                {
                    GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                    GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                    Destroy(g);
                    GameManager.instance1.Align();
                }
                break;
            case "Obstacle":
                AudioSource.PlayClipAtPoint(negative, transform.position);
                for (int i = 0; i < _countNumber; i++)
                {
                    GameObject g = GameManager.instance1.bullets[GameManager.instance1.bullets.Count - 1];
                    GameManager.instance1.bullets.RemoveAt(GameManager.instance1.bullets.Count - 1);
                    Destroy(g);
                    GameManager.instance1.Align();
                }
                _countNumber = 0;
                if (GUIManager.instance.vibrateToggle.isOn)
                {
                    Handheld.Vibrate();
                }
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Gate"))
        {
            _gateScript = other.gameObject.GetComponent<GateManager>();
            if (_gateScript.gateType == GateManager.GateType.PositiveGate)
            {
                Invoke("ActiveUpgradeArrows", 1);
            }
            else if (_gateScript.gateType == GateManager.GateType.NegativeGate)
            {
                Invoke("ActiveDownArrows", 1);
            }
            else if (_gateScript.gateType == GateManager.GateType.PositiveGateX)
            {
                Invoke("ActiveUpgradeArrows", 1);
            }
            else if (_gateScript.gateType == GateManager.GateType.NegativeGateY)
            {
                Invoke("ActiveDownArrows", 1);
            }
            else if (_gateScript.gateType == GateManager.GateType.JetPositive)
            {
                Invoke("ActiveUpgradeArrows", 1);
            }
        }
    }
    #endregion
    #region Movement
    private void HorizontalMove()
    {
        if (!GUIManager.instance.panelSetActive && gameOver == false)
        {
            if (Input.GetMouseButton(0))
            {
                _horizontal = Input.GetAxisRaw("Mouse X");
            }
            else
            {
                _horizontal = 0;
            }
            _newX = transform.position.x + _horizontal * horizontalSpeed * Time.deltaTime;
            _newX = Mathf.Clamp(_newX, -horizontalLimit, horizontalLimit);
            transform.position = new Vector3(
                _newX,
                transform.position.y,
                transform.position.z
                );
        }
    }
    private void RotateRPG()
    {
        transform.LookAt(lookRotObj.position);
    }
    private void ForwardMove()
    {
        if (!GUIManager.instance.panelSetActive && gameOver == false)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
    }
    private void LastShoot()
    {
        if (gameOver == true && _countNumber == 0)
        {
            lastShootActive = true;
            transform.Translate(Vector3.forward * lastShootSpeed * Time.deltaTime);
        }
    }
    #endregion
    #region Others
    private void InputControl(bool inputEnabled)
    {
        if (inputEnabled) return;
        Input.ResetInputAxes();
    }
    private void SelectFinishType()
    {
        int randomType = Random.Range(0, 6);
        switch (randomType)
        {
            case 0:
                finishType = FinishType.Normal;
                break;
            case 1:
                finishType = FinishType.Reverse;
                break;
            case 2:
                finishType = FinishType.TwoSide;
                break;
            case 3:
                finishType = FinishType.TwoSideReverse;
                break;
            case 4:
                finishType = FinishType.SideReverse;
                break;
            case 5:
                finishType = FinishType.SideReverse1;
                break;
        }
    }
    private void UpdateCountText()
    {
        countText.text = _countNumber.ToString();
    }
    private void ActiveUpgradeArrows()
    {
        upgradeArrows.SetActive(false);
    }
    private void ActiveDownArrows()
    {
        downArrows.SetActive(false);
    }
    #endregion
    IEnumerator GenerateClone(int gateNumber)
    {
        yield return new WaitForSeconds(0.65f);
        cloneParent.parent = null;
        switch (finishType)
        {
            case FinishType.Normal:
                for (int i = 0; i <= _countNumber; _countNumber--)
                {
                    cloneParent.GetChild(_countNumber - 1).parent = null;
                    transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
                    damageHolder++;
                    if (damageHolder % 50 == 0)
                    {
                        hotImage.SetActive(true);
                        hotMultiplier++;
                        hotText.text = hotMultiplier + "x";
                    }
                    _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                    #region Speed Rate
                    yield return new WaitForSeconds(0.040f);
                    #endregion
                }
                break;
            case FinishType.Reverse:
                for (int i = 0; i <= _countNumber; _countNumber--)
                {
                    cloneParent.GetChild(0).parent = null;
                    transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
                    damageHolder++;
                    if (damageHolder % 50 == 0)
                    {
                        hotImage.SetActive(true);
                        hotMultiplier++;
                        hotText.text = hotMultiplier + "x";
                    }
                    _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                    #region Speed Rate
                    yield return new WaitForSeconds(0.040f);
                    #endregion
                }
                break;
            case FinishType.TwoSide:
                if (_countNumber % 2 == 0)
                {
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) + 1)).parent = null;
                        cloneParent.GetChild(Mathf.Abs(((_countNumber + 1) / 2) - 1)).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                else if (_countNumber % 2 != 0)
                {
                    _countNumber--;
                    cloneParent.GetChild(0).parent = null;
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) + 1)).parent = null;
                        cloneParent.GetChild(Mathf.Abs(((_countNumber + 1) / 2) - 1)).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                break;
            case FinishType.TwoSideReverse:
                if (_countNumber % 2 == 0)
                {
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(_countNumber - 1).parent = null;
                        cloneParent.GetChild(0).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                else if (_countNumber % 2 != 0)
                {
                    _countNumber--;
                    cloneParent.GetChild(_countNumber - 1).parent = null;
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(_countNumber - 1).parent = null;
                        cloneParent.GetChild(0).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                break;
            case FinishType.SideReverse:
                if (_countNumber % 2 == 0)
                {
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) + 1)).parent = null;
                        cloneParent.GetChild(0).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                else if (_countNumber % 2 != 0)
                {
                    _countNumber--;
                    cloneParent.GetChild(0).parent = null;
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) + 1)).parent = null;
                        cloneParent.GetChild(0).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        #region Speed Rate
                        yield return new WaitForSeconds(0.060f);
                        #endregion
                    }
                }
                break;
            case FinishType.SideReverse1:
                if (_countNumber % 2 == 0)
                {
                    _countNumber--;
                    _countNumber--;
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        if (_countNumber <= 0)
                        {
                            cloneParent.GetChild(0).parent = null;
                            cloneParent.GetChild(0).parent = null;
                        }
                        cloneParent.GetChild(_countNumber - 1).parent = null;
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) - 1)).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        yield return new WaitForSeconds(0.060f);
                    }
                }
                else if (_countNumber % 2 != 0)
                {
                    _countNumber--;
                    for (int i = 0; i <= _countNumber; _countNumber -= 2)
                    {
                        if (_countNumber <= 0)
                        {
                            cloneParent.GetChild(0).parent = null;
                        }
                        cloneParent.GetChild(_countNumber - 1).parent = null;
                        cloneParent.GetChild(Mathf.Abs(((_countNumber - 1) / 2) - 1)).parent = null;
                        transform.localScale += new Vector3(0.50f, 0.50f, 0.50f);
                        damageHolder += 2;
                        if (damageHolder % 50 == 0)
                        {
                            hotImage.SetActive(true);
                            hotMultiplier++;
                            hotText.text = hotMultiplier + "x";
                        }
                        _countNumber = Mathf.Clamp(_countNumber, 0, 3000);
                        yield return new WaitForSeconds(0.060f);
                    }
                }
                break;
        }
    }

}

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    #region Variables
    [Header("Enemy")]
    private int score;
    public int maxHealt;
    [SerializeField] private AudioClip damageTaken;
    [SerializeField] private TMP_Text healtText;
    [SerializeField] private Material hitEffect, hitEffect2;
    public Image healtBarImage;
    [HideInInspector] public int tankHealt;
    [HideInInspector] public MeshRenderer _mr;
    private Coroutine hitRoutine;
    private Material _orginalMat;
    [Space]
    [Header("Hit Shake Effect")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float strength;
    [SerializeField] private int vibrateCount;
    [SerializeField] private float randomness;
    #region Others
    [HideInInspector] public bool playerActive = true;
    private PlayerManager _playerScript;
    [SerializeField] private GameManager gaManager;
    #endregion
    #endregion
    #region General
    void Awake()
    {
        healtBarImage.fillAmount = PlayerPrefs.GetFloat("HealtBar");
        tankHealt = PlayerPrefs.GetInt("TankHealt");
        if (PlayerPrefs.HasKey("HealtBar") == false)
        {
            healtBarImage.fillAmount = 1;
        }
        _mr = GetComponent<MeshRenderer>();
        _orginalMat = GetComponent<MeshRenderer>().material;
        _playerScript = GameObject.FindObjectOfType<PlayerManager>();
    }
    private void Start()
    {
        maxHealt = PlayerPrefs.GetInt("MaxHealt", 0);
        maxHealt = (250 * (gaManager.levelIndex));
        PlayerPrefs.SetInt("MaxHealt", maxHealt);
        if (PlayerPrefs.HasKey("TankHealt") == false)
        {
            tankHealt = maxHealt;
        }
    }
    void Update()
    {
        UpdateHealt();
        if (tankHealt <= 0)
        {
            transform.gameObject.SetActive(false);
        }
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Clone"))
        {
            AudioSource.PlayClipAtPoint(damageTaken, _playerScript.gameObject.transform.position);
            healtBarImage.fillAmount -= 1f / maxHealt;
            CloneEffect();
            tankHealt--;
            Destroy(col.gameObject);
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (GUIManager.instance.vibrateToggle.isOn)
            {
                Handheld.Vibrate();
            }
            ShakeHitEffect();
            AudioSource.PlayClipAtPoint(damageTaken, transform.position);
            healtBarImage.fillAmount -= (1 * (float)_playerScript.damageHolder) / maxHealt;
            CloneEffect2();
            tankHealt -= _playerScript.damageHolder;
            Destroy(col.gameObject);
            playerActive = false;

        }
    }
    #endregion
    #region Enemy
    private void UpdateHealt()
    {
        healtText.text = tankHealt.ToString() + "/" + maxHealt;
    }
    private void ShakeHitEffect()
    {
        transform.DOShakeScale(shakeDuration, strength, vibrateCount, randomness);
    }
    private IEnumerator HitRoutine()
    {
        _mr.material = hitEffect;
        yield return new WaitForSeconds(0.02f);
        _mr.material = _orginalMat;
        hitRoutine = null;
    }
    private IEnumerator HitRoutine2()
    {
        _mr.material = hitEffect2;
        yield return new WaitForSeconds(0.2f);
        _mr.material = _orginalMat;
        hitRoutine = null;
    }
    public void CloneEffect()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }
        hitRoutine = StartCoroutine(HitRoutine());
    }
    public void CloneEffect2()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }
        hitRoutine = StartCoroutine(HitRoutine2());
    }
    #endregion
}

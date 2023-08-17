using UnityEngine;

public class CamController : MonoBehaviour
{
    #region Variables
    #region Serialize
    [Header("General")]
    [SerializeField] private Transform playerTransform, lastCamPos;
    [SerializeField] private float lerpTime, lerpMovePos;
    [SerializeField] private GameManager gManager;
    [Header("Shake")]
    [SerializeField] private GameObject player;
    [SerializeField] private float _shakePower, _shakeDuration, _downAmount;
    #endregion
    #region Private
    private bool _playerActive = true;
    private bool _tankActive = true;
    private float _initialDuration;
    private Vector3 _startPos, _offset;
    private Transform _cam;
    private PlayerManager gameOverControl;
    private GameObject isActiveTank;
    private EnemyManager enemyScript;
    #endregion
    #endregion
    #region General
    private void Awake()
    {
        gameOverControl = GameObject.FindObjectOfType<PlayerManager>();
        isActiveTank = gManager.enemyParent.transform.GetChild(0).GetChild(0).gameObject;
        enemyScript = isActiveTank.GetComponent<EnemyManager>();
    }
    void Start()
    {
        _startPos = lastCamPos.position;
        _initialDuration = _shakeDuration;
        _offset = transform.position - playerTransform.position;
    }
    private void Update()
    {
        if (player.gameObject == null && _playerActive == true)
        {
            if (_shakeDuration > 0)
            {
                transform.localPosition = _startPos + Random.onUnitSphere * _shakePower;
                _shakeDuration -= _downAmount * Time.deltaTime;
            }
            else
            {
                _playerActive = false;
                transform.localPosition = _startPos;
                _shakeDuration = _initialDuration;
            }
        }

        if (isActiveTank.activeInHierarchy != true && _tankActive == true)
        {
            if (GUIManager.instance.vibrateToggle.isOn)
            {
                Handheld.Vibrate();
            }
            if (_shakeDuration > 0)
            {
                transform.localPosition = _startPos + Random.onUnitSphere * _shakePower;
                _shakeDuration -= _downAmount * Time.deltaTime;
            }
            else
            {
                _tankActive = false;
                transform.localPosition = _startPos;
                _shakeDuration = _initialDuration;
            }
        }

    }
    void LateUpdate()
    {
        if (gameOverControl.gameOver == false)
        {
            Vector3 _newPos = Vector3.Lerp(
                       transform.position,
                       playerTransform.position + _offset,
                       lerpTime * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, _newPos.z);
        }
        else if (gameOverControl.gameOver == true)
        {
            transform.position = Vector3.Lerp(transform.position, lastCamPos.position, lerpMovePos);
        }
    }
    #endregion
}

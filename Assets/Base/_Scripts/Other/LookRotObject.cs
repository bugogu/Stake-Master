using UnityEngine;

public class LookRotObject : MonoBehaviour
{
    #region Variables
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float horizontalLimit;
    [SerializeField] private float forwardSpeed;
    private PlayerManager _isOnGameOver;
    private float _horizontal;
    private float _newX;
    #endregion
    private void Awake()
    {
        _isOnGameOver = GameObject.FindObjectOfType<PlayerManager>();
    }
    void Update()
    {
        if (!GUIManager.instance.panelSetActive && !_isOnGameOver.gameOver)
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
            if (!GUIManager.instance.panelSetActive)
            {
                transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
            }
        }
        else if(_isOnGameOver.gameOver == true)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z + 100);
        }
    }
}

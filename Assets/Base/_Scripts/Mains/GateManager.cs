using UnityEngine;
using TMPro;

public class GateManager : MonoBehaviour
{
    #region Variables
    public GateType gateType;
    #region Serialize
    [Header("Gate Value")]
    [SerializeField] private TMP_Text gateText;
    [SerializeField] private int maxValue;
    [SerializeField] private int minValue;
    [Header("Movement")]
    [SerializeField] private GateMove gateMove;
    [SerializeField] private float moveLimit;
    [SerializeField] private float moveSpeed;
    #endregion
    private Vector3 _startPos;
    private Vector3 _movePos;
    private int gateNumber;
    public enum GateType
    {
        PositiveGate,
        NegativeGate,
        PositiveGateX,
        NegativeGateY,
        JetPositive,
        PowerGate
    }
    [SerializeField]
    private enum GateMove
    {
        Dinamic,
        Static
    }
    #endregion
    void Start()
    {
        _startPos = transform.position;
        GenerateNumber();
    }
    private void Update()
    {
        switch (gateMove)
        {
            case GateMove.Dinamic:
                _movePos.x = _startPos.x + Mathf.Sin(Time.timeSinceLevelLoad * moveSpeed) * moveLimit;
                transform.position = new Vector3(_movePos.x, transform.position.y, transform.position.z);
                break;
            case GateMove.Static:
                break;
        }
    }
    public int GetGateNumber()
    {
        return gateNumber;
    }
    private void GenerateNumber()
    {
        switch (gateType)
        {
            case GateType.PositiveGate:
                gateNumber = Random.Range(minValue + GameManager.instance1.levelIndex, maxValue + (3 * GameManager.instance1.levelIndex));
                gateText.text = "+" + gateNumber.ToString();
                break;
            case GateType.NegativeGate:
                gateNumber = Random.Range(-maxValue - (3 * GameManager.instance1.levelIndex), -minValue + GameManager.instance1.levelIndex);
                if (gateNumber > 0) gateNumber = -1;
                gateText.text = gateNumber.ToString();
                break;
            case GateType.PositiveGateX:
                gateNumber = Random.Range(minValue, maxValue);
                gateText.text = "x" + gateNumber.ToString();
                break;
            case GateType.NegativeGateY:
                gateNumber = Random.Range(minValue, maxValue);
                gateText.text = "÷" + gateNumber.ToString();
                break;
            case GateType.JetPositive:
                gateNumber = Random.Range(minValue, maxValue);
                gateText.text = gateNumber.ToString();
                break;
        }
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}

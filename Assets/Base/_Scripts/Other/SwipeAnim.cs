using UnityEngine;

public class SwipeAnim : MonoBehaviour
{
    [SerializeField] private float moveLimit;
    [SerializeField] private float moveSpeed;
    private Vector3 _startPos;
    private Vector3 _movePos;
    void Awake()
    {
        _startPos = transform.position;
    }
    void Update()
    {
        _movePos.x = _startPos.x + Mathf.Sin(Time.timeSinceLevelLoad * moveSpeed) * moveLimit;
        transform.position = new Vector3(_movePos.x, transform.position.y, transform.position.z);
    }
}

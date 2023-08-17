using UnityEngine;
using TMPro;

public class CloneManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float lerpValue;
    private PlayerManager _plManager;
    private TMP_Text _cloneText;
    void Awake()
    {
        targetTransform = GameObject.FindWithTag("Target").GetComponent<Transform>();
    }
    void Update()
    {
        if (transform.parent == null)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, lerpValue * Time.deltaTime);
        }
        transform.LookAt(targetTransform);
        if (targetTransform.gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(false);
        }
        if (transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.activeSelf) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.SetActive(true);
    }
}

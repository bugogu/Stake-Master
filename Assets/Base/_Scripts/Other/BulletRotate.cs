using UnityEngine;

public class BulletRotate : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 2, 0);
        if (transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.activeSelf) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(PlayerPrefs.GetInt("SkinID")).gameObject.SetActive(true);
    }
}

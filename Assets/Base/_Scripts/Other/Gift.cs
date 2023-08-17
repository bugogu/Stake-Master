using UnityEngine;

public class Gift : MonoBehaviour
{
    private void Update()
    {
        if(gameObject.tag == "Bomb")
        {
            transform.Rotate(new Vector3(0, 300 * Time.deltaTime, 0));
        }
        else 
        {
            transform.Rotate(new Vector3(0, 150 * Time.deltaTime, 0));
        }
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}

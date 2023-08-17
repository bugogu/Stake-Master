using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(300 * Time.deltaTime, 0, 0));
    }
    private void FixedUpdate()
    {
        transform.Translate(transform.forward * -GameManager.instance1.speed);
    }
    private void OnEnable()
    {
        transform.position = new Vector3(Random.Range(-GameManager.instance1.widthX, GameManager.instance1.widthX), transform.position.y, transform.position.z);
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}

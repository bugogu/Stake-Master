using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Start()
    {
        AudioSource music = GetComponent<AudioSource>();
        music.Play();
        DontDestroyOnLoad(transform.gameObject);
    }
}

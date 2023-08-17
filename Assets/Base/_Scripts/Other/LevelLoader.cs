using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitiontime;
    public void LoadNextLevel(int tankNull)
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + tankNull));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitiontime);
        SceneManager.LoadScene(levelIndex);
    }
}

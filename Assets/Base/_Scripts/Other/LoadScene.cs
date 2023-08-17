using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LoadScene : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    public Slider sld;
    void Awake()
    {
        if (PlayerPrefs.GetString("FirstLoad", "True") == "False") return;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("FirstLoad", "False");
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(LoadingScene());
    }
    IEnumerator LoadingScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressText.text = "Loading % " + (progress * 100).ToString("F");
            sld.value = progress;
            yield return null;
        }
    }
}

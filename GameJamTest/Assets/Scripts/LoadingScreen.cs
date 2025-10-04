using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;

    void Start()
    {
        if (progressBar == null)
            progressBar = GameObject.Find("ProgressBar")?.GetComponent<Slider>();

        progressBar.value = 0f;

        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
        asyncLoad.allowSceneActivation = false;

        float currentProgress = 0f;
        float targetProgress = 0f;

        yield return new WaitForSeconds(0.5f);

        while (!asyncLoad.isDone)
        {
            targetProgress = asyncLoad.progress;

            while (currentProgress < targetProgress)
            {
                currentProgress += Time.deltaTime * 0.5f; 
                currentProgress = Mathf.Min(currentProgress, targetProgress);

                UpdateProgressUI(currentProgress);
                yield return null;
            }

            if (asyncLoad.progress >= 0.9f)
            {
                while (currentProgress < 1f)
                {
                    currentProgress += Time.deltaTime * 0.3f; // 最后更慢
                    currentProgress = Mathf.Min(currentProgress, 1f);

                    UpdateProgressUI(currentProgress);
                    yield return null;
                }

                // 最终确认显示100%
                UpdateProgressUI(1f);
                yield return new WaitForSeconds(0.5f);

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void UpdateProgressUI(float progress)
    {
        // 计算显示用的百分比 (0-100%)
        float displayProgress = progress * 100f;

        if (progressBar != null)
            progressBar.value = progress;
    }
}
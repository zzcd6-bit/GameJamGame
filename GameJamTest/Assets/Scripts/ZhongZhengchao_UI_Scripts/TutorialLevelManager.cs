using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialLevel : MonoBehaviour
{
    [Header("目标污渍")]
    public Transform stainsParent;

    [Header("关卡设置")]
    public string nextLevelName = "MainLevel";

    private bool allStainsCleared = false;
    private bool thirdStepCompleted = false; // 新增：标记第三步是否完成

    void Start()
    {
        // 开始教学
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.StartTutorial();
        }
    }

    void Update()
    {
        // 检测操作完成
        CheckInputCompletion();

        // 检查污渍是否全部清除
        if (!allStainsCleared && stainsParent != null)
        {
            CheckAllStainsCleared();
        }

        // 新增：检测第三步完成条件（任意污渍被消除）
        CheckThirdStepCompletion();
    }

    void CheckInputCompletion()
    {
        if (TutorialManager.Instance == null) return;

        int currentStep = TutorialManager.Instance.GetCurrentStep();

        switch (currentStep)
        {
            case 0: // WASD移动光源
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    TutorialManager.Instance.CompleteCurrentStep();
                }
                break;

            case 1: // 箭头移动人物
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow))
                {
                    TutorialManager.Instance.CompleteCurrentStep();
                }
                break;

            case 2: // 触碰消化 - 通过污渍消除来检测
                // 这里不检测按键，通过CheckThirdStepCompletion来检测
                break;
        }
    }

    // 新增：检测第三步完成条件
    void CheckThirdStepCompletion()
    {
        if (TutorialManager.Instance == null) return;
        if (thirdStepCompleted) return; // 如果已经完成，不再检测

        int currentStep = TutorialManager.Instance.GetCurrentStep();

        // 只有在第三步且还有污渍存在时才检测
        if (currentStep == 2 && stainsParent != null && stainsParent.childCount > 0)
        {
            // 检查是否有污渍被消除（active = false）
            foreach (Transform stainTransform in stainsParent)
            {
                if (!stainTransform.gameObject.activeSelf)
                {
                    // 有污渍被消除了，完成第三步
                    thirdStepCompleted = true;
                    TutorialManager.Instance.CompleteCurrentStep();
                    break;
                }
            }
        }
    }

    void CheckAllStainsCleared()
    {
        if (stainsParent != null && stainsParent.childCount > 0)
        {
            bool allStainsClearedCheck = true;

            foreach (Transform stainTransform in stainsParent)
            {
                if (stainTransform.gameObject.activeSelf)
                {
                    allStainsClearedCheck = false;
                    break;
                }
            }

            if (allStainsClearedCheck)
            {
                allStainsCleared = true;
                StartCoroutine(LoadNextLevel());
            }
        }
    }

    IEnumerator LoadNextLevel()
    {
        Debug.Log("All stains cleared! Loading next level...");

        yield return new WaitForSeconds(2f);

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("Next level name is not set!");
        }
    }
}
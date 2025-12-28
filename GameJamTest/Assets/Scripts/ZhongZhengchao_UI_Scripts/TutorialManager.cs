using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    public string description;
    public string requiredInput;
    public GameObject tutorialPanel;
    public bool isCompleted = false;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("引导Panel")]
    public GameObject lightMovePanel;    // WASD移动光源Panel
    public GameObject characterMovePanel; // 箭头移动人物Panel
    public GameObject digestPanel;       // 触碰消化Panel

    private int currentStepIndex = 0;
    private bool isTutorialActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        HideAllPanels();
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStepIndex = 0;
        ShowCurrentStep();
    }

    void HideAllPanels()
    {
        if (lightMovePanel != null) lightMovePanel.SetActive(false);
        if (characterMovePanel != null) characterMovePanel.SetActive(false);
        if (digestPanel != null) digestPanel.SetActive(false);
    }

    void ShowCurrentStep()
    {
        if (!isTutorialActive) return;

        HideAllPanels();

        switch (currentStepIndex)
        {
            case 0:
                if (lightMovePanel != null) lightMovePanel.SetActive(true);
                break;
            case 1:
                if (characterMovePanel != null) characterMovePanel.SetActive(true);
                break;
            case 2:
                if (digestPanel != null) digestPanel.SetActive(true);
                break;
        }
    }

    public void CompleteCurrentStep()
    {
        if (!isTutorialActive) return;

        HideAllPanels();
        currentStepIndex++;

        if (currentStepIndex < 3)
        {
            StartCoroutine(ShowNextStepAfterDelay(1.0f));
        }
        else
        {
            EndTutorial();
        }
    }

    IEnumerator ShowNextStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowCurrentStep();
    }

    void EndTutorial()
    {
        isTutorialActive = false;
        HideAllPanels();
        Debug.Log("Tutorial completed");
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }

    public int GetCurrentStep()
    {
        return currentStepIndex;
    }
}
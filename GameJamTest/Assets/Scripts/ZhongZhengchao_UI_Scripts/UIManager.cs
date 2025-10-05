using LANSHEN_SCRIPTS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("暂停界面")]
    public GameObject pauseCanvas;
    public Button resumeButton;
    public Button restartButton;
    public Button quitToMenuButton;

    [Header("通关界面")]
    public GameObject winCanvas; // 新增：通关界面
    public Transform goalsParent; // 目标物体的父对象

    Coroutine routine;
    private bool isPaused = false;

    void Awake()
    {
        // 单例模式
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
        // 自动查找UI元素
        FindUIElements();

        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(ResetGameState);
        quitToMenuButton?.onClick.AddListener(QuitToMainMenu);

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        routine = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // 如果已经在显示通关界面，则不再检测
        if (routine != null || (winCanvas != null && winCanvas.activeSelf))
        {
            return;
        }

        // 检测goalsParent是否有子对象
        if (goalsParent != null && goalsParent.childCount > 0)
        {
            // 如果有子对象，检查是否所有目标都已完成
            bool allGoalsCompleted = true;

            foreach (Transform goalTransform in goalsParent)
            {
                var goal = goalTransform.GetComponent<GoalScript>();
                if (goal != null && goal.hide == false)
                {
                    allGoalsCompleted = false; // 还有未完成的目标
                    break;
                }
            }

            // 如果所有目标都完成了，显示通关界面
            if (allGoalsCompleted)
            {
                routine = StartCoroutine(ShowWin());
            }
        }
    }

    IEnumerator ShowWin()
    {
        yield return null;

        // 方法1：使用winCanvas变量
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Time.timeScale = 0f; // 通关时暂停游戏
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            yield break;
        }

        routine = null; // 重置routine
    }
    public void TogglePause()
    {
        if (winCanvas != null && winCanvas.activeSelf)
            return;

        isPaused = !isPaused;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void ResetGameState()
    {
        Debug.Log("重置游戏状态");

        // 1. 恢复游戏时间
        Time.timeScale = 1f;

        // 2. 隐藏暂停界面
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        //3. 重新加载界面
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        // . 重置游戏状态标志
        isPaused = false;
        routine = null;

        Debug.Log("游戏状态重置完成");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        isPaused = false;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        SceneManager.LoadScene(0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIElements();

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
    }

    void FindUIElements()
    {
        if (pauseCanvas == null)
            pauseCanvas = GameObject.Find("PauseCanvas");

        if (resumeButton == null)
            resumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();
        if (restartButton == null)
            restartButton = GameObject.Find("RestartButton")?.GetComponent<Button>();
        if (quitToMenuButton == null)
            quitToMenuButton = GameObject.Find("QuitToMenuButton")?.GetComponent<Button>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }

    // 重置方法
    public void SoftReset()
    {
        ResetGameState();
    }
}
using LANSHEN_SCRIPTS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("��ͣ����")]
    public GameObject pauseCanvas;
    public Button resumeButton;
    public Button restartButton;
    public Button quitToMenuButton;

    [Header("ͨ�ؽ���")]
    public GameObject winCanvas; // ������ͨ�ؽ���
    public Transform goalsParent; // Ŀ������ĸ�����

    Coroutine routine;
    private bool isPaused = false;

    void Awake()
    {
        // ����ģʽ
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
        // �Զ�����UIԪ��
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

        // ����Ѿ�����ʾͨ�ؽ��棬���ټ��
        if (routine != null || (winCanvas != null && winCanvas.activeSelf))
        {
            return;
        }

        // ���goalsParent�Ƿ����Ӷ���
        if (goalsParent != null && goalsParent.childCount > 0)
        {
            // ������Ӷ��󣬼���Ƿ�����Ŀ�궼�����
            bool allGoalsCompleted = true;

            foreach (Transform goalTransform in goalsParent)
            {
                var goal = goalTransform.GetComponent<GoalScript>();
                if (goal != null && goal.hide == false)
                {
                    allGoalsCompleted = false; // ����δ��ɵ�Ŀ��
                    break;
                }
            }

            // �������Ŀ�궼����ˣ���ʾͨ�ؽ���
            if (allGoalsCompleted)
            {
                routine = StartCoroutine(ShowWin());
            }
        }
    }

    IEnumerator ShowWin()
    {
        AudioManager.PlaySound("Victory");
        yield return null;

        // ����1��ʹ��winCanvas����
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Time.timeScale = 0f; // ͨ��ʱ��ͣ��Ϸ
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            yield break;
        }

        routine = null; // ����routine
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
        Debug.Log("������Ϸ״̬");

        // 1. �ָ���Ϸʱ��
        Time.timeScale = 1f;

        // 2. ������ͣ����
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        //3. ���¼��ؽ���
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        //4 . ������Ϸ״̬��־
        isPaused = false;
        routine = null;

        Debug.Log("��Ϸ״̬�������");
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

    // ���÷���
    public void SoftReset()
    {
        ResetGameState();
    }
}
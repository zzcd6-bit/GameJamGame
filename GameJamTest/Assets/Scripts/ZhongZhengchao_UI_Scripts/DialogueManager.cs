using LANSHEN_SCRIPTS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueText;

    private Queue<string> sentences;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;
    private Transform targetCharacter;
    private float currentXOffset = 10f;  // 默认值
    private float currentYOffset = 60f;  // 默认值

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        sentences = new Queue<string>();
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Update dialogue position to follow character
        if (isDialogueActive && targetCharacter != null)
        {
            UpdateDialoguePosition();
        }

        // Space to continue
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("2");
            //DisplayNextSentence();
            EndDialogue();
        }
    }

    public void StartDialogue(string[] dialogueLines, Transform character = null, float xOffset = 10f, float yOffset = 60f)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            return;
        }

        // 保存偏移量
        currentXOffset = xOffset;
        currentYOffset = yOffset;

        targetCharacter = character;
        sentences.Clear();

        foreach (string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // 设置对话文本
        if (dialogueText != null && sentences.Count > 0)
        {
            string firstSentence = sentences.Peek();
            dialogueText.text = firstSentence;
            Debug.Log($"设置对话文本: {firstSentence}");
        }

        // 显示继续提示
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }

        isDialogueActive = true;

        Debug.Log($"开始对话，偏移量: ({xOffset}, {yOffset})");
    }

    void UpdateDialoguePosition()
    {
        if (dialoguePanel == null || targetCharacter == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetCharacter.position);
        Debug.Log($"世界坐标: {targetCharacter.position}, 屏幕坐标: {screenPos}");

        // 使用从DialogueInteractionObject传递的偏移量
        screenPos.x += currentXOffset;
        screenPos.y += currentYOffset;

        // 确保在屏幕范围内
        screenPos.x = Mathf.Clamp(screenPos.x, 100f, Screen.width - 100f);
        screenPos.y = Mathf.Clamp(screenPos.y, 100f, Screen.height - 100f);

        dialoguePanel.transform.position = screenPos;

        Debug.Log($"对话位置: {dialoguePanel.transform.position}, 偏移量: ({currentXOffset}, {currentYOffset})");
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isDialogueActive = false;
        targetCharacter = null;

        // Unfreeze player
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.freezed = false;
        }

        Debug.Log("7");
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
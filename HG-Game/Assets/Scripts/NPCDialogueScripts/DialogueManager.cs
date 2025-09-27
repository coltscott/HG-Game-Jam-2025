using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;
    public Button nextButton;

    [Header("Settings")]
    public float textSpeed = 0.05f;
    public KeyCode nextDialogueKey = KeyCode.Space;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;

    public static DialogueManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

    void Update()
    {
        if (dialogueActive && Input.GetKeyDown(nextDialogueKey))
        {
            if (isTyping)
                CompleteCurrentLine();
            else
                NextLine();
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueActive = true;

        dialoguePanel.SetActive(true);
        npcNameText.text = dialogue.npcName;

        // Pause game if needed
        Time.timeScale = 0f; // Remove this if you want dialogue during gameplay

        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentDialogue.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void CompleteCurrentLine()
    {
        StopAllCoroutines();
        dialogueText.text = currentDialogue.dialogueLines[currentLineIndex].text;
        isTyping = false;
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        string currentLine = currentDialogue.dialogueLines[currentLineIndex].text;

        foreach (char letter in currentLine.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}
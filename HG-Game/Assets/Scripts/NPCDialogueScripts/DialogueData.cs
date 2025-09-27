using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;
        public float displayDuration = 2f; // Optional: auto-advance timing
    }

    public string npcName;
    public DialogueLine[] dialogueLines;
}

using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("NPC Settings")]
    public DialogueData dialogue;
    public float interactionRange = 2f;
    public Transform player;
    public GameObject interactionPrompt;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is in range
        if (distanceToPlayer <= interactionRange && !playerInRange)
        {
            playerInRange = true;
            ShowInteractionPrompt();
        }
        else if (distanceToPlayer > interactionRange && playerInRange)
        {
            playerInRange = false;
            HideInteractionPrompt();
        }

        // Handle interaction input
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Interact()
    {
        if (dialogue != null && DialogueManager.Instance != null)
        {
            HideInteractionPrompt();
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    // Visual debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
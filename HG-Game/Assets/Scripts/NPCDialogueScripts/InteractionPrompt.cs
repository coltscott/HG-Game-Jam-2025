using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [Header("Animation Settings")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Simple bobbing animation
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
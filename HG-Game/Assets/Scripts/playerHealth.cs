using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHearts = 2;
    private int currentHearts;

    private void Start()
    {
        currentHearts = maxHearts;
        Debug.Log("Player starting HP: " + currentHearts);
    }

    public void TakeDamage(int damage = 1)
    {
        currentHearts -= damage;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        Debug.Log("Player took damage! Current HP: " + currentHearts);

        if (currentHearts <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player dead!");
        gameObject.SetActive(false); // or Destroy(gameObject);
    }
}
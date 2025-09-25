using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;      // units per second
    public int damage = 1;        // damage to player

    [Header("Visual Spin")]
    public bool spinVisual = true;
    public float spinSpeed = 360f; // degrees per second
    public Transform visual;       // assign the child sprite here

    private void Update()
    {
        // Move projectile straight along local +X
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Rotate only the visual child
        if (spinVisual && visual != null)
        {
            visual.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Player hit by projectile! HP now: " + ph);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
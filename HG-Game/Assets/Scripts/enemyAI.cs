using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged }
    public EnemyType enemyType;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public bool canJump = false;
    public bool canClimbWalls = false;

    [Header("Combat")]
    public int damage = 1;
    public float attackRange = 1.5f;
    public GameObject projectilePrefab; // For ranged atks
    public Transform firePoint;
    public bool damageOnTouch = true;

    [Header("Command Pattern")]
    [SerializeField] private string[] commandSequence; // Ex: {"Walk", "Attack", "Turn", "Idle"}
    public float commandDelay = 2f; // Duration of command
    private int commandIndex = 0;
    private bool isExecuting = false;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    private void Start()
    {
        Debug.Log("EnemyAI Start called!");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(CommandLoop());
    }

    private IEnumerator CommandLoop()
    {
        while (true)
        {
            if (commandSequence.Length > 0 && !isExecuting)
            {
                string command = commandSequence[commandIndex];
                yield return ExecuteCommand(command);

                commandIndex = (commandIndex + 1) % commandSequence.Length; // Loop commands
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator ExecuteCommand(string command)
    {
        isExecuting = true;
        // Debug.Log("Executing command: " + command);

        switch (command)
        {
            case "Walk":
                // Debug.Log("Triggering Walk");
                anim.SetTrigger("Walk");

                yield return new WaitForSeconds(0.2f);

                float timer = 0f;
                while (timer < commandDelay)
                {
                    rb.linearVelocity = new Vector2(moveSpeed * transform.localScale.x, rb.linearVelocity.y);
                    timer += Time.deltaTime;
                    yield return null;
                }

                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Idle");
                break;

            case "Attack":
                anim.SetTrigger("Attack");

                // Melee attack
                if (enemyType == EnemyType.Melee && player != null)
                {
                    if (Vector2.Distance(transform.position, player.position) <= attackRange)
                        player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
                }
                // Ranged attack
                else if (enemyType == EnemyType.Ranged && projectilePrefab && firePoint)
                {
                    // Spawn 
                    yield return new WaitForSeconds(1.2f);
                    GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

                    // Determine direction based on enemy facing
                    float direction = Mathf.Sign(transform.localScale.x); // +1 = right, -1 = left

                    // Flip projectile to face the correct direction
                    Vector3 projScale = proj.transform.localScale;
                    projScale.x = Mathf.Abs(projScale.x) * direction;
                    proj.transform.localScale = projScale;

                    // Set projectile speed if it has EnemyProjectile script
                    EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
                    if (ep != null)
                    {
                        ep.speed = Mathf.Abs(ep.speed) * direction; // move left if enemy facing left
                    }
                }

                yield return new WaitForSeconds(commandDelay);
                anim.SetTrigger("Idle");
                break;

            case "Idle":
                // Debug.Log("Triggering Idle");
                anim.SetTrigger("Idle");
                rb.linearVelocity = Vector2.zero;
                yield return new WaitForSeconds(commandDelay);
                break;

            case "Turn":
                // Debug.Log("Triggering Turn");
                // Flip that boy
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                yield return new WaitForSeconds(0.2f);
                break;

            case "Jump":
                if (canJump)
                {
                    // Debug.Log("Triggering Jump");
                    anim.SetTrigger("Jump");
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                yield return new WaitForSeconds(commandDelay);
                anim.SetTrigger("Idle");
                break;
        }

        isExecuting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!damageOnTouch) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Player hit by enemy on touch! HP now: " + ph);
            }
        }
    }
}

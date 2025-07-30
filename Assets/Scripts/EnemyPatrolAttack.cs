using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class EnemyPatrolAttack : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("Attack Settings")]
    public Transform player;
    public float attackRange = 2f;
    public LayerMask obstacleMask;
    public float visionDistance = 10f;

    private Animator animator;
    private AudioSource audioSource;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isPlayerVisible = false;
    private Vector3 originalScale;

    [Header("Attack Info")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    [Header("Audio Clips")]
    public AudioClip attackClip;
    public AudioClip deathClip;

    [Header("Health Settings")]
    public int maxHealth = 5;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDead || FindObjectOfType<GameManager>().isGameActive == false)
            return;

        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        isPlayerVisible = CanSeePlayer();

        if (isPlayerVisible)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsWalking", false);
            FacePlayer();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
            Patrol();
        }
    }

    public void Attack()
    {
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);

        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (collInfo)
        {
            PlayerMovement playerScript = collInfo.gameObject.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Die();
        }
    }

    void Patrol()
    {
        animator.SetBool("IsWalking", true);

        float moveDirection = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDirection * patrolSpeed * Time.deltaTime);

        if (movingRight)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        if (movingRight && transform.position.x >= startPosition.x + patrolDistance)
            movingRight = false;
        else if (!movingRight && transform.position.x <= startPosition.x - patrolDistance)
            movingRight = true;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > attackRange || distanceToPlayer > visionDistance)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
        return hit.collider == null;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        // Optional: delay destruction to allow death animation and sound to play
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);

            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
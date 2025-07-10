using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyPatrolAttack : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float patrolSpeed = 2f;

    [Header("Attack Settings")]
    public Transform player;
    public float attackRange = 2f;
    public LayerMask obstacleMask;
    public float visionDistance = 5f;

    private Animator animator;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isPlayerVisible = false;
    private Vector3 originalScale;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        // Check if player is in vision
        isPlayerVisible = CanSeePlayer();

        if (isPlayerVisible)
        {
            // Attack
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsWalking", false);

            // Face player
            FacePlayer();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
            Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("IsWalking", true);

        // Move
        float moveDirection = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDirection * patrolSpeed * Time.deltaTime);

        // Flip back to patrol direction
        if (movingRight)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Change direction at patrol limits
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

        // 1. Out of attack range?
        if (distanceToPlayer > attackRange)
            return false;

        // 2. Out of vision distance?
        if (distanceToPlayer > visionDistance)
            return false;

        // 3. Check obstacles (bushes etc.)
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
        if (hit.collider != null)
        {
            // There's something blocking view
            return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        // Optional: visualize vision in editor
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
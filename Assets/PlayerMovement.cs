using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    public Text health;
    public Animator animator;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    public int MaxHealth = 5;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (MaxHealth <= 0)
        {
            Die();
        }

        health.text = MaxHealth.ToString();

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Set running animation
        animator.SetFloat("Run", Mathf.Abs(move));

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("Jump", true);
        }

        // Flip sprite
        if (move < 0f && facingRight)
            Flip();
        else if (move > 0f && !facingRight)
            Flip();

        // Attack handling
        if (Input.GetMouseButtonDown(0))
        {
            if (isGrounded)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("JumpAttack");
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Handle hiding behind bush
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 0;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 2;
        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            if(collInfo.gameObject.GetComponent<EnemyPatrolAttack>() != null)
            {
                collInfo.gameObject.GetComponent<EnemyPatrolAttack>().TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        if (MaxHealth <= 0) {
            return;
        }
        MaxHealth -= damage;
    }

    void Die()
    {
        Debug.Log("Player Dies.");
    }
}
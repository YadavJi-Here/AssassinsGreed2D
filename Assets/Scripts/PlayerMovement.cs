using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Text coinText;
    public int currentCoin = 0;
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

        coinText.text = currentCoin.ToString();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle hiding behind bush
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 0;
        }

        // Handle coin collection
        if (other.CompareTag("Coin"))
        {
            currentCoin += 1;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collect");
            Destroy(other.gameObject, 1f); // Delay to allow animation to play
        }

        if (other.gameObject.tag == "VictoryPoint")
        {
            Debug.Log("Victory Point Reached!");
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
            var enemy = collInfo.gameObject.GetComponent<EnemyPatrolAttack>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
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
        if (MaxHealth <= 0)
        {
            return;
        }
        MaxHealth -= damage;
    }

    void Die()
    {
        Debug.Log("Player Dies.");
        FindObjectOfType<GameManager>().isGameActive = false;
        Destroy(this.gameObject);
    }
}
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
    private bool isDead = false;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (isDead || (gameManager != null && !gameManager.isGameActive))
            return;

        coinText.text = currentCoin.ToString();
        health.text = MaxHealth.ToString();

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("Run", Mathf.Abs(move));

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("Jump", true);
        }

        if (move < 0f && facingRight)
            Flip();
        else if (move > 0f && !facingRight)
            Flip();

        if (Input.GetMouseButtonDown(0))
        {
            FindObjectOfType<AudioManager>().PlayAudio();

            if (isGrounded)
                animator.SetTrigger("Attack");
            else
                animator.SetTrigger("JumpAttack");
        }

        if (MaxHealth <= 0)
        {
            Die();
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
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 0;
        }

        if (other.CompareTag("Coin"))
        {
            currentCoin += 1;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collect");
            Destroy(other.gameObject, 1f);
        }

        if (other.CompareTag("VictoryPoint"))
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
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        if (MaxHealth <= 0 || isDead)
            return;

        MaxHealth -= damage;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager not found in Die()");
        }
        rb.bodyType = RigidbodyType2D.Static;
    }
}
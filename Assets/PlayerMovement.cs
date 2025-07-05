using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y); // fixed typo: linearVelocity ? velocity

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Jump", true);
        }

        // Flip the player when changing direction
        if (move < 0f && facingRight)
            Flip();
        else if (move > 0f && !facingRight)
            Flip();

        animator.SetFloat("Run", Mathf.Abs(move)); // simpler and more accurate

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
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

    // Handle hiding behind bush using trigger area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 0; // Behind bush
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bush"))
        {
            spriteRenderer.sortingOrder = 2; // In front of bush
        }
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.up * jumpForce;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Oyun Bitti!");
            Time.timeScale = 0;
        }
    }
}
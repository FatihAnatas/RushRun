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
    private ScoreManager scoreManager;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && (Input.GetKeyDown(KeyCode.Mouse0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Jump();
        }

        if (transform.position.y < -7f || transform.position.x < -10f)
        {
            FinishGame();
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.up * jumpForce;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {

            FindFirstObjectByType<GameEndManager>().GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            FindFirstObjectByType<ScoreManager>().StopScore();

            FindFirstObjectByType<GameEndManager>().GameOver();
        }
    }

    void FinishGame()
    {
        GameEndManager lm = FindFirstObjectByType<GameEndManager>();
        if (lm != null)
        {
            lm.GameOver();
        }
    }
}
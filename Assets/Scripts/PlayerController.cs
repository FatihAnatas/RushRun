using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Fizik Ayarlarý")]
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    [Header("Kayma (Slide) Ayarlarý")]
    public float slideDuration = 0.7f; 
    public float swipeThreshold = 50f; 
    public Vector2 slideColliderSize = new Vector2(1f, 0.5f); 
    public Vector2 slideColliderOffset = new Vector2(0f, -0.25f); 

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D col;
    private ScoreManager scoreManager;

    private bool isGrounded;
    private bool jumpRequested;
    private bool isSliding;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        
        originalColliderSize = col.size;
        originalColliderOffset = col.offset;
    }

    void Update()
    {
        HandleInput();
        UpdateAnimations();

       
        if (transform.position.y < -7f || transform.position.x < -10f)
        {
            FinishGame();
        }
    }

    void HandleInput()
    {

        if (Input.GetMouseButtonDown(0) && isGrounded && !isSliding)
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        float verticalDistance = endTouchPosition.y - startTouchPosition.y;

        
        if (verticalDistance < -swipeThreshold && isGrounded && !isSliding)
        {
            StartSlide();
        }
        
        else if (Mathf.Abs(verticalDistance) < swipeThreshold && isGrounded)
        {
            jumpRequested = true;
        }
    }

    void StartSlide()
    {
        isSliding = true;
        col.size = slideColliderSize; 
        col.offset = slideColliderOffset;

        
        Invoke("StopSlide", slideDuration);
    }

    void StopSlide()
    {
        isSliding = false;
        col.size = originalColliderSize; 
        col.offset = originalColliderOffset;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, checkRadius, groundLayer);

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    void Jump()
    {
        
        if (isSliding) StopSlide();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("jump"); 
    }

    void UpdateAnimations()
    {
        
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

  
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) HandleGameOver();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) HandleGameOver();
    }

    void HandleGameOver()
    {
        if (scoreManager != null) scoreManager.StopScore();
        GameEndManager gem = FindFirstObjectByType<GameEndManager>();
        if (gem != null) gem.GameOver();
    }

    void FinishGame()
    {
        GameEndManager lm = FindFirstObjectByType<GameEndManager>();
        if (lm != null) lm.GameOver();
    }
}
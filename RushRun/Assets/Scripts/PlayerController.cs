using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    public static bool isGameOver = false; 

    [Header("Oyun Modu Ayarý")]
    public bool isLevelMode = false; // Bunu iþaretlersen karakter saða koþar
    public float runSpeed = 8f;      // Level modunda ne kadar hýzlý koþacak?

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
    
    private bool isLevelFinished = false;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        isGameOver = false; 

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        originalColliderSize = col.size;
        originalColliderOffset = col.offset;
    }

    void Update()
    {
        // KÝLÝT: Eðer bölüm bittiyse aþaðýdaki kodlarý çalýþtýrma (Karakter dursun)
        if (isLevelFinished) return;

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
        // KÝLÝT: Eðer bölüm bittiyse fiziksel hýzý 0 yap ve çýk
        if (isLevelFinished)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(transform.position, checkRadius, groundLayer);

        if (isLevelMode)
        {
            // Level Modu: Karakter fiziksel olarak saða gider
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Sonsuz Mod: Karakterin X hýzý 0'dýr
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

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
        // Diken/Tuzak çarparsa öl
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleGameOver();
        }
        // "Finish" etiketli portala çarparsa kazan
        else if (collision.gameObject.CompareTag("Finish"))
        {
            LevelCompleted();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) HandleGameOver();
    }

    void HandleGameOver()
    {

        isGameOver = true;
        isLevelFinished = true; // Karakteri kilitler
        rb.linearVelocity = Vector2.zero; // Hýzý sýfýrlar
        isSliding = false; // Kaymayý iptal eder

        if (anim != null)
        {
            anim.SetBool("isSliding", false);
            anim.SetTrigger("die"); // Ölüm animasyonunu tetikle
        }

        if (scoreManager != null)
        {
            scoreManager.StopScore(); // Skoru anýnda durdur
        }

        // 2. MODLARA GÖRE AYRIM: 1.5 saniye sonra ne olacak?
        if (isLevelMode)
        {
            // Level modundaysa 1.5 saniye sonra bölümü baþtan baþlat
            Invoke("RestartLevel", 1.0f);
        }
        else
        {
            // Sonsuz moddaysa 1.5 saniye sonra Game Over ekranýný getir
            Invoke("ShowGameOverScreen", 1.0f);
            
        }
    }

    // YENÝ FONKSÝYON: Sonsuz mod için gecikmeli Game Over ekraný
    void ShowGameOverScreen()
    {
        GameEndManager gem = FindFirstObjectByType<GameEndManager>();
        if (gem != null) gem.GameOver();
    }

    // YENÝ EKLENEN FONKSÝYON: Gecikmeli olarak sahneyi baþtan yükler
    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FinishGame()
    {
        // HandleGameOver ile ayný mantýkta çalýþsýn
        HandleGameOver();
        
    }

    // --- YENÝ EKLENEN: BÖLÜM GEÇME FONKSÝYONU ---
    void LevelCompleted()
    {
        Debug.Log("Tebrikler! Bölüm Bitti.");

        isLevelFinished = true; // Kilidi kapat
        rb.linearVelocity = Vector2.zero; // Hýzý sýfýrla

        if (anim != null)
        {
            anim.enabled = false;
        }

        // 1.5 saniye bekle, sonra Ana Menüye dön
        Invoke("LoadMainMenu", 0.5f);
    }

    void LoadMainMenu()
    {
        // "MainMenu" yazan yer, ana menü sahnene verdiðin tam isimle ayný olmalý!
        SceneManager.LoadScene("MainMenu");
    }
}
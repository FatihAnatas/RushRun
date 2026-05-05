using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    public static bool isGameOver = false; 

    [Header("Oyun Modu Ayarý")]
    public bool isLevelMode = false; 
    public float runSpeed = 8f;

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
        Application.targetFrameRate = 60;
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
        // 1. MOBÝL ÝÇÝN GERÇEK DOKUNMATÝK KONTROLÜ
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && isGrounded && !isSliding)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
        // 2. BÝLGÝSAYAR TESTLERÝ ÝÇÝN FARE KONTROLÜ
        else
        {
            if (Input.GetMouseButtonDown(0) && isGrounded && !isSliding)
            {
                startTouchPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchPosition = Input.mousePosition;
                DetectSwipe();
            }
        }
    }

    void DetectSwipe()
    {
        float verticalDistance = endTouchPosition.y - startTouchPosition.y;

        // Eðer parmaðý aþaðý doðru belli bir mesafe kaydýrdýysa (Kayma)
        if (verticalDistance < -swipeThreshold && isGrounded && !isSliding)
        {
            StartSlide();
        }
        // Aþaðý kaydýrmadýysa, parmaðýný çektiði an zýpla (Sýnýrý biraz gevþettik)
        else if (isGrounded && !isSliding)
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
        // KÝLÝT: Eðer bölüm bittiyse sadece yatay hýzý (X) sýfýrla. 
        // Y hýzýný (yerçekimini) sýfýrlamýyoruz ki karakter havada donmasýn, yere düþüp yatsýn!
        if (isLevelFinished)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        isGrounded = Physics2D.OverlapCircle(transform.position, checkRadius, groundLayer);

        if (isLevelMode)
        {
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }
        else
        {
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
        // 1. DURUM: Diken veya Varil (Kutu KAPANMAZ, karakter zemine çarpýp üstünde yatar)
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleGameOver();
        }
        // 2. DURUM: Zehirli Su (Ýllüzyon Taktiði çalýþýr, kutu KAPANIR ve lüp diye suya batar)
        else if (collision.gameObject.CompareTag("Water"))
        {
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }
            HandleGameOver();
        }
        // 3. DURUM: Bitiþ çizgisi
        else if (collision.gameObject.CompareTag("Finish"))
        {
            LevelCompleted();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Katý tuzaklara (Kutu vb.) çarptýðýnda Collider'ý KAPATMIYORUZ 
        // ki karakter kutunun üstünde/yanýnda dursun, yerin dibine düþmesin.
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleGameOver();
        }
    }

    void HandleGameOver()
    {
        isGameOver = true;
        isLevelFinished = true; // Karakteri kilitler
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Y hýzýný ellemiyoruz
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

        if (isLevelMode)
        {
            Invoke("RestartLevel", 1.0f);
        }
        else
        {
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
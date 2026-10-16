using UnityEngine;
using TMPro;

public class Planemovement : MonoBehaviour
{
    public static Planemovement Instance; 
    
    private Rigidbody2D rb;
    public float speed = 10f;
    private Vector2 movement;
    
    // --- HỆ THỐNG MÁU & HIỆU ỨNG ---
    public int maxHealth = 5;
    private int currentHealth;
    public GameObject explosionPrefab; 
    
    // --- HỆ THỐNG TIỀN/ĐIỂM & NÂNG CẤP ---
    private int currentCoins = 0; 
    public bool isUpgraded = false; 
    
    // --- GIAO DIỆN MÀN HÌNH ---
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI upgradePromptText; 
    public GameObject gameOverUI; 
    
    [Header("Khoảng lề để máy bay không bị lẹm ra ngoài")]
    public float paddingX = 0.5f; 
    public float paddingY = 0.5f;
    private float minX, maxX, minY, maxY;

    [Header("Hiệu ứng nhấp nháy chữ")]
    public float blinkSpeed = 3f; // Tốc độ nhấp nháy của chữ Upgrade

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        currentCoins = 0; 
        UpdateCoinUI();

        if (upgradePromptText != null)
        {
            upgradePromptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Chưa gán UI UpgradePromptText vào máy bay!");
        }
        
        Time.timeScale = 1f; 
        SetUpBoundaries();
    }

    void SetUpBoundaries()
    {
        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + paddingX;
        maxX = topRight.x - paddingX;
        minY = bottomLeft.y + paddingY;
        maxY = topRight.y - paddingY;
    }

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement = movement.normalized;

        // XỬ LÝ NHẤP NHÁY CHỮ UPGRADE KHI ĐANG HIỆN
        if (upgradePromptText != null && upgradePromptText.gameObject.activeInHierarchy)
        {
            Color textColor = upgradePromptText.color;
            // Mathf.PingPong chạy qua lại giữa 0 và 1, giúp Alpha (độ trong suốt) thay đổi liên tục
            textColor.a = Mathf.PingPong(Time.time * blinkSpeed, 1f); 
            upgradePromptText.color = textColor;
        }

        // Kiểm tra nâng cấp
        if (!isUpgraded && currentCoins >= 5 && Input.GetKeyDown(KeyCode.L))
        {
            TriggerUpgrade();
        }
    }

    void FixedUpdate()
    {
        Vector2 nextPosition = rb.position + movement * speed * Time.fixedDeltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);    
        rb.MovePosition(nextPosition);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyBullet>() != null || other.GetComponent<HomingBullet>() != null)
        {
            Destroy(other.gameObject); 
            currentHealth -= 1; 
            UpdateHealthUI();
            
            if (currentHealth <= 0)
            {
                if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, transform.rotation);
                if (gameOverUI != null) gameOverUI.SetActive(true);
                
                Time.timeScale = 0f;
                Destroy(gameObject); 
            }
        }
        else if (other.GetComponent<CoinDrop>() != null)
        {
            Destroy(other.gameObject); 
            currentCoins += 1; 
            UpdateCoinUI(); 

            if (currentCoins >= 5 && !isUpgraded)
            {
                if (upgradePromptText != null)
                {
                    upgradePromptText.text = "Bấm L để Nâng Cấp!";
                    upgradePromptText.gameObject.SetActive(true);
                }
            }
        }
        else if (other.GetComponent<HeartDrop>() != null)
        {
            Destroy(other.gameObject); 
            
            if (currentHealth < maxHealth)
            {
                currentHealth += 1; 
                UpdateHealthUI();   
            }
        }
    }

    void TriggerUpgrade()
    {
        isUpgraded = true;
        currentCoins -= 5;
        UpdateCoinUI();

        if (upgradePromptText != null)
        {
            // Trả lại độ trong suốt 100% trước khi tắt để đảm bảo lỗi hiển thị cho các lần sau (nếu dùng lại)
            Color textColor = upgradePromptText.color;
            textColor.a = 1f;
            upgradePromptText.color = textColor;
            
            upgradePromptText.gameObject.SetActive(false);
        }

        maxHealth = 7;
        currentHealth += 2;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.cyan;
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null) healthText.text = currentHealth + "/" + maxHealth;
    }

    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = "Coins: " + currentCoins;
    }
}
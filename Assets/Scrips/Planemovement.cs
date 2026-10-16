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
    public TextMeshProUGUI upgradePromptText; // Kéo Text thông báo "Bấm L để nâng cấp máy bay" vào đây
    public GameObject gameOverUI; 
    
    private float minX, maxX, minY, maxY;
    [Header("Khoảng lề để máy bay không bị lẹm ra ngoài")]
    public float paddingX = 0.5f; 
    public float paddingY = 0.5f;

    void Awake()
    {
        Instance = this; 

        // Tự động tìm kiếm nếu chưa kéo thả vào Inspector
        FindUpgradePromptText();
    }

    void FindUpgradePromptText()
    {
        if (upgradePromptText != null) return;

        // 1. Tìm GameObject đang active tên "Upgrade"
        GameObject upgradeObj = GameObject.Find("Upgrade");
        if (upgradeObj != null)
        {
            upgradePromptText = upgradeObj.GetComponent<TextMeshProUGUI>();
            return;
        }

        // 2. Tìm toàn bộ TextMeshProUGUI kể cả đang bị inactive
        TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in allTexts)
        {
            if (t != null && t.gameObject.name.Equals("Upgrade", System.StringComparison.OrdinalIgnoreCase))
            {
                upgradePromptText = t;
                return;
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        currentCoins = 0; 
        UpdateCoinUI();

        // Ẩn dòng thông báo nâng cấp lúc đầu game
        FindUpgradePromptText();
        if (upgradePromptText != null)
        {
            upgradePromptText.gameObject.SetActive(false);
        }
        else
        {
            GameObject fallbackObj = GameObject.Find("Upgrade");
            if (fallbackObj != null) fallbackObj.SetActive(false);
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

        // Kiểm tra nếu đủ 5 xu, chưa nâng cấp và người chơi bấm phím L
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
        // 1. Va chạm với ĐẠN CỦA ĐỊCH
        if (other.GetComponent<EnemyBullet>() != null)
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
        
        // 2. Va chạm với ĐỒNG XU
        else if (other.GetComponent<CoinDrop>() != null)
        {
            Destroy(other.gameObject); 
            currentCoins += 1; 
            UpdateCoinUI(); 

            // Khi đạt đủ 5 xu và chưa nâng cấp, hiện thông báo bấm L
            if (currentCoins >= 5 && !isUpgraded)
            {
                FindUpgradePromptText();
                if (upgradePromptText != null)
                {
                    upgradePromptText.text = "Bấm L để nâng cấp máy bay";
                    upgradePromptText.gameObject.SetActive(true);
                }
            }
        }

        // 3. Va chạm với TRÁI TIM (Hồi máu)
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

        // Trừ 5 xu của người chơi khi nâng cấp
        currentCoins -= 5;
        UpdateCoinUI();

        // Ẩn dòng thông báo sau khi đã bấm L nâng cấp
        FindUpgradePromptText();
        if (upgradePromptText != null)
        {
            upgradePromptText.gameObject.SetActive(false);
        }
        else
        {
            GameObject fallbackObj = GameObject.Find("Upgrade");
            if (fallbackObj != null) fallbackObj.SetActive(false);
        }

        // Tăng max máu lên 7 và hồi thêm +2 máu hiện tại
        maxHealth = 7;
        currentHealth += 2;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        
        UpdateHealthUI();

        // Đổi màu máy bay thành màu Xanh ngọc (Cyan) báo hiệu cường hóa
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.cyan;
        }

        Debug.Log("Đã nâng cấp thành công qua phím L!");
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + currentCoins;
        }
    }
}
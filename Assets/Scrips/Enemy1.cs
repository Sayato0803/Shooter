using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("Hệ thống rớt đồ")]
    public GameObject coinPrefab;
    [Range(0f, 100f)] public float dropChance = 70f; 
    
    public GameObject heartPrefab; 
    [Range(0f, 100f)] public float heartDropChance = 20f; 

    [Header("Hệ thống di chuyển & Bắn")]
    public Transform firePoint;
    public float speed = 3f; 
    public float stopY = 2.5f; 
    public GameObject enemyBulletPrefab; 
    public float fireRate = 1.5f; 
    private float nextFireTime;

    [Header("Cấu hình di chuyển qua lại")]
    public float horizontalSpeed = 3f; 
    public float patrolWidth = 2.5f;   
    private float startX;              
    private bool reachedStopY = false; 
    private int moveDirection = 1;     

    private float screenMinX, screenMaxX;
    public float paddingX = 0.8f;

    // --- HỆ THỐNG MÁU ---
    public int maxHealth = 5;
    private int currentHealth;
    public GameObject healthBarObject; 
    public Transform healthBarFill;    
    public SpriteRenderer healthBarSprite; 
    public GameObject explosionPrefab; 

    // --- CHỐT CHẶN CHỐNG LỖI ĐẾM KÉP ---
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarObject != null) healthBarObject.SetActive(false);

        Camera cam = Camera.main;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        screenMinX = bottomLeft.x + paddingX;
        screenMaxX = topRight.x - paddingX;
    }

    void Update()
    {
        if (!reachedStopY)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y <= stopY)
            {
                transform.position = new Vector3(transform.position.x, stopY, transform.position.z);
                startX = transform.position.x;
                reachedStopY = true;
            }
        }
        else
        {
            transform.Translate(Vector3.right * moveDirection * horizontalSpeed * Time.deltaTime);

            if (transform.position.x > Mathf.Min(startX + patrolWidth, screenMaxX))
            {
                moveDirection = -1;
            }
            else if (transform.position.x < Mathf.Max(startX - patrolWidth, screenMinX))
            {
                moveDirection = 1;
            }

            float clampedX = Mathf.Clamp(transform.position.x, screenMinX, screenMaxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        if (enemyBulletPrefab != null && firePoint != null)
        {
            Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.Euler(0,0,180));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        
        // Thêm điều kiện !isDead để đảm bảo quái chỉ nhận sát thương khi còn sống
        if (bullet != null && !isDead) 
        {
            Destroy(other.gameObject); 
            currentHealth -= bullet.damage; 
            if (healthBarObject != null) healthBarObject.SetActive(true);
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                // KHÓA LẠI NGAY LẬP TỨC: Tránh việc viên đạn thứ 2 bay vào cùng lúc gọi hàm này thêm lần nữa
                isDead = true; 
                
                if (explosionPrefab != null) 
                    Instantiate(explosionPrefab, transform.position, transform.rotation);
                
                float roll = Random.Range(0f, 100f);
                
                if (roll <= dropChance)
                {
                    if (coinPrefab != null) Instantiate(coinPrefab, transform.position, Quaternion.identity);
                }
                else if (roll <= dropChance + heartDropChance)
                {
                    if (heartPrefab != null) Instantiate(heartPrefab, transform.position, Quaternion.identity);
                }
                
                // Báo về Spawner để tính số lượng tiêu diệt an toàn
                if (EnemySpawner.Instance != null)
                {
                    EnemySpawner.Instance.EnemyKilled();
                }

                Destroy(gameObject);
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null || healthBarSprite == null) return;
        float healthPercent = (float)currentHealth / maxHealth;
        healthBarFill.localScale = new Vector3(healthPercent, 0.15f, 1f);

        if (healthPercent > 0.5f) {
            healthBarSprite.color = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2f);
        } else {
            healthBarSprite.color = Color.Lerp(Color.red, Color.yellow, healthPercent * 2f);
        }
    }
}
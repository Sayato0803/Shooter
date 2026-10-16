using UnityEngine;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Cài đặt Kẻ địch")]
    public GameObject enemyPrefab; 

    [Header("Cài đặt Thời gian sinh (Giây)")]
    public float minSpawnTime = 3f; 
    public float maxSpawnTime = 6f; 
    private float currentSpawnTime;

    [Header("Cài đặt Vị trí")]
    public float minX = -2.5f;    
    public float maxX = 2.5f;      
    public float spawnY = 5.5f;    

    [Header("Hệ thống Wave & UI")]
    public int currentWave = 1;
    public int enemiesKilled = 0;
    public int enemiesNeededForNextWave = 5; // Mặc định Wave 1 cần 5 con

    public TextMeshProUGUI WaveLabel;        
    public TextMeshProUGUI WaveAnnouncement; 

    private float timer;
    private bool canSpawn = false; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(BeginWaveRoutine());
    }

    IEnumerator BeginWaveRoutine()
    {
        canSpawn = false; 

        UpdateWaveUI();

        if (WaveAnnouncement != null)
        {
            WaveAnnouncement.text = "WAVE " + currentWave;
            WaveAnnouncement.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        if (WaveAnnouncement != null)
        {
            WaveAnnouncement.gameObject.SetActive(false);
        }

        canSpawn = true;
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;

        if (timer >= currentSpawnTime)
        {
            SpawnEnemy();
            timer = 0f; 
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // Wave 1 tối đa 3 con, từ Wave 2 trở đi tối đa 6 con trên màn hình
            int maxEnemiesOnScreen = (currentWave == 1) ? 3 : 6;
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemies < maxEnemiesOnScreen)
            {
                float randomX = Random.Range(minX, maxX);
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("Đã tiêu diệt 1 enemy! Tổng số đã giết: " + enemiesKilled);
        
        UpdateWaveUI(); 

        // Kiểm tra điều kiện chuyển wave
        if (currentWave == 1 && enemiesKilled >= enemiesNeededForNextWave)
        {
            StartWave2();
        }
        else if (currentWave == 2 && enemiesKilled >= enemiesNeededForNextWave)
        {
            StartWave3();
        }
        else if (currentWave == 3 && enemiesKilled >= enemiesNeededForNextWave)
        {
            // Xử lý khi hoàn thành Wave 3 (Ví dụ: Màn hình chiến thắng / Thắng game)
            Debug.Log("Đã hoàn thành Wave 3! Chiến thắng!");
        }
    }

    void StartWave2()
    {
        currentWave = 2;
        enemiesKilled = 0;                    
        enemiesNeededForNextWave = 10;        // Wave 2 yêu cầu 10 enemy

        minSpawnTime = 1.0f;
        maxSpawnTime = 2.5f;

        StartCoroutine(BeginWaveRoutine());
    }

    void StartWave3()
    {
        currentWave = 3;
        enemiesKilled = 0;                    
        enemiesNeededForNextWave = 15;        // Wave 3 yêu cầu 15 enemy (bạn có thể đổi số lượng này tùy ý)

        minSpawnTime = 0.7f;                  // Quái ra cực kỳ dồn dập
        maxSpawnTime = 2.0f;

        StartCoroutine(BeginWaveRoutine());
    }

    void UpdateWaveUI()
    {
        if (WaveLabel != null)
        {
            WaveLabel.text = "Wave " + currentWave + "\n" + enemiesKilled + "/" + enemiesNeededForNextWave;
        }
    }
}
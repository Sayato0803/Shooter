using UnityEngine;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Cài đặt Kẻ địch")]
    public GameObject enemyPrefab;              // Quái cho Wave 1 và 2
    public GameObject enemyPrefabWave3_Spread;  // Quái bắn 3 tia (Enemy2)
    public GameObject enemyPrefabWave3_Homing;  // Quái bắn đạn đuổi (Enemy3)

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
    
    public int enemiesSpawnedThisWave = 0; 
    public int totalEnemiesToSpawn = 5; 

    // Biến lưu trữ riêng cho Wave 3
    private int spreadSpawned = 0;
    private int homingSpawned = 0;

    public TextMeshProUGUI WaveLabel;        
    public TextMeshProUGUI WaveAnnouncement; 

    private float timer;
    private bool canSpawn = false; 

    void Awake() { Instance = this; }

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

        if (WaveAnnouncement != null) WaveAnnouncement.gameObject.SetActive(false);

        canSpawn = true;
    }

    void Update()
    {
        if (!canSpawn) return;

        if (enemiesSpawnedThisWave < totalEnemiesToSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= currentSpawnTime)
            {
                SpawnEnemy();
                timer = 0f; 
                currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }
    }

    void SpawnEnemy()
    {
        // Wave 1 và 3 chỉ cho phép 3 con, Wave 2 cho phép 5 con
        int maxEnemiesOnScreen = (currentWave == 2) ? 5 : 3;
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemies < maxEnemiesOnScreen)
        {
            GameObject prefabToSpawn = enemyPrefab;

            if (currentWave == 3)
            {
                // Lựa chọn ngẫu nhiên giữa bắn 3 tia và đạn đuổi (mỗi loại đúng 5 con)
                if (spreadSpawned < 5 && homingSpawned < 5)
                {
                    if (Random.value > 0.5f) { prefabToSpawn = enemyPrefabWave3_Spread; spreadSpawned++; }
                    else { prefabToSpawn = enemyPrefabWave3_Homing; homingSpawned++; }
                }
                else if (spreadSpawned < 5)
                {
                    prefabToSpawn = enemyPrefabWave3_Spread; spreadSpawned++;
                }
                else if (homingSpawned < 5)
                {
                    prefabToSpawn = enemyPrefabWave3_Homing; homingSpawned++;
                }
            }

            if (prefabToSpawn != null)
            {
                float randomX = Random.Range(minX, maxX);
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);
                
                GameObject newEnemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                enemiesSpawnedThisWave++;

                if (currentWave == 2)
                {
                    Enemy1 enemyScript = newEnemy.GetComponent<Enemy1>();
                    if (enemyScript != null) enemyScript.fireRate = 0.8f; 
                }
            }
        }
    }

    public void EnemyKilled()
    {
        if (enemiesKilled < totalEnemiesToSpawn) enemiesKilled++;
        UpdateWaveUI(); 

        if (currentWave == 1 && enemiesKilled >= totalEnemiesToSpawn)
        {
            StartWave2();
        }
        else if (currentWave == 2 && enemiesKilled >= totalEnemiesToSpawn)
        {
            StartWave3();
        }
        else if (currentWave == 3 && enemiesKilled >= totalEnemiesToSpawn)
        {
            Debug.Log("CHÚC MỪNG! BẠN ĐÃ CHIẾN THẮNG WAVE 3!");
            // Mở UI Thắng Game tại đây nếu có
        }
    }

    void StartWave2()
    {
        currentWave = 2;
        enemiesKilled = 0; enemiesSpawnedThisWave = 0; 
        
        totalEnemiesToSpawn = 10;   
        minSpawnTime = 1.0f; maxSpawnTime = 2.5f;

        StartCoroutine(BeginWaveRoutine());
        SpeedUpMap(3.5f);
    }

    void StartWave3()
    {
        currentWave = 3;
        enemiesKilled = 0; enemiesSpawnedThisWave = 0; 
        spreadSpawned = 0; homingSpawned = 0;
        
        totalEnemiesToSpawn = 10; // Giới hạn Wave 3 thành 10 con
        minSpawnTime = 1.5f; maxSpawnTime = 3.0f;

        StartCoroutine(BeginWaveRoutine());
        SpeedUpMap(5f); 
    }

    void UpdateWaveUI()
    {
        if (WaveLabel != null) WaveLabel.text = "Wave " + currentWave + "\n" + enemiesKilled + "/" + totalEnemiesToSpawn;
    }

    void SpeedUpMap(float newSpeed)
    {
        if (MapScroller.Instance != null) MapScroller.Instance.speed = newSpeed; 
    }
}
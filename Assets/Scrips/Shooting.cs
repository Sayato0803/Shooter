using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Loại đạn thường")]
    public GameObject bulletPrefab; 

    [Header("Loại đạn nâng cấp (sau khi bấm L)")]
    public GameObject upgradedBulletPrefab; // Nếu để trống sẽ tự dùng bulletPrefab để bắn đạn đôi

    [Header("Cấu hình vị trí bắn")]
    public Transform firePoint;
    public Transform leftFirePoint;  // (Tùy chọn) Gán nòng súng trái nếu có
    public Transform rightFirePoint; // (Tùy chọn) Gán nòng súng phải nếu có
    [Tooltip("Khoảng cách lệch 2 viên đạn khi tự động bắn 2 viên từ firePoint")]
    public float dualBulletOffset = 0.35f; 

    [Header("Tốc độ bắn")]
    public float fireRate = 0.3f; // Thời gian chờ giữa mỗi lần bắn
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        bool isUpgraded = Planemovement.Instance != null && Planemovement.Instance.isUpgraded;

        if (isUpgraded)
        {
            ShootDualBullets();
        }
        else
        {
            ShootSingleBullet();
        }
    }

    void ShootSingleBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void ShootDualBullets()
    {
        // Ưu tiên dùng loại đạn nâng cấp, nếu chưa gán thì dùng loại đạn thường
        GameObject bulletToFire = (upgradedBulletPrefab != null) ? upgradedBulletPrefab : bulletPrefab;
        if (bulletToFire == null) return;

        // Trường hợp 1: Có gán sẵn 2 nòng súng trái/phải trong Inspector
        if (leftFirePoint != null && rightFirePoint != null)
        {
            Instantiate(bulletToFire, leftFirePoint.position, leftFirePoint.rotation);
            Instantiate(bulletToFire, rightFirePoint.position, rightFirePoint.rotation);
        }
        // Trường hợp 2: Tự động tách 2 viên đạn bay song song 2 bên dựa theo firePoint
        else if (firePoint != null)
        {
            Vector3 leftPos = firePoint.position - firePoint.right * dualBulletOffset;
            Vector3 rightPos = firePoint.position + firePoint.right * dualBulletOffset;

            Instantiate(bulletToFire, leftPos, firePoint.rotation);
            Instantiate(bulletToFire, rightPos, firePoint.rotation);
        }
    }
}
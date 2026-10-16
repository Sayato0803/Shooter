using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 3.5f;       // Tốc độ đạn bay
    public float rotateSpeed = 150f; // Tốc độ rẽ hướng bám theo player
    private Transform target;

    void Start()
    {
        // Tự động quét tìm máy bay người chơi
        if (Planemovement.Instance != null)
        {
            target = Planemovement.Instance.transform;
        }
        
        // Đạn tự hủy sau 5 giây nếu không trúng
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target != null)
        {
            // Tìm hướng về phía player
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            direction.Normalize();

            // Tính toán bẻ lái đạn đuổi theo
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);
        }

        // Tiến tới trước
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
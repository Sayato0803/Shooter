using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1; // Sát thương của viên đạn

    void Start()
    {
        // Tự động xóa viên đạn khỏi bộ nhớ sau 1.5 giây để game không bị giật lag
        Destroy(gameObject, 1.5f);
    }

    void Update()
    {
        // Vector3.up tương đương với trục Y dương (bay thẳng lên trên)
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 0.5f; // Thời gian sống của vụ nổ (0.5 giây)

    void Start()
    {
        // Tự động xóa chính nó sau khoảng thời gian delay
        Destroy(gameObject, delay);
    }
}
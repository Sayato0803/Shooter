using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    public float speed = 3f; // Tốc độ rơi của đồng xu

    void Update()
    {
        // Rơi thẳng xuống dưới
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }
}
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public float speed = 2f;
    private float mapHeight; 

    void Start()
    {
        // Tự động tìm và đo chiều cao thực tế của tấm bản đồ con đầu tiên (Map 1)
        mapHeight = GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Di chuyển toàn bộ cụm 2 bản đồ cắm đầu xuống dưới
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y <= -mapHeight)
        {
            // Dịch chuyển tức thời toàn bộ cụm bản đồ ngược lên trên để bù trừ
            transform.position = new Vector3(transform.position.x, transform.position.y + mapHeight, transform.position.z);
        }
    }
}
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public static MapScroller Instance; 

    public float speed = 2f;
    private float mapHeight; 

    void Awake()
    {
        // BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ ENEMY SPAWNER GỌI SANG
        Instance = this; 
    }

    void Start()
    {
        mapHeight = GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y <= -mapHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + mapHeight, transform.position.z);
        }
    }
}
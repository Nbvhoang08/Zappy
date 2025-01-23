
using UnityEngine;

public class Bullet : MonoBehaviour
{
     [Header("Bullet Settings")]
    public float lifeTime = 5f; // Thời gian sống của bullet (5 giây mặc định)
    public LayerMask wallLayer; // Layer của Wall

    private void Start()
    {
        // Tự động hủy object sau thời gian chỉ định
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu object va chạm thuộc layer Wall
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Phá hủy bullet
        }
    }

}

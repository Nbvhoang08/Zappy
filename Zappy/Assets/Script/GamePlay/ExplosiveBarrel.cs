using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Prefab của hiệu ứng nổ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu object va chạm là bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Spawn hiệu ứng nổ
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            
            // Phá hủy object thùng dầu
            Destroy(gameObject);
        }
    }
}

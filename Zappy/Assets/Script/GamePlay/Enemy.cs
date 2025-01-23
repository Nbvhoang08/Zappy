using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float rayLength = 10f; // Độ dài raycast
    public LayerMask detectLayer; // Lớp của Player
    public Sprite dieSprite; // Sprite thay đổi khi Enemy chết
    public GameObject bulletPrefab; // Prefab của đạn
    public Transform firePoint; // Vị trí bắn đạn
    public float fireDelay = 2f; // Thời gian chờ trước khi bắn
    public LineRenderer lineRenderer; // Dùng để trực quan hóa Raycast
    public Direction shootDirection; // Hướng bắn
    private bool isPlayerDetected = false; // Cờ để kiểm soát trạng thái phát hiện Player
    private bool isDead = false; // Trạng thái Enemy có chết hay không

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (isDead) return; // Nếu Enemy đã chết, không làm gì thêm

        PerformRaycast();
    }

    private void PerformRaycast()
    {
        // Xác định hướng raycast
    Vector2 rayDirection = Vector2.zero;
    switch (shootDirection)
    {
        case Direction.Up: rayDirection = Vector2.up; break;
        case Direction.Down: rayDirection = Vector2.down; break;
        case Direction.Left: rayDirection = Vector2.left; break;
        case Direction.Right: rayDirection = Vector2.right; break;
    }

    // Biến lưu điểm kết thúc của raycast
    Vector3 endPoint = firePoint.position + (Vector3)(rayDirection * rayLength);

    // Lấy collider của chính đối tượng
    Collider2D selfCollider = GetComponent<Collider2D>();

    // Thực hiện RaycastAll
    RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, rayDirection, rayLength, detectLayer);

    // Duyệt qua tất cả các collider mà tia raycast chạm tới
    foreach (RaycastHit2D hit in hits)
    {
        // Bỏ qua collider của chính đối tượng
        if (hit.collider == selfCollider) continue;

        // Nếu gặp vật cản hoặc Player, dừng raycast
        endPoint = hit.point;

        if (hit.collider.CompareTag("Player"))
        {
            if (!isPlayerDetected)
            {
                isPlayerDetected = true;
                Invoke(nameof(Shoot), fireDelay); // Gọi hàm Shoot sau thời gian trễ
            }
        }
        else
        {
            isPlayerDetected = false; // Không phát hiện Player nữa
        }

        // Raycast chỉ dừng tại collider đầu tiên mà nó gặp
        break;
    }

    // Vẽ LineRenderer
    lineRenderer.SetPosition(0, firePoint.position);
    lineRenderer.SetPosition(1, endPoint);
    }

    private void Shoot()
    {
        if (isDead) return; // Kiểm tra nếu Player hoặc Enemy đã chết

        // Bắn đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Lấy hướng bắn
        Vector2 direction = GetDirectionVector(shootDirection);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 10f; // Gán tốc độ đạn

        // Tính góc xoay của đạn
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Tính góc từ hướng bắn
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle+90); // Gán góc xoay cho đạn
        ManagerSound.Instance.PlayVFXSound(0);
    }   

    private Vector2 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return Vector2.up;
            case Direction.Down: return Vector2.down;
            case Direction.Left: return Vector2.left;
            case Direction.Right: return Vector2.right;
        }
        return Vector2.zero;
    }
    void OnTriggerEnter2D(Collider2D other)
    {   
        if(other.CompareTag("Explore"))
        {
            Debug.Log("no");
            if(!isDead)
            {
                isDead = true;
                Die();
            }
        }
        
    }
    public void Die()
    {
        isDead = true; // Đặt trạng thái chết
        GetComponent<SpriteRenderer>().sprite = dieSprite; // Thay đổi sprite
        lineRenderer.enabled = false; // Tắt line renderer
          // Đổi tag của object thành "Item"
        gameObject.tag = "Item";
    }
}
 public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

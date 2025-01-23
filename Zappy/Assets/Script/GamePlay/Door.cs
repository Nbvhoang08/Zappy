using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IObserver
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _doorCollider;
    private string _originalLayer; // Lưu layer gốc của Door
    [SerializeField] private int doorID;

    void Awake()
    {
        Subject.RegisterObserver(this);
    }
    void OnDestroy()
    {
        Subject.UnregisterObserver(this);
    }

    public void OnNotify(string eventName , object eventData)
    {
        if (eventData is int id && id == doorID)
        {
            if (eventName == "Open")
            {
                Open();
            }
            else if (eventName == "Close")
            {
                Close();
            }
        }
    }
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _doorCollider = GetComponent<Collider2D>();

        // Lưu layer ban đầu của Door
        _originalLayer = LayerMask.LayerToName(gameObject.layer);
    }

    public void Open()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = false; // Tắt SpriteRenderer
        }

        gameObject.layer = LayerMask.NameToLayer("Default"); // Đổi layer thành Default

        if (_doorCollider != null)
        {
            _doorCollider.enabled = false; // Tắt Collider (nếu có)
        }
    }

    public bool Close()
    {
        // Kiểm tra xem có object nào đang ở vị trí của door hay không
        if (IsObjectInDoorPosition())
        {
            return false;
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = true; // Bật SpriteRenderer
        }

        gameObject.layer = LayerMask.NameToLayer(_originalLayer); // Đặt lại layer ban đầu

        if (_doorCollider != null)
        {
            _doorCollider.enabled = true; // Bật Collider (nếu có)
        }

        return true; // Đóng cửa thành công
    }

    private bool IsObjectInDoorPosition()
    {
        // Lấy BoxCollider2D của Door
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            return false;
        }

        // Lấy thông tin về kích thước và tâm của BoxCollider2D
        Vector2 boxSize = boxCollider.size;
        Vector2 boxCenter = (Vector2)transform.position + boxCollider.offset;

        // Kiểm tra tất cả các Collider2D trong hình dạng Box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.name);
            if (collider.gameObject != gameObject)
            {
                return true; // Có object khác tại vị trí của Door
            }
        }

        return false; // Không có object nào cản
    }
    private void OnDrawGizmosSelected()
    {
        // Lấy BoxCollider2D của Door
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null) return;

        // Lấy thông tin về kích thước và tâm của BoxCollider2D
        Vector2 boxSize = boxCollider.size;
        Vector2 boxCenter = (Vector2)transform.position + boxCollider.offset;

        // Vẽ hình chữ nhật biểu thị vùng kiểm tra
        Gizmos.color = Color.green; // Màu xanh lá cây để dễ nhận biết
        Gizmos.DrawWireCube(boxCenter, boxSize); // Vẽ đường viền hình chữ nhật
        Gizmos.color = new Color(0, 1, 0, 0.2f); // Màu xanh lá nhạt, có độ trong suốt
        Gizmos.DrawCube(boxCenter, boxSize); // Vẽ hình chữ nhật đặc
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMove : Singleton<PlayerMove>
{
    public Tilemap tilemap; // Gắn TileMap mặt đất
    public Tilemap obstacleTilemap; // Gắn TileMap chứa obstacle
    public float moveSpeed = 5f; // Tốc độ di chuyển
    public LayerMask obstacleLayer; // Layer của chướng ngại vật
    public Animator animator; // Animator để quản lý hoạt ảnh
    public Transform heldItemParent; // Vị trí để hiển thị item đang được cầm

    private Vector3Int _currentCell; // Vị trí hiện tại trên TileMap
    private Vector2 _currentDirection = Vector2.zero; // Hướng hiện tại
    private GameObject _heldItem; // Item đang được giữ
    private bool _canMove = true; // Trạng thái có thể di chuyển hay không

    private void Start()
    {
        _currentCell = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_currentCell);
    }

    public void StartMove(Vector2 direction)
    {
        if (!_canMove) return;

        _currentDirection = direction;

        // Tính toán vị trí tiếp theo
        Vector3Int nextCell = _currentCell + new Vector3Int((int)direction.x, (int)direction.y, 0);

        // Kiểm tra ô hợp lệ
        if (IsValidCell(nextCell))
        {
            StartCoroutine(MoveToCell(nextCell));

            // Cập nhật hoạt ảnh
            if (animator != null)
            {
                animator.SetFloat("XInput", direction.x);
                animator.SetFloat("YInput", direction.y);
                animator.SetBool("IsMoving", true);
            }
        }
    }

    public void StopMove()
    {
        // Reset hướng
        _currentDirection = Vector2.zero;
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private IEnumerator MoveToCell(Vector3Int nextCell)
    {
        _canMove = false;
        Vector3 targetPosition = tilemap.GetCellCenterWorld(nextCell);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        _currentCell = nextCell;
        _canMove = true;
    }

    private bool IsValidCell(Vector3Int cellPosition)
    {
        // Kiểm tra nếu ô hợp lệ trong TileMap và không có chướng ngại vật
        if (!tilemap.HasTile(cellPosition)) return false;

        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Collider2D hit = Physics2D.OverlapCircle(cellWorldPosition, 0.1f, obstacleLayer);

        return hit == null; // Không có chướng ngại vật thì hợp lệ
    }

    public void PickOrDropItem()
    {
        if (_heldItem == null)
        {
            // Nếu không cầm item, thực hiện pick item
            TryPickItem();
        }
        else
        {
            // Nếu đang cầm item, thực hiện drop item
            TryDropItem();
        }
    }

    private void TryPickItem()
    {
        // Kiểm tra item tại ô hiện tại
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(_currentCell);
        Collider2D hit = Physics2D.OverlapCircle(cellWorldPosition, 0.1f);

        if (hit != null && hit.CompareTag("Item"))
        {
            // Pick item
            _heldItem = hit.gameObject;
            _heldItem.transform.SetParent(heldItemParent);
            _heldItem.transform.localPosition = Vector3.zero;
            _heldItem.GetComponent<Collider2D>().enabled = false; // Ngừng cản đường
        }
    }

    private void TryDropItem()
    {
        // Xác định ô phía trước mặt dựa trên currentDirection
        Vector3Int dropCell = _currentCell + new Vector3Int((int)_currentDirection.x, (int)_currentDirection.y, 0);

        if (IsValidDropCell(dropCell))
        {
            // Drop item
            Vector3 dropPosition = tilemap.GetCellCenterWorld(dropCell);
            _heldItem.transform.SetParent(null);
            _heldItem.transform.position = dropPosition;
            _heldItem.GetComponent<Collider2D>().enabled = true; // Kích hoạt cản đường
            _heldItem = null;
        }
    }

    private bool IsValidDropCell(Vector3Int cellPosition)
    {
        // Kiểm tra ô hợp lệ để drop item
        if (!tilemap.HasTile(cellPosition)) return false;

        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Collider2D hit = Physics2D.OverlapCircle(cellWorldPosition, 0.1f, obstacleLayer);

        return hit == null; // Có thể drop nếu không có vật cản
    }

   
}

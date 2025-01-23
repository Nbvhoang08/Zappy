using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class PlayerMove : Singleton<PlayerMove>
{
    public Tilemap tilemap; // Gắn TileMap mặt đất
    public float moveSpeed = 5f; // Tốc độ di chuyển
    public LayerMask obstacleLayer; // Layer của chướng ngại vật
    public Animator animator; // Animator để quản lý hoạt ảnh
    public Transform heldItemParent; // Vị trí để hiển thị item đang được cầm

    private bool isDeath;

    private Vector3Int _currentCell; // Vị trí hiện tại trên TileMap
    [SerializeField] private Vector2 _currentDirection = Vector2.zero; // Hướng hiện tại
    private GameObject _heldItem; // Item đang được giữ
    private bool _canMove = true; // Trạng thái có thể di chuyển hay không
    private Coroutine _movementCoroutine; // Coroutine để di chuyển liên tục
    private Coroutine _inputHoldCoroutine; // Coroutine để chờ giữ nút

    
    // Thêm các biến mới để quản lý teleport
    private bool isTeleporting = false; // Kiểm tra trạng thái teleport
    private bool hasLeftTeleportZone = true; // Kiểm tra nếu Player đã rời khỏi cổng teleport
    private float teleportCooldown = 1f; // Thời gian chờ giữa các lần teleport
    private float lastTeleportTime = 0f; // Lưu thời gian teleport cuối cùng
    private void Start()
    {
        _currentCell = tilemap.WorldToCell(transform.position);
        transform.position = tilemap.GetCellCenterWorld(_currentCell);
        isDeath = false;
    }
    void Update()
    {
        animator.SetBool("IsBring", _heldItem!= null);
        animator.SetBool("IsDead", isDeath);

    }
    
    public void StartMove(Vector2 direction)
    {
        if(isDeath) return;
        if (!_canMove) return;

        // Lưu hướng di chuyển hiện tại
        _currentDirection = direction;

        // Quản lý hoạt ảnh dựa trên hướng di chuyển
        if (animator != null)
        {
            animator.SetFloat("XInput", direction.x);
            animator.SetFloat("YInput", direction.y);
            animator.SetBool("IsBring", _heldItem != null);
        }

        // Nếu chưa có Coroutine giữ nút, bắt đầu Coroutine
        if (_inputHoldCoroutine == null)
        {
            _inputHoldCoroutine = StartCoroutine(HoldToMove());
        }
    }

    private IEnumerator HoldToMove()
    {
        // Chờ 0.2 giây trước khi bắt đầu di chuyển liên tục
        yield return new WaitForSeconds(0.2f);

        // Sau thời gian chờ, nếu vẫn có hướng, bắt đầu di chuyển liên tục
        if (_currentDirection != Vector2.zero)
        {
            _movementCoroutine = StartCoroutine(ContinuousMove());
        }
    }

    private IEnumerator ContinuousMove()
    {
        while (true)
        {
            if (_canMove && _currentDirection != Vector2.zero)
            {
                // Tính toán vị trí tiếp theo
                Vector3Int nextCell = _currentCell + new Vector3Int((int)_currentDirection.x, (int)_currentDirection.y, 0);

                // Nếu cell hợp lệ, di chuyển
                if (IsValidCell(nextCell))
                {
                    yield return MoveToCell(nextCell);
                }
                else
                {
                    // Nếu gặp cell không hợp lệ, dừng một frame
                    yield return null;
                }
            }
            else
            {
                // Nếu không có hướng hoặc không thể di chuyển, dừng một frame
                yield return null;
            }
        }
    }

    private IEnumerator MoveToCell(Vector3Int nextCell)
    {
        _canMove = false;

        // Di chuyển nhân vật đến tâm của ô tiếp theo
        Vector3 targetPosition = tilemap.GetCellCenterWorld(nextCell);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // Đảm bảo vị trí chính xác
        _currentCell = nextCell; // Cập nhật vị trí hiện tại

        _canMove = true; // Cho phép di chuyển tiếp
    }

    private bool IsValidCell(Vector3Int cellPosition)
    {
        // Kiểm tra nếu cell nằm trong TileMap
        if (!tilemap.HasTile(cellPosition))
        {
            return false; // Không có tile tại cell này
        }

        // Kiểm tra nếu có chướng ngại vật
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Collider2D hit = Physics2D.OverlapCircle(cellWorldPosition, 0.1f, obstacleLayer);
        return hit == null; // Trả về true nếu không có chướng ngại vật
    }

    public void StopMove()
    {
        // Dừng các coroutine liên quan
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        if (_inputHoldCoroutine != null)
        {
            StopCoroutine(_inputHoldCoroutine);
            _inputHoldCoroutine = null;
        }

        // Tìm cell gần nhất theo vị trí hiện tại
        Vector3Int closestCell = tilemap.WorldToCell(transform.position);

        // Bắt đầu coroutine slide tới cell gần nhất
        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
        }
        _slideCoroutine = StartCoroutine(SlideToClosestCell(closestCell));
    }

    private Coroutine _slideCoroutine; // Coroutine để slide về cell gần nhất

    private IEnumerator SlideToClosestCell(Vector3Int targetCell)
    {
        _canMove = false; // Không cho phép di chuyển trong khi đang slide

        Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);
            yield return null;
        }

        // Cập nhật vị trí chính xác và trạng thái di chuyển
        transform.position = targetPosition;
        _currentCell = targetCell;
        _canMove = true; // Cho phép di chuyển trở lại
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
        // Xác định ô phía trước mặt dựa trên currentDirection
        Vector3Int frontCell = _currentCell + new Vector3Int((int)_currentDirection.x, (int)_currentDirection.y, 0);

        // Kiểm tra item tại ô phía trước
        Vector3 cellWorldPosition = tilemap.GetCellCenterWorld(frontCell);
        Collider2D hit = Physics2D.OverlapCircle(cellWorldPosition, 0.1f);
        Debug.Log(hit.name);
        if (hit != null && hit.CompareTag("Item"))
        {
            // Pick item
            ManagerSound.Instance.PlayVFXSound(5);
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

    // Hàm xử lý teleport
    
    [Header("VFX Settings")]
    public GameObject teleportEffectPrefab; // Hiệu ứng teleport tại vị trí bắt đầu
    public GameObject appearEffectPrefab; // Hiệu ứng xuất hiện tại vị trí đích
    public GameObject hitEffectPrefab; // Hiệu ứng khi bị trúng đạn
    private void TeleportPlayer(GameObject teleport)
    {
        // Lấy vị trí điểm đến từ đối tượng teleport (có thể gán điểm này trong Inspector)
        Transform destination = teleport.GetComponent<Teleport>().destination;

        if (destination != null)
        {
            // Spawn hiệu ứng teleport tại vị trí hiện tại của Player
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
            }

            // Cập nhật vị trí của Player đến vị trí của destination
            transform.position = destination.position;
            _currentCell = tilemap.WorldToCell(destination.position); // Cập nhật lại ô trong Tilemap

            // Spawn hiệu ứng xuất hiện tại điểm đích
            if (appearEffectPrefab != null)
            {
                Instantiate(appearEffectPrefab, destination.position, Quaternion.identity);
            }

            // Đánh dấu Player đã teleport, và cần phải di chuyển ra khỏi cổng mới có thể teleport lại
            hasLeftTeleportZone = false;

            // Bắt đầu coroutine để reset trạng thái teleport sau một thời gian chờ
            StartCoroutine(AllowTeleportAfterCooldown());
        }
    }

    // Coroutine để reset trạng thái teleport sau thời gian chờ
    private IEnumerator AllowTeleportAfterCooldown()
    {
        // Đợi một thời gian để Player không thể teleport ngay lập tức
        yield return new WaitForSeconds(teleportCooldown);

        // Cho phép teleport lại sau khi chờ xong
        hasLeftTeleportZone = true;
        isTeleporting = false; // Reset trạng thái teleport
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Port") && !isTeleporting && hasLeftTeleportZone && Time.time - lastTeleportTime >= teleportCooldown)
        {
            isTeleporting = true; // Đánh dấu trạng thái đang teleport
            lastTeleportTime = Time.time; // Cập nhật thời gian teleport

            StopAllCoroutines(); // Dừng mọi coroutine di chuyển đang diễn ra

            // Teleport Player đến điểm đến mới
            TeleportPlayer(other.gameObject);
        }
        else if(other.CompareTag("Bullet"))
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            isDeath = true;
            StartCoroutine(Lose());
            ManagerSound.Instance.PlayVFXSound(2);
        }   
    }
    IEnumerator Lose()
    {
        yield return new WaitForSeconds(1);
         // Load lại scene hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Nếu Player rời khỏi vùng teleport, reset trạng thái để cho phép teleport lại
        if (collision.gameObject.CompareTag("Port"))
        {
            hasLeftTeleportZone = true;
        }
    }
}

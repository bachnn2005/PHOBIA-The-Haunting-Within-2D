using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    public Transform topPoint; // Điểm cao nhất
    public Transform bottomPoint; // Điểm thấp nhất
    public float speed; // Tốc độ di chuyển
    public ParticleSystem sparkEffect; // Hệ thống hạt tia lửa
    private bool isMovingUp = true; // Điều khiển hướng di chuyển của thang máy
    private bool isMoving = false; // Kiểm soát trạng thái thang máy đang di chuyển
    private bool playerOnElevator = false; // Kiểm tra người chơi có trên thang máy không
    private bool playerNearActivator = false; // Kiểm tra người chơi có gần điểm kích hoạt không
    [SerializeField] private CameraManager cameraManager; // Tham chiếu đến CameraManager
    private int originalLayer; // Lưu layer ban đầu của thang máy

    void Start()
    {
        // Lưu layer ban đầu
        originalLayer = gameObject.layer;

        // Tắt Particle System ngay khi vào game
        if (sparkEffect.isPlaying)
        {
            sparkEffect.Stop(); // Đảm bảo tắt hệ thống hạt khi game bắt đầu
        }
    }

    void Update()
    {
        // Nhấn E hoặc V chỉ khi đứng gần điểm kích hoạt và thang máy không di chuyển
        if (playerNearActivator && !isMoving && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.V)) && playerOnElevator)
        {
            isMoving = true; // Bắt đầu di chuyển thang máy
            sparkEffect.Play(); // Bắt đầu hệ thống hạt khi thang máy di chuyển

            // Thay đổi layer của thang máy và các đối tượng con (nhưng không thay đổi layer của người chơi)
            ChangeLayerRecursively(gameObject, LayerMask.NameToLayer("Water"));
        }

        if (isMoving)
        {
            MoveElevator();
        }
    }

    void MoveElevator()
    {
        // Lấy vị trí hiện tại của thang máy
        Vector3 currentPosition = transform.position;

        // Di chuyển lên
        if (isMovingUp)
        {
            // Chỉ di chuyển theo trục Y
            transform.position = new Vector3(currentPosition.x,
                Mathf.MoveTowards(currentPosition.y, topPoint.position.y, speed * Time.deltaTime),
                currentPosition.z);

            // Kiểm tra nếu đã đến điểm cao nhất
            if (Mathf.Approximately(transform.position.y, topPoint.position.y))
            {
                isMovingUp = false; // Đổi hướng di chuyển xuống
                isMoving = false;   // Thang máy ngừng lại
                sparkEffect.Stop(); // Dừng hệ thống hạt khi thang máy dừng lại

                // Khôi phục layer
                ChangeLayerRecursively(gameObject, originalLayer);

                // Gọi hàm khi thang máy chạm điểm cao nhất
                OnElevatorReachTop();
            }
        }
        // Di chuyển xuống
        else
        {
            // Chỉ di chuyển theo trục Y
            transform.position = new Vector3(currentPosition.x,
                Mathf.MoveTowards(currentPosition.y, bottomPoint.position.y, speed * Time.deltaTime),
                currentPosition.z);

            // Kiểm tra nếu đã đến điểm thấp nhất
            if (Mathf.Approximately(transform.position.y, bottomPoint.position.y))
            {
                isMovingUp = true;  // Đổi hướng di chuyển lên
                isMoving = false;   // Thang máy ngừng lại
                sparkEffect.Stop(); // Dừng hệ thống hạt khi thang máy dừng lại

                // Khôi phục layer
                ChangeLayerRecursively(gameObject, originalLayer);

                // Gọi hàm khi thang máy chạm đáy
                OnElevatorReachBottom();
            }
        }
    }

    // Thay đổi layer cho đối tượng và tất cả các đối tượng con, nhưng bỏ qua người chơi
    private void ChangeLayerRecursively(GameObject obj, int newLayer)
    {
        // Không thay đổi layer của người chơi
        if (obj.CompareTag("Player")) return;

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, newLayer);
        }
    }

    // Khi người chơi vào vùng kích hoạt
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player")) // Thay bằng tag của người chơi
        {
            playerNearActivator = true; // Đánh dấu người chơi gần điểm kích hoạt
        }
    }

    // Khi người chơi rời khỏi vùng kích hoạt
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerNearActivator = false; // Đánh dấu người chơi không còn gần điểm kích hoạt
        }
    }

    // Khi người chơi đứng lên thang máy
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player")) // Thay bằng tag của người chơi
        {
            coll.transform.parent = transform; // Gắn người chơi vào thang máy
            playerOnElevator = true; // Đánh dấu người chơi đang trên thang máy
        }
    }

    // Khi người chơi rời khỏi thang máy
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            coll.transform.parent = null; // Hủy gắn người chơi với thang máy
            playerOnElevator = false; // Đánh dấu người chơi không còn trên thang máy
        }
    }

    // Hàm gọi khi thang máy chạm đáy
    private void OnElevatorReachBottom()
    {
        // Gọi shake camera
        cameraManager.Shake(1.5f, 1.5f, 0.2f); // Điều chỉnh giá trị theo nhu cầu
    }

    // Hàm gọi khi thang máy chạm điểm cao nhất
    private void OnElevatorReachTop()
    {
        // Gọi shake camera
        cameraManager.Shake(1.3f, 1.3f, 0.2f); // Điều chỉnh giá trị theo nhu cầu
    }
}

using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform; // Tham chiếu đến transform của camera
    public float parallaxEffect; // Tốc độ parallax

    private Vector3 _lastCameraPosition; // Vị trí camera trước đó

    void Start()
    {
        _lastCameraPosition = cameraTransform.position; // Lưu vị trí camera ban đầu
    }

    void Update()
    {
        // Tính khoảng cách di chuyển của camera
        float parallax = (cameraTransform.position.x - _lastCameraPosition.x) * parallaxEffect;

        // Cập nhật vị trí của lớp nền
        transform.position += new Vector3(parallax, 0, 0);

        // Lưu vị trí camera hiện tại để tính toán ở khung hình tiếp theo
        _lastCameraPosition = cameraTransform.position;
    }
}

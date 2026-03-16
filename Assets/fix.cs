using UnityEngine;

public class LockYPosition : MonoBehaviour
{
    // Giá trị Y cố định
    public float fixedYPosition = 0f;

    // Offset cho chiều X
    public float xOffset = 0f;

    void Update()
    {
        // Khóa chiều Y của GameObject
        // Di chuyển GameObject theo chiều X với offset
        transform.position = new Vector3(transform.position.x + xOffset, fixedYPosition, transform.position.z);
    }
}

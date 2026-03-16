using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Transform cam; // Camera được kéo vào từ Editor
    Vector3 camStartPos;
    Vector2 distance; // Khoảng cách giữa vị trí bắt đầu và vị trí hiện tại của camera

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;
    Vector3[] backStartPos; // Lưu vị trí ban đầu của các background

    float farthestBack;

    [Range(0.01f, 1f)]
    public float parallaxSpeed;

    // Start is called trước tiên khi script bắt đầu
    void Start()
    {
        // Nếu camera không được gán từ Editor, lấy camera chính
        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];
        backStartPos = new Vector3[backCount]; // Lưu vị trí ban đầu của các background

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
            backStartPos[i] = backgrounds[i].transform.position; // Lưu vị trí ban đầu
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++) // Tìm lớp nền xa nhất
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++) // Thiết lập tốc độ nền
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        // Tính toán khoảng cách di chuyển của camera theo cả trục X và Y
        distance = new Vector2(cam.position.x - camStartPos.x, cam.position.y - camStartPos.y);

        // Di chuyển GameObject cha theo cả trục X và Y
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        // Di chuyển nền theo cả hai trục X và Y dựa trên vị trí ban đầu và tốc độ parallax
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            Vector2 offset = new Vector2(distance.x, distance.y) * speed;

            // Cập nhật offset cho texture của background
            mat[i].SetTextureOffset("_MainTex", offset);

            // Căn chỉnh background để nó nằm ở giữa camera
            Vector3 backgroundPosition = backStartPos[i];
            backgroundPosition.x = cam.position.x;
            backgroundPosition.y = cam.position.y;

            // Đặt lại vị trí của background
            backgrounds[i].transform.position = backgroundPosition;
        }
    }

}

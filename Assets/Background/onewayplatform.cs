using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onewayplatform : MonoBehaviour
{
    public string OneWayLayerName = "OneWay";
    public string PlayerLayerName = "Player";
    public float dropCooldown = 0.2f;  // Để tránh người chơi rơi liên tục
    private float dropTimer;
    private bool isOnPlatform;  // Biến kiểm tra nếu nhân vật đang đứng trên nền tảng

    private void Update()
    {
        dropTimer -= Time.deltaTime;

        // Kiểm tra nếu người chơi nhấn phím xuống và đang đứng trên nền tảng
        if (Input.GetAxis("Vertical") < 0 && dropTimer <= 0 && isOnPlatform)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(OneWayLayerName), true);
            dropTimer = dropCooldown;  // Đặt thời gian cooldown để tránh va chạm bật lại ngay lập tức
        }
        else if (dropTimer <= 0)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(OneWayLayerName), false);
        }
    }

    // Kiểm tra va chạm khi người chơi đứng trên nền tảng
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Kiểm tra xem nhân vật có đang chạm vào nền tảng một chiều hay không
        if (collision.gameObject.layer == LayerMask.NameToLayer(OneWayLayerName))
        {
            isOnPlatform = true;  // Nhân vật đang đứng trên nền tảng
        }
    }

    // Khi không còn va chạm với nền tảng, thiết lập lại biến
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(OneWayLayerName))
        {
            isOnPlatform = false;  // Nhân vật đã rời khỏi nền tảng
        }
    }
}

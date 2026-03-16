using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSnake : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private DmgFlash _dmgFlash;
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private PolygonCollider2D triggerBox;
    [SerializeField] private Animator animator;
    private bool getHit;
    private bool isMoving = true;
    private PlayerMoveBehave player;
    private int currentWaypointIndex = 1;
    private Slash slashDmg;
    // Start is called before the first frame update
    void Start()
    {
        slashDmg = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>();
        player = FindAnyObjectByType<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            return;
        }
        if(hp <= 0f)
        {
            StartCoroutine(Dying());
        }
        MoveToWaypoint();
    }
    private void MoveToWaypoint()
    {
        // Tính khoảng cách giữa đối tượng và điểm đích
        float distance = Vector2.Distance(transform.position, waypoints[currentWaypointIndex].transform.position);

        // Nếu đối tượng chưa đến đích, tiếp tục di chuyển
        if (distance > 0.1f)
        {
            Vector2 direction = (waypoints[currentWaypointIndex].transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);
        }
        else
        {
            // Nếu đã đến đích, bắt đầu xoay đối tượng mượt mà 90 độ
            StartCoroutine(RotateObjectSmooth(-90f));

            // Chuyển sang điểm tiếp theo
            currentWaypointIndex++;

            // Nếu đã đi hết các điểm, quay lại điểm đầu tiên
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
    }
    private IEnumerator GetHit()
    {
        //isPursuing = true;
        getHit = true;
        isMoving = false;
        //rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);
        isMoving = true;
        yield return new WaitForSeconds(0.05f);
        getHit = false;
    }
    private IEnumerator Dying()
    {
        //noitice.enabled = false;
        animator.SetTrigger("isDead");
        //rb.velocity = Vector2.zero;
        isMoving = false;
        triggerBox.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") && !getHit)
        {
            if (player != null)
            {
                StartCoroutine(GetHit());
                hp -= slashDmg.getSlashDmg();
                _dmgFlash.CallDmgFlash();
            }
        }
    }
    private IEnumerator RotateObjectSmooth(float degrees)
    {
        float targetRotation = transform.rotation.eulerAngles.z + degrees;
        float currentRotation = transform.rotation.eulerAngles.z;
        float time = 0f;
        float duration = 0.4f; // thời gian để hoàn thành việc xoay

        while (time < duration)
        {
            time += Time.deltaTime;
            float zRotation = Mathf.Lerp(currentRotation, targetRotation, time / duration);
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        // Đảm bảo xoay đúng vị trí cuối cùng
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    private void OnDrawGizmos()
    {
        // Đặt màu cho Gizmos (màu xanh dương)
        Gizmos.color = Color.white;

        // Vẽ hình cầu tại vị trí của mỗi waypoint
        foreach (GameObject waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawSphere(waypoint.transform.position, 0.1f); // Vẽ hình cầu với bán kính 0.3
            }
        }
    }

}

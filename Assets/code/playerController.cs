using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("走路动画模拟")]
    public float bobSpeed = 10f;        // 上下摆动速度
    public float bobAmount = 0.1f;      // 摆动幅度
    public float tiltAngle = 5f;        // 倾斜角度

    private Vector3 originalPosition;
    private Rigidbody rb;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // 冻结旋转，防止Cube翻倒
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        originalPosition = transform.position;
    }

    void Update()
    {
        // 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 判断是否在移动
        isMoving = horizontal != 0 || vertical != 0;

        // 移动
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // 模拟走路动画
        if (isMoving)
        {
            WalkingAnimation();
        }
    }

    void WalkingAnimation()
    {
        // 上下摆动（模拟脚步）
        float newY = originalPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 左右摇晃（模拟身体摆动）
        float tilt = Mathf.Sin(Time.time * bobSpeed) * tiltAngle;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, tilt);
    }
}

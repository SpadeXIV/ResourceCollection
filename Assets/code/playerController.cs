using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("场景边界")]
    public float boundaryX = 8f;
    public float boundaryZ = 8f;

    private Animator animator;

    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        ClampPosition();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
        bool currentlyMoving = movement.magnitude > 0.1f;

        // 移动
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // 转向
        if (currentlyMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }

        // 触发器切换动画状态
        if (currentlyMoving && !isMoving)
        {
            // 刚开始移动 → 触发Run
            animator.SetTrigger("Running");
            isMoving = true;
        }
        else if (!currentlyMoving && isMoving)
        {
            // 刚停下来 → 触发Idle
            animator.SetTrigger("StopRun");
            isMoving = false;
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
        pos.z = Mathf.Clamp(pos.z, -boundaryZ, boundaryZ);
        transform.position = pos;
    }
}

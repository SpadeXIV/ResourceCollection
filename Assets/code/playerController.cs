using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 0.2f;

    [Header("场景边界")]
    public GameObject groundPlane;
    public float boundaryOffset = 0.5f;

    private Animator animator;
    private float boundaryX;
    private float boundaryZ;
    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        CalculateBoundary();
    }

    void CalculateBoundary()
    {
        if (groundPlane != null)
        {
            Vector3 groundScale = groundPlane.transform.localScale;
            boundaryX = 5f * groundScale.x - boundaryOffset;
            boundaryZ = 5f * groundScale.z - boundaryOffset;
        }
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
        }

        // 用Trigger切换动画状态
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
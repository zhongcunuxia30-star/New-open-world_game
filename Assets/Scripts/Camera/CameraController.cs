using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Cinemachine 3.x 対応のサードパーソンカメラコントローラー
/// FreeLook スタイルの軌道カメラ + マウスズーム
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("ターゲット")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("回転設定")]
    [SerializeField] private float mouseSensitivityX = 2.5f;
    [SerializeField] private float mouseSensitivityY = 1.5f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;
    [SerializeField] private float rotationSmoothTime = 0.05f;

    [Header("ズーム設定")]
    [SerializeField] private float defaultDistance = 4f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomSmoothTime = 0.15f;

    [Header("衝突回避")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionLayers = ~0;

    [Header("カーソル設定")]
    [SerializeField] private bool lockCursorOnStart = true;

    // 内部状態
    private float yaw;
    private float pitch;
    private float currentDistance;
    private float targetDistance;
    private float distanceSmoothVelocity;
    private Vector2 lookInput;
    private float zoomInput;
    private bool isCursorLocked;

    private void Start()
    {
        currentDistance = defaultDistance;
        targetDistance = defaultDistance;

        if (followTarget != null)
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;
        if (!isCursorLocked) return;

        UpdateRotation();
        UpdateZoom();
        UpdatePosition();
    }

    #region カメラ更新

    private void UpdateRotation()
    {
        yaw += lookInput.x * mouseSensitivityX;
        pitch -= lookInput.y * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
    }

    private void UpdateZoom()
    {
        targetDistance -= zoomInput * zoomSpeed;
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceSmoothVelocity, zoomSmoothTime);
    }

    private void UpdatePosition()
    {
        Vector3 targetPosition = followTarget.position + targetOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPosition = targetPosition - rotation * Vector3.forward * currentDistance;

        // 衝突回避レイキャスト
        float adjustedDistance = currentDistance;
        if (Physics.SphereCast(targetPosition, collisionRadius, (desiredPosition - targetPosition).normalized,
            out RaycastHit hit, currentDistance, collisionLayers))
        {
            adjustedDistance = hit.distance - collisionRadius;
            adjustedDistance = Mathf.Max(adjustedDistance, minDistance * 0.5f);
        }

        Vector3 finalPosition = targetPosition - rotation * Vector3.forward * adjustedDistance;

        transform.position = finalPosition;
        transform.rotation = rotation;
    }

    #endregion

    #region カーソル管理

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }

    public void ToggleCursorLock()
    {
        if (isCursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    public bool IsCursorLocked => isCursorLocked;

    #endregion

    #region Input System コールバック

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>() / 120f;
    }

    #endregion

    #region 公開メソッド

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public float Yaw => yaw;
    public float Pitch => pitch;

    #endregion
}

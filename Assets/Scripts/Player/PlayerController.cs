using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 原神PC版風プレイヤーコントローラー
/// CharacterController ベースの移動、ジャンプ、ダッシュ、歩き、攻撃、スキル、回避、インタラクト
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float speedSmoothTime = 0.1f;

    [Header("ジャンプ設定")]
    [SerializeField] private float jumpHeight = 1.4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("ダッシュ設定")]
    [SerializeField] private float sprintStaminaCost = 15f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 20f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("回避設定")]
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.4f;
    [SerializeField] private float dodgeStaminaCost = 20f;
    [SerializeField] private float dodgeCooldown = 0.8f;

    [Header("戦闘設定")]
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private float skillCooldown = 6f;
    [SerializeField] private float burstCooldown = 12f;

    [Header("参照")]
    [SerializeField] private Transform cameraTransform;

    // コンポーネント
    private CharacterController characterController;
    private Animator animator;

    // 入力値
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool sprintHeld;
    private bool walkToggle;

    // 移動ステート
    private Vector3 velocity;
    private float currentSpeed;
    private float speedSmoothVelocity;
    private float rotationSmoothVelocity;
    private bool isGrounded;

    // スタミナ
    private float currentStamina;
    private float staminaRegenTimer;

    // 回避
    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private Vector3 dodgeDirection;

    // 戦闘クールダウン
    private float attackCooldownTimer;
    private float skillCooldownTimer;
    private float burstCooldownTimer;

    // 状態
    public bool IsSprinting { get; private set; }
    public bool IsWalking { get; private set; }
    public bool IsGrounded => isGrounded;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentStamina = maxStamina;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        UpdateGroundCheck();
        UpdateTimers();
        UpdateStamina();

        if (isDodging)
        {
            UpdateDodge();
        }
        else
        {
            UpdateMovement();
            ApplyGravity();
        }

        characterController.Move(velocity * Time.deltaTime);
        UpdateAnimator();
    }

    #region 物理・移動

    private void UpdateGroundCheck()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void UpdateMovement()
    {
        Vector2 input = moveInput;
        Vector3 moveDirection = Vector3.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;

            if (cameraTransform != null)
                targetAngle += cameraTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationSmoothVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // 速度決定
        float targetSpeed = 0f;
        if (input.sqrMagnitude > 0.01f)
        {
            if (IsSprinting && currentStamina > 0)
                targetSpeed = sprintSpeed;
            else if (IsWalking)
                targetSpeed = walkSpeed;
            else
                targetSpeed = runSpeed;
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void UpdateDodge()
    {
        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
        {
            isDodging = false;
            velocity = Vector3.zero;
            return;
        }

        float dodgeSpeed = dodgeDistance / dodgeDuration;
        velocity = dodgeDirection * dodgeSpeed;
        velocity.y += gravity * Time.deltaTime;
    }

    #endregion

    #region スタミナ

    private void UpdateStamina()
    {
        // ダッシュ中のスタミナ消費
        if (IsSprinting && moveInput.sqrMagnitude > 0.01f)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            staminaRegenTimer = staminaRegenDelay;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                IsSprinting = false;
            }
        }
        else
        {
            // スタミナ回復
            staminaRegenTimer -= Time.deltaTime;
            if (staminaRegenTimer <= 0f)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
            }
        }
    }

    #endregion

    #region タイマー

    private void UpdateTimers()
    {
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (skillCooldownTimer > 0) skillCooldownTimer -= Time.deltaTime;
        if (burstCooldownTimer > 0) burstCooldownTimer -= Time.deltaTime;
        if (dodgeCooldownTimer > 0) dodgeCooldownTimer -= Time.deltaTime;
    }

    #endregion

    #region アニメーター

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsSprinting", IsSprinting);
        animator.SetFloat("VerticalVelocity", velocity.y);
    }

    #endregion

    #region Input System コールバック

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSprinting = true;
        }
        else if (context.canceled)
        {
            IsSprinting = false;
        }
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsWalking = !IsWalking;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && attackCooldownTimer <= 0f && !isDodging)
        {
            PerformAttack();
        }
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed && skillCooldownTimer <= 0f && !isDodging)
        {
            PerformSkill();
        }
    }

    public void OnElementalBurst(InputAction.CallbackContext context)
    {
        if (context.performed && burstCooldownTimer <= 0f && !isDodging)
        {
            PerformElementalBurst();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && dodgeCooldownTimer <= 0f && !isDodging && currentStamina >= dodgeStaminaCost)
        {
            PerformDodge();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformInteract();
        }
    }

    #endregion

    #region アクション実行

    private void PerformAttack()
    {
        attackCooldownTimer = attackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        Debug.Log("[PlayerController] 通常攻撃実行");
    }

    private void PerformSkill()
    {
        skillCooldownTimer = skillCooldown;

        if (animator != null)
            animator.SetTrigger("Skill");

        Debug.Log("[PlayerController] スキル実行");
    }

    private void PerformElementalBurst()
    {
        burstCooldownTimer = burstCooldown;

        if (animator != null)
            animator.SetTrigger("Burst");

        Debug.Log("[PlayerController] 元素爆発実行");
    }

    private void PerformDodge()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
        currentStamina -= dodgeStaminaCost;
        staminaRegenTimer = staminaRegenDelay;

        // 回避方向：移動入力があればその方向、なければ後方
        if (moveInput.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            if (cameraTransform != null)
                targetAngle += cameraTransform.eulerAngles.y;
            dodgeDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            dodgeDirection = -transform.forward;
        }

        if (animator != null)
            animator.SetTrigger("Dodge");

        Debug.Log("[PlayerController] 回避実行");
    }

    private void PerformInteract()
    {
        // インタラクト対象を検索
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                Debug.Log($"[PlayerController] インタラクト: {col.gameObject.name}");
                return;
            }
        }

        Debug.Log("[PlayerController] インタラクト対象なし");
    }

    #endregion
}

/// <summary>
/// インタラクト可能なオブジェクト用インターフェース
/// </summary>
public interface IInteractable
{
    void Interact(GameObject interactor);
    string GetInteractPrompt();
}

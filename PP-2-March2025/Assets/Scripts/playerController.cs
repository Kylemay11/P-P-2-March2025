using UnityEngine;
using System;
using TMPro;
using System.Collections;

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Crouching,
    Sliding
}

public class playerController : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private TMP_Text speedMeter;
    [SerializeField] private Transform playerCamera;

    private PlayerState currentState;

    [Header("Player Stats")]
    [SerializeField] private int maxHP;
    [SerializeField] public int currentHP;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaDrainRate;
    [SerializeField] private float staminaRegainRate;
    [SerializeField] private float staminaRegainDelay;
    [SerializeField] private float slideStaminaDrainMultiplier;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration;
    [SerializeField] private float slideSpeedBoost;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int maxJumps;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private Vector3 crouchColliderSize;
    [SerializeField] private Vector3 normalColliderSize;
    [SerializeField] private float crouchCameraHeight;
    [SerializeField] private float normalCameraHeight;

    private Vector3 moveDir;
    private Vector3 velocity;
    private int jumpCount;
    private float currentSpeed;
    private float staminaRegenTimer;
    private bool staminaFullyDrained;
    private bool canSprint;
    private bool isGrounded;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;
        currentState = PlayerState.Idle;

        if (playerCamera != null)
            UpdateCameraHeight(normalCameraHeight);

        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        updatePlayerUI();
    }

    void Update()
    {
        HandleMovement();
        HandleSprint();
        HandleCrouch();
        HandleStamina();
        UpdateSpeedUI();
    }

    private void HandleMovement()
    {
        if (controller.isGrounded)
        {
            isGrounded = true;
            velocity.y = 0;
            jumpCount = 0;

            if (currentState != PlayerState.Sprinting && currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
            {
                currentState = PlayerState.Walking;
                currentSpeed = walkSpeed;
            }

            if (currentState == PlayerState.Crouching)
                currentSpeed = crouchSpeed;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        HandleJump();

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleSprint()
    {
        if (Input.GetButtonDown("Sprint") && isGrounded && canSprint)
        {
            staminaRegenTimer = 0f;
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMultiplier;
        }
        else if (Input.GetButtonUp("Sprint") && currentState == PlayerState.Sprinting)
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (currentState == PlayerState.Sprinting && isGrounded)
            {
                EnterSlide();
                return;
            }
            if (currentState != PlayerState.Crouching)
                EnterCrouch();
        }

        if (Input.GetButton("Crouch") && currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
        {
            EnterCrouch();
        }
        else if (!Input.GetButton("Crouch") && currentState == PlayerState.Crouching)
        {
            ExitCrouch();
        }
    }

    private void EnterCrouch()
    {
        currentState = PlayerState.Crouching;
        currentSpeed = crouchSpeed;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 2f, 0);
        UpdateCameraHeight(crouchCameraHeight);
    }

    private void ExitCrouch()
    {
        currentState = PlayerState.Walking;
        currentSpeed = walkSpeed;
        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        UpdateCameraHeight(normalCameraHeight);
    }

    private void EnterSlide()
    {
        currentState = PlayerState.Sliding;
        currentSpeed = walkSpeed * sprintMultiplier * slideSpeedBoost;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 2f, 0);
        UpdateCameraHeight(crouchCameraHeight);
        StartCoroutine(SlideTimer());
    }

    private IEnumerator SlideTimer()
    {
        yield return new WaitForSeconds(slideDuration);
        ExitSlide();
    }

    private void ExitSlide()
    {
        if (Input.GetButton("Crouch"))
        {
            EnterCrouch();
        }
        else if (Input.GetButton("Sprint"))
        {
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMultiplier;
        }
        else
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }

        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 16f, 0);
        UpdateCameraHeight(normalCameraHeight);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            velocity.y = jumpForce;
            jumpCount++;
        }
    }

    private void HandleStamina()
    {
        if (currentState == PlayerState.Sprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina <= 0)
            {
                staminaFullyDrained = true;
                canSprint = false;
                currentState = PlayerState.Walking;
                currentSpeed = walkSpeed;
            }
            staminaRegenTimer = 0f;
        }

        if (currentState == PlayerState.Sliding)
        {
            currentStamina -= staminaDrainRate * slideStaminaDrainMultiplier * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            staminaRegenTimer = 0f;
        }

        if (currentState != PlayerState.Sprinting && currentState != PlayerState.Sliding)
        {
            if (staminaFullyDrained)
            {
                if (staminaRegenTimer < staminaRegainDelay)
                {
                    staminaRegenTimer += Time.deltaTime;
                }
                else
                {
                    RegainStamina();
                }
            }
            else
            {
                RegainStamina();
            }
        }
    }

    private void RegainStamina()
    {
        if (staminaFullyDrained && currentStamina > 0)
            staminaFullyDrained = false;

        currentStamina += staminaRegainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina > 0)
            canSprint = true;
    }

    private void UpdateCameraHeight(float height)
    {
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, height, playerCamera.localPosition.z);
    }

    private void UpdateSpeedUI()
    {
        if (speedMeter != null)
            speedMeter.text = $"Speed: {currentSpeed:F2}";
        gameManager.instance.playerStaminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void takeDamage(int amount)
    {
        currentHP -= amount;
        updatePlayerUI();
        StartCoroutine(DamageFlash());

        if (currentHP <= 0)
            gameManager.instance.youLose();
    }

    private IEnumerator DamageFlash()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)currentHP / maxHP;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        updatePlayerUI();
    }
}

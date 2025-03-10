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
    [SerializeField] CharacterController controller;

    private PlayerState currentState;

    [Header("Player States")]
    [SerializeField] public int HP;

    [Header("Movment Settings")]
    [SerializeField] private TMP_Text speedMeter;
    [Range(1f, 10f)][SerializeField] private float walkSpeed;
    [Range(1f, 5f)][SerializeField] private float sprintMult;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration;
    [SerializeField] private float slideBoost;

    [Header("Jump Settings")]
    [Range(1, 25)][SerializeField] private int jumpSpeed;
    [Range(8f, 45f)][SerializeField] private float gravity;
    [Range(1, 3)][SerializeField] private int maxJumps;
    [SerializeField] private bool isGrounded;

    [Header("Crouch Settings")]
    [SerializeField] private int crouchSpeed;
    [SerializeField] private Vector3 crouchColliderSize;
    [SerializeField] private Vector3 normalColliderSize;
    [SerializeField] private float crouchCameraHeight;
    [SerializeField] private float normalCameraHeight;
    [SerializeField] private Transform playerCamera;

    private Vector3 moveDir;
    private Vector3 velocity;
    private int jumpCount;
    private float currentSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = PlayerState.Idle;
        currentSpeed = walkSpeed;

        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, normalCameraHeight, playerCamera.localPosition.z);
        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 14f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        Sprint();
        HandleCrouch();
        updateSpeed();
    }

    private void updateSpeed()
    {
        if (speedMeter != null)
        {
            float flatSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            speedMeter.text = $"Speed: {currentSpeed:F2}";
        }
    }
    void HandleCrouch()
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
    void EnterCrouch()
    {
        currentState = PlayerState.Crouching;
        currentSpeed = crouchSpeed;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 14f, 0);
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, crouchCameraHeight, playerCamera.localPosition.z);
    }
    void EnterSlide()
    {
        currentState = PlayerState.Sliding;
        controller.height = crouchColliderSize.y;
        controller.center = new Vector3(0, crouchColliderSize.y / 6f, 0);
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, crouchCameraHeight, playerCamera.localPosition.z);

        currentSpeed = walkSpeed * sprintMult * slideBoost;
        StartCoroutine(SlideTimer());
    }

    void ExitCrouch()
    {
        currentState = PlayerState.Walking;
        currentSpeed = walkSpeed;
        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 14f, 0);
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, normalCameraHeight, playerCamera.localPosition.z);
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
        {
            isGrounded = true;
            jumpCount = 0;
            velocity = Vector3.zero;

            if (IsSprintingHeld() && currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
            {
                currentState = PlayerState.Sprinting;
                currentSpeed = walkSpeed * sprintMult;
            }
            else if (currentState != PlayerState.Crouching && currentState != PlayerState.Sliding)
            {
                currentState = PlayerState.Walking;
                currentSpeed = walkSpeed;
            }

            if (currentState == PlayerState.Crouching)
                currentSpeed = crouchSpeed;
        }
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        Jump();

        controller.Move(velocity * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && isGrounded)
        {
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMult;
        }
        else if (Input.GetButtonUp("Sprint") && currentState == PlayerState.Sprinting)
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }
    }

    void ExitSlide()
    {
        if (Input.GetButton("Crouch"))
        {
            EnterCrouch();
        }
        else if (IsSprintingHeld())
        {
            currentState = PlayerState.Sprinting;
            currentSpeed = walkSpeed * sprintMult;
        }
        else
        {
            currentState = PlayerState.Walking;
            currentSpeed = walkSpeed;
        }

        controller.height = normalColliderSize.y;
        controller.center = new Vector3(0, normalColliderSize.y / 6f, 0);
        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, normalCameraHeight, playerCamera.localPosition.z);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            isGrounded = false;
            jumpCount++;
            velocity.y = jumpSpeed;
        }
    }
    bool IsSprintingHeld()
    {
        return Input.GetButton("Sprint");
    }

    private void Move(Vector3 vector3)
    {
        throw new NotImplementedException();
    }

    private IEnumerator SlideTimer()
    {
        yield return new WaitForSeconds(slideDuration);
        ExitSlide();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
      
    }
}
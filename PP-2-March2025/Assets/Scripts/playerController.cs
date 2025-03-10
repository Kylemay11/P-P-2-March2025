using UnityEngine;
using System;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movment Settings")]
    [Range(1, 5)][SerializeField] private int walkSpeed;
    [Range(1, 5)][SerializeField] private int sprintMult;

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
    [SerializeField] private bool isCrouching;

    private Vector3 moveDir;
    private Vector3 velocity;
    private int jumpCount;
    private int currentSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        sprint();
        HandleCrouch();
    }

    private void HandleCrouch()
    {
        if (Input.GetButton("Crouch") && currentSpeed == walkSpeed)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                controller.height = crouchColliderSize.y;
                controller.center = new Vector3(0, crouchColliderSize.y / 6f, 0);
                if (playerCamera != null)
                    playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, crouchCameraHeight, playerCamera.localPosition.z);
            }
        }
        else if (isCrouching && (!Input.GetButton("Crouch") || currentSpeed != crouchSpeed))
        {
            isCrouching = false;
            currentSpeed = walkSpeed;
            controller.height = normalColliderSize.y;
            controller.center = new Vector3(0, normalColliderSize.y / 14f, 0);
            if (playerCamera != null)
                playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, normalCameraHeight, playerCamera.localPosition.z);
        }
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
        {
            isGrounded = true;
            jumpCount = 0;
            velocity = Vector3.zero;

            if (isCrouching)
            {
                currentSpeed = crouchSpeed;
            }
        }
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        Jump();

        controller.Move(velocity * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            currentSpeed *= sprintMult;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            currentSpeed /= sprintMult;
        }
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

    private void Move(Vector3 vector3)
    {
        throw new NotImplementedException();
    }
}

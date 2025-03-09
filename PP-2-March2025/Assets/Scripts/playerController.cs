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

    private Vector3 moveDir;
    private Vector3 velocity;
    private bool isGrounded;
    private int jumpCount;


    private int currentSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        sprint();
    }

    void HandleMovement()
    {

        if (controller.isGrounded)
        {
            jumpCount = 0;
            velocity = Vector3.zero;
        }
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * walkSpeed * Time.deltaTime);

        Jump();

        controller.Move(velocity * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            walkSpeed *= sprintMult;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            walkSpeed /= sprintMult;
        }
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            jumpCount++;
            velocity.y = jumpSpeed;
        }
    }

    private void Move(Vector3 vector3)
    {
        throw new NotImplementedException();
    }
}

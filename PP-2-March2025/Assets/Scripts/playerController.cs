using UnityEngine;
using System;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movment Settings")]
    [Range(1, 5)][SerializeField] private int walkSpeed;
    [Range(1, 5)][SerializeField] private int sprintMult;

    private Vector3 moveDir;



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
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * walkSpeed * Time.deltaTime);
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            walkSpeed *= sprintMult;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            walkSpeed /= sprintMult;
        }
    }

    private void Move(Vector3 vector3)
    {
        throw new NotImplementedException();
    }
}

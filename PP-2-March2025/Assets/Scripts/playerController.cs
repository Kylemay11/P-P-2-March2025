using UnityEngine;
using System;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] int speed;

    Vector3 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

    }

    private void Move(Vector3 vector3)
    {
        throw new NotImplementedException();
    }
}

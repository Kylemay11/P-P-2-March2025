using UnityEngine;

public class WeaponIdleSway : MonoBehaviour
{
    [SerializeField] private float idleSwayAmount;
    [SerializeField] private float idleSwaySpeed;

    [SerializeField] private float walkSwayAmount;
    [SerializeField] private float walkSwaySpeed;

    [SerializeField] private float sprintSwayAmount;
    [SerializeField] private float sprintSwaySpeed;

    [SerializeField] private playerController player; // Reference to playerController

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;

        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<playerController>();
    }

    void Update()
    {
        float swayAmount = idleSwayAmount;
        float swaySpeed = idleSwaySpeed;

        switch (player.CurrentState)
        {
            case PlayerState.Idle:
            case PlayerState.Crouching:
                swayAmount = idleSwayAmount;
                swaySpeed = idleSwaySpeed;
                break;

            case PlayerState.Walking:
                swayAmount = walkSwayAmount;
                swaySpeed = walkSwaySpeed;
                break;

            case PlayerState.Sprinting:
                swayAmount = sprintSwayAmount;
                swaySpeed = sprintSwaySpeed;
                break;

            case PlayerState.Sliding:
                swayAmount = 0f;
                break;
        }

        float swayOffset = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        transform.localPosition = startPosition + new Vector3(swayOffset, 0f, 0f);

        transform.localRotation = Quaternion.Euler(0f, 0f, -swayOffset * 10f);
    }
}

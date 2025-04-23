using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController characterController;

    [Header("Head Bob Settings")]
    [SerializeField] private float walkBobSpeed;
    [SerializeField] private float walkBobAmount;
    [SerializeField] private float runBobSpeed;
    [SerializeField] private float runBobAmount;
    [SerializeField] private float transitionSpeed;
    [SerializeField][Range(0, 1)] private float horizontalBobFactor;

    private Vector3 originalLocalPosition;
    private float timer = 0;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = transform;
        if (characterController == null) characterController = GetComponentInParent<CharacterController>();

        // Store the original local position of the camera
        originalLocalPosition = playerCamera.localPosition;
    }

    private void Update()
    {
        HandleHeadBob();
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded)
        {
            ResetCameraPosition();
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(horizontal, 0, vertical);

        if (inputVector.magnitude > 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;

            timer += Time.deltaTime * bobSpeed;

            // Calculate bob offset (centered around zero)
            float verticalBob = Mathf.Sin(timer) * bobAmount;
            float horizontalBob = Mathf.Sin(timer * 0.5f) * bobAmount * horizontalBobFactor;

            // Apply offset to original position
            Vector3 newPosition = originalLocalPosition +
                                new Vector3(horizontalBob, verticalBob, 0);

            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                newPosition,
                Time.deltaTime * transitionSpeed
            );
        }
        else
        {
            ResetCameraPosition();
        }
    }

    private void ResetCameraPosition()
    {
        timer = 0;
        playerCamera.localPosition = Vector3.Lerp(
            playerCamera.localPosition,
            originalLocalPosition,
            Time.deltaTime * transitionSpeed
        );
    }
}